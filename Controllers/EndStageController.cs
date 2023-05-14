
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
namespace Com2usServerCampus.Controllers;

[ApiController]
[Route("[controller]")]
    public class EndStageController:ControllerBase
    {
    readonly ILogger<EndStageController> _logger;
    readonly IMasterDataDB _masterDataDB;
    readonly IRedisDB _redisDB;

    public EndStageController(ILogger<EndStageController> logger, IMasterDataDB masterDataDB, IRedisDB redisDB)
    {
        _logger = logger;
        _masterDataDB = masterDataDB;
        _redisDB = redisDB;
    }

    [HttpPost]
    public async Task<EndStageResponse> Post(EndStageRequest endStageInfo)
    {
        EndStageResponse endStageResponse = new EndStageResponse();

        //클리어 못했다면
        //레디스에 저장된 정보 지우기 (아이템, npc)
        if (!endStageInfo.isClear)
        {
          var removeItemKey=  await _redisDB.DeleteUserStageItem(endStageInfo.UserId,endStageInfo.StageCode);
            if (removeItemKey!=ErrorCode.None)
            {
                endStageResponse.Error = removeItemKey;
                return endStageResponse;
            }

            var removeNpcKey = await _redisDB.DeleteUserStageNPC(endStageInfo.UserId, endStageInfo.StageCode);
            if (removeNpcKey != ErrorCode.None)
            {
                endStageResponse.Error = removeNpcKey;
                return endStageResponse;
            }
        }



        //클리어했다면
        //레디스에 있던 유저가 얻은 아이템을 마스터데이터에 있는 아이템 정보와 비교


        (var itemMasterError, var itemMasterData) = _masterDataDB.GetStageItem(endStageInfo.StageCode); //스테이지 아이템 마스터데이터 가져옴
        if (itemMasterError != ErrorCode.None)
        {
            endStageResponse.Error = itemMasterError;
            return endStageResponse;
        }

        (var itemRedisError, var itemRedisData) = await _redisDB.GetAllUserStageItem(endStageInfo.UserId, endStageInfo.StageCode);   //레디스에 있는 유저가 얻은 아이템 정보들 불러옴
        if (itemRedisError != ErrorCode.None)
        {
            endStageResponse.Error = itemRedisError;
            return endStageResponse;
        }

        (var npcMasterError, var npcMasterData) = _masterDataDB.GetStageNPC(endStageInfo.StageCode); //스테이지 npc 마스터데이터 가져옴
        if (itemMasterError != ErrorCode.None)
        {
            endStageResponse.Error = itemMasterError;
            return endStageResponse;
        }

        (var npcRedisError, var npcRedisData) = await _redisDB.GetAllUserStageNPC(endStageInfo.UserId, endStageInfo.StageCode);   //레디스에 있던 유저가 잡은 npc 정보들을 불러옴
        if (npcRedisError != ErrorCode.None)
        {
            endStageResponse.Error = npcRedisError;
            return endStageResponse;
        }

        //마스터데이터와 레디스에 저장된 정보 비교 (아이템 수)
        if (itemMasterData.Count != 0)
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
                    _logger.ZLogError(e, $" ErrorCode: {ErrorCode.GetUserStageNPCFail} Email:{endStageInfo.UserId} ItemCode:{item.ItemCode} StageNum: {endStageInfo.UserId} ");    //레디스에 스테이지 아이템 넣기 실패 에러

                    endStageResponse.Error = ErrorCode.EndStageException;
                    return endStageResponse;
                }
            }


            if (itemRedisData.Count!=0)            //레디스 아이템 정보에서 마스터데이터의 아이템 외 다른 아이템 정보가 있거나 아이템의 갯수가 더 많으면
            {
                endStageResponse.Error = ErrorCode.NotMatchStageItemData;       //에러
                return endStageResponse;
            }
        }


        //마스터데이터와 레디스에 저장된 정보 비교 (npc 수)
        if (npcRedisData.Count!=0)
        {
            foreach (var item in npcMasterData)
            {
                try
                {

                    var tempNPC = npcRedisData.Find(x => x.NPCCode == item.NPCCode);
                    if (tempNPC != null)           //레디스에 해당 아이템이 있을때
                    {
                        tempNPC.Count -= item.Count;

                        if (tempNPC.Count <= 0)
                        {
                            npcRedisData.Remove(tempNPC);
                        }
                    }

                }
                catch (Exception e)                 //
                {
                    _logger.ZLogError(e, $" ErrorCode: {ErrorCode.GetUserStageNPCFail} Email:{endStageInfo.UserId} NPCCode:{item.NPCCode} StageNum: {endStageInfo.UserId} ");    //레디스에 스테이지 아이템 넣기 실패 에러

                    endStageResponse.Error = ErrorCode.EndStageException;
                    return endStageResponse;
                }
            }

            if (npcRedisData.Count!=0)      //레디스 npc 정보에서 마스터데이터의 npc 외 다른 npc 정보가 있거나 npc 처치 수가 더 많으면
            {
                endStageResponse.Error = ErrorCode.NotMatchStageNPCData;       //에러
                return endStageResponse;
            }
        }

        //맞다면 DB의 아이템 테이블에 저장
        if (itemRedisData.Count==0&&npcRedisData.Count==0)
        {
            //아이템 테이블에 마스터 데이터 아이템 넣기
            //유저의 게임 정보 테이블에 경험치 계산 해서 넣기
        }

        return endStageResponse;
    }

}

