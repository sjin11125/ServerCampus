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
using System.Net.Mail;
using System.Security.Cryptography;
using ZLogger;
using static Com2usServerCampus.LogManager;

namespace Com2usServerCampus.Controllers;
[ApiController]
[Route("[controller]")]
public class StageSelectController:ControllerBase
{
    readonly ILogger<StageSelectController> _logger;
    readonly IMasterDataDB _masterDataDB;
    readonly IGameDB _gameDB;
    readonly IRedisDB _redisDB;

    public StageSelectController(ILogger<StageSelectController> logger, IMasterDataDB masterDataDB, IGameDB gameDB, IRedisDB redisDB)
    {
        _logger = logger;
        _masterDataDB = masterDataDB;
        _gameDB = gameDB;
        _redisDB = redisDB;
    }
    [HttpPost]
    public async Task<StageSelectResponse> StagePost(StageSelectRequest stageInfo)
    {
        StageSelectResponse stageResonse = new StageSelectResponse();

        var isSelect = await IsStageSelectable(stageInfo.UserId,stageInfo.StageId);             //유저가 스테이지를 선택 가능한지 검증
        if (isSelect!=ErrorCode.None)
        {
            stageResonse.Error = isSelect;
            return stageResonse;

        }

        (var stageDataError, var stageDataInfo)= GetData(stageInfo.StageId);         //마스터데이터에서 해당 스테이지 정보들 불러오기
        if (stageDataError != ErrorCode.None)
        {
            stageResonse.Error = stageDataError;
            return stageResonse;
        }


        //레디스에 사용자 상태 넣기
        var authUser = (AuthUser)HttpContext.Items[nameof(AuthUser)]!;
       var tokenError=  await _redisDB.UpdateUserToken(stageInfo.UserId, authUser.AuthToken, (int)authUser.AccountId);
        if (tokenError != ErrorCode.None)
        {
            stageResonse.Error = tokenError;
            return stageResonse;
        }



        stageResonse.StageItems = stageDataInfo.StageItmes;
        stageResonse.StageNPCs = stageDataInfo.StageNPCs;
        stageResonse.Error = ErrorCode.None;

        _logger.ZLogInformationWithPayload(EventIdDictionary[EventType.StageSelect], new { UserId = stageInfo.UserId }, $"StageSelect Success");

        return stageResonse;    

    }
    public async Task<ErrorCode> IsStageSelectable(string userId,int selectedStageId)
    {
        if (selectedStageId<0)
        {
            return ErrorCode.SelectStageError;
        }
        (var stageError, var stage) = await _gameDB.GetUserStageInfo(userId);       //유저가 클리어한 스테이지 불러옴
        if (stageError != ErrorCode.None)
        {
            return stageError;
        }

        if (stage < selectedStageId) //선택 가능한지 검증(선택한 스테이지가 클리어한 스테이지 보다 더 많다)
        {
            return ErrorCode.SelectStageError;
        }

        return ErrorCode.None;
    }

    public (ErrorCode, StageInfo) GetData(int stage)
    {
        StageInfo stageInfos = new StageInfo();

        stageInfos.StageId = stage;

        (var stageItemError, var stageItem) = _masterDataDB.GetStageItem(stage);  //던전에 생성될 아이템을 리스트 보냄 (마스터데이터)
        if (stageItemError != ErrorCode.None)
        {
            return (stageItemError, null);

        }

        stageInfos.StageItmes = stageItem;


        (var stageNPCError, var stageNPC) = _masterDataDB.GetStageNPC(stage);   //적 NPC 리스트를 보냄 (마스터데이터)
        if (stageItemError != ErrorCode.None)
        {
            return (stageItemError, null);

        }

        stageInfos.StageNPCs = stageNPC;



        return (ErrorCode.None, stageInfos);
    }
}

