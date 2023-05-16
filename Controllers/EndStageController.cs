
using CloudStructures;
using CloudStructures.Structures;
using Com2usServerCampus.Model;
using Com2usServerCampus.ModelReqRes;
using Com2usServerCampus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using SqlKata.Execution;
using StackExchange.Redis;
using System.Reflection.Emit;
using System;
using System.Security.Cryptography;
using ZLogger;
using static Com2usServerCampus.LogManager;

namespace Com2usServerCampus.Controllers;

[ApiController]
[Route("[controller]")]
    public class EndStageController:ControllerBase
    {
    readonly ILogger<EndStageController> _logger;
    readonly IMasterDataDB _masterDataDB;
    readonly IRedisDB _redisDB;
    readonly IGameDB _gameDB;

    public EndStageController(ILogger<EndStageController> logger, IMasterDataDB masterDataDB, IRedisDB redisDB, IGameDB gameDB)
    {
        _logger = logger;
        _masterDataDB = masterDataDB;
        _redisDB = redisDB;
        _gameDB = gameDB;
    }

    [HttpPost]
    public async Task<EndStageResponse> Post(EndStageRequest endStageInfo)
    {
        EndStageResponse endStageResponse = new EndStageResponse();



        var authUser = (AuthUser)HttpContext.Items[nameof(AuthUser)]!;


        var isGame = await _redisDB.CheckPlayGmae(authUser.Email, authUser.AuthToken, (int)authUser.AccountId); //게임중인지 확인
        if (isGame != ErrorCode.None)         //아니라면 에러
        {
            endStageResponse.Error = isGame;
            return endStageResponse;
        }
        //클리어 못했다면
        //레디스에 저장된 정보 지우기 (아이템, npc)
        if (!endStageInfo.isClear)
        {
            var NotClearError = await DeleteReidsKey(endStageInfo.UserId, endStageInfo.StageCode);
            if (NotClearError != ErrorCode.None)
            {
                endStageResponse.Error = NotClearError;
                return endStageResponse;
            }
        }



        //클리어했다면
        //레디스에 있던 유저가 얻은 아이템을 마스터데이터에 있는 아이템 정보와 비교


        //마스터데이터 불러옴
        (var getMasterDataError, var itemMasterData, var npcMasterData) = GetStageMasterData(endStageInfo.StageCode);

        if (getMasterDataError != ErrorCode.None)
        {
            endStageResponse.Error = getMasterDataError;
            return endStageResponse;
        }



        //레디스에서 데이터 불러옴
        (var getRedisDataError, var itemRedisData, var npcRedisData) = await GetRedisData(endStageInfo.UserId, endStageInfo.StageCode);

        if (getRedisDataError != ErrorCode.None)
        {
            endStageResponse.Error = getRedisDataError;
            return endStageResponse;
        }


        //마스터데이터와 레디스에 저장된 정보 비교 (아이템 수)
        var itemCheckError = ItemDataCheck(itemRedisData, itemMasterData, endStageInfo.UserId, endStageInfo.StageCode);
        if (itemCheckError != ErrorCode.None)
        {
            endStageResponse.Error = itemCheckError;
            return endStageResponse;
        }


        //마스터데이터와 레디스에 저장된 정보 비교 (npc 수)
        (var npcCheckError, int totalEXP) = NpcDataCheck(npcRedisData, npcMasterData, endStageInfo.UserId, endStageInfo.StageCode);
        if (npcCheckError != ErrorCode.None)
        {
            endStageResponse.Error = npcCheckError;
            return endStageResponse;
        }



        //전부 다 정보가 맞다면 DB의 아이템 테이블과 게임테이블에 해당 정보 저장

        //아이템 테이블에 마스터 데이터 아이템 넣기
        var takeRewardError = await TakeReward(itemRedisData, endStageInfo.UserId);
        if (takeRewardError != ErrorCode.None)
        {
            endStageResponse.Error = takeRewardError;
            return endStageResponse;
        }


        //유저의 게임 정보 테이블에 경험치 넣고 클리어한 스테이지 업뎃
        var updateUserGameDataError = await _gameDB.UpdateStageClearData(new EndStageResult
        {
            UserId = endStageInfo.UserId,
            TotalEXP = totalEXP,
            StageCode = endStageInfo.StageCode + 1

        });

        if (updateUserGameDataError != ErrorCode.None)
        {
            endStageResponse.Error = updateUserGameDataError;
            return endStageResponse;
        }

        //레디스 키 삭제
        var deleteRedisKeyError = await DeleteReidsKey(endStageInfo.UserId, endStageInfo.StageCode);
        if (deleteRedisKeyError != ErrorCode.None)
        {
            endStageResponse.Error = deleteRedisKeyError;
            return endStageResponse;
        }

        var endGameError = await _redisDB.SetUserToken(authUser.Email,authUser.AuthToken,(int)authUser.AccountId); //레디스에 있는 유저 상태 변경
        if (endGameError != ErrorCode.None)
        {
            endStageResponse.Error = endGameError;
            return endStageResponse;

        }

        _logger.ZLogInformationWithPayload(EventIdDictionary[EventType.EndStage], new { UserId = endStageInfo.UserId }, $"EndStage Success");


        return endStageResponse;
    }

    public async Task<ErrorCode> TakeReward(List<AcquireStageItem> itemRedisData, string userId)
    {
        foreach (var item in itemRedisData)
        {
            (var getItemDataError, var itemInfo) = _masterDataDB.GetItemData(item.ItemCode); //아이템 마스터데이터 불러오기
            if (getItemDataError != ErrorCode.None)
            {
                return getItemDataError;
            }

            var insertItemError = await _gameDB.InsertItem(itemInfo.isCount, new UserItem           //아이템 테이블에 아이템 넣기
            {
                UserId = userId,
                ItemCode = item.ItemCode,
                ItemCount = 1,
                EnhanceCount = 1,
                Attack = itemInfo.Attack,
                Defence = itemInfo.Defence,
                Magic = itemInfo.Magic,

            });
            if (insertItemError != ErrorCode.None)
            {
                return insertItemError;
            }

        }
        return ErrorCode.None;
    }


    public async Task<ErrorCode> DeleteReidsKey(string userId,int stageCode)
    {
        var removeItemKey = await _redisDB.DeleteUserStageItemData(userId, stageCode);
        if (removeItemKey != ErrorCode.None)
        {
            return removeItemKey;
        }

        var removeNpcKey = await _redisDB.DeleteUserStageNPCData(userId, stageCode);
        if (removeNpcKey != ErrorCode.None)
        {
            return removeNpcKey;
        }

        return ErrorCode.None;
    }



    public (ErrorCode,int )NpcDataCheck(List<KillStageNPC> npcRedisData,List<StageNPC> npcMasterData,string userId,int stageCode)
    {
        int totalEXP = 0;

        if (npcRedisData is not null)
        {
            foreach (var item in npcMasterData)
            {
                try
                {

                    var tempNPC = npcRedisData.Find(x => x.NPCCode == item.NPCCode);
                    if (tempNPC != null)           //레디스에 해당 아이템이 있을때
                    {
                        tempNPC.Count -= item.Count;

                        totalEXP += item.Exp * item.Count;            //경험치 계산

                        if (tempNPC.Count <= 0)
                        {
                            npcRedisData.Remove(tempNPC);
                        }
                    }

                }
                catch (Exception e)                 //
                {
                    _logger.ZLogError(e, $" ErrorCode: {ErrorCode.GetUserStageNPCFail} UserId:{userId} NPCCode:{item.NPCCode} StageNum: {stageCode} ");    //레디스에 스테이지 아이템 넣기 실패 에러

                    return (ErrorCode.EndStageException,0);
                }
            }

            if (npcRedisData.Count != 0)      //레디스 npc 정보에서 마스터데이터의 npc 외 다른 npc 정보가 있거나 npc 처치 수가 더 많으면
            {
                return (ErrorCode.NotMatchStageNPCData,0);
            }
        }

        return (ErrorCode.None,totalEXP);
    }

    public ErrorCode ItemDataCheck(List<AcquireStageItem> itemRedisData,List<StageItem> itemMasterData, string userId, int stageCode)
    {
        if (itemRedisData is not null)
        {
            foreach (var item in itemMasterData)
            {
                try
                {

                    var tempItem = itemRedisData.Find(x => x.ItemCode == item.ItemCode);
                    if (tempItem != null)           //레디스에 해당 아이템이 있을때
                    {
                        tempItem.ItemCode -= -1;

                        if (tempItem.ItemCode == 0)
                        {
                            itemRedisData.Remove(tempItem);
                        }
                    }

                }
                catch (Exception e)                 //
                {
                    _logger.ZLogError(e, $" ErrorCode: {ErrorCode.GetUserStageNPCFail} UserId:{userId} ItemCode:{item.ItemCode} StageNum: {stageCode} ");    //레디스에 스테이지 아이템 넣기 실패 에러

                    return ErrorCode.EndStageException;
                }
            }


            if (itemRedisData.Count != 0)            //레디스 아이템 정보에서 마스터데이터의 아이템 외 다른 아이템 정보가 있거나 아이템의 갯수가 더 많으면
            {
                return ErrorCode.NotMatchStageItemData;
            }


        }

        return ErrorCode.None;
    }

    public (ErrorCode,List<StageItem>,List<StageNPC>) GetStageMasterData(int stageCode)     //해당 스테이지의 아이템,npc 마스터데이터 불러오기
    {
        (var itemMasterError, var itemMasterData) = _masterDataDB.GetStageItem(stageCode); //스테이지 아이템 마스터데이터 가져옴
        if (itemMasterError != ErrorCode.None)
        {
            return (itemMasterError,null,null);
        }

        (var npcMasterError, var npcMasterData) = _masterDataDB.GetStageNPC(stageCode); //스테이지 npc 마스터데이터 가져옴
        if (itemMasterError != ErrorCode.None)
        {
            return (itemMasterError,null,null);
        }

        return (ErrorCode.None, itemMasterData, npcMasterData);
    }


    public async Task<(ErrorCode,List<AcquireStageItem>,List<KillStageNPC>)> GetRedisData(string userId,int stageCode)      //레디스에 저장되어 있는 데이터 불러오기
    {
        (var itemRedisError, var itemRedisData) = await _redisDB.GetAllUserStageItem(userId,stageCode);   //레디스에 있는 유저가 얻은 아이템 정보들 불러옴
        if (itemRedisError != ErrorCode.None)
        {
            return (itemRedisError,null,null);
        }


        (var npcRedisError, var npcRedisData) = await _redisDB.GetAllUserStageNPC(userId, stageCode);   //레디스에 있던 유저가 잡은 npc 정보들을 불러옴
        if (npcRedisError != ErrorCode.None)
        {
            return (npcRedisError,null,null);
        }


        return (ErrorCode.None,itemRedisData,npcRedisData);
    }
}



