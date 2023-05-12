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
namespace Com2usServerCampus.Controllers;
public class StageController
{
    readonly ILogger _logger;
    readonly IGameDB _gameDB;

    public StageController(ILogger logger, IGameDB gameDB)
    {
        _logger = logger;
        _gameDB = gameDB;
    }
    [HttpPost]
    public async Task<StageResponse> StagePost(StageRequest stageInfo)
    {
        StageResponse stageResonse = new StageResponse();
        //완료한 스테이지 리스트를 요청

        (var stageError, var stage) = await _gameDB.GetUserStageInfo(stageInfo.UserId);       //유저가 클리어한 스테이지 불러옴
        if (stageError != ErrorCode.None)
        {
            stageResonse.Error= stageError;
            return stageResonse;
        }

        stageResonse.Error=ErrorCode.None;
        stageResonse.StageId = stage;


        return stageResonse;



       (var stageDataError, var stageDataInfo)= GetData(stage);         //마스터데이터에서 클리어한 스테이지 정보들 불러오기
        if (stageDataError != ErrorCode.None)
        {
            stageResonse.Error = stageDataError;
            return stageResonse;
        }


    }

    public (ErrorCode, List<StageInfo>) GetData(int stage)
    {
        List<StageInfo> stageInfos = new List<StageInfo>();

        for (int i = 0; i < stage; i++)
        {
            StageInfo stageInfo = new StageInfo();


            (var stageItemError, var stageItem) = _masterDataDB.GetStageItem(i);  //클리어한 스테이지들에 대한 아이템정보를 가져오기 (마스터데이터)
            if (stageItemError != ErrorCode.None)
            {
                return (stageItemError, null);

            }

            stageInfo.StageItmes = stageItem;


            (var stageNPCError, var stageNPC) = _masterDataDB.GetStageNPC(i);  //클리어한 스테이지들에 대한 npc정보를 가져오기 (마스터데이터)
            if (stageItemError != ErrorCode.None)
            {
                return (stageItemError, null);

            }

            stageInfo.StageNPCs = stageNPC;


            stageInfos.Add(stageInfo);
        }

        return (ErrorCode.None, stageInfos);
    }
}

