
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

        (var itemRedisError, var itemRedisData) = await _redisDB.GetUserStageItem();   //레디스에 있는 유저가 얻은 아이템 정보들 불러옴



        // 레디스에 있던 유저가 잡은 npc 정보들을 불러와서 마스터데이터에 있는 npc들을 비교

        //맞다면 DB의 아이템 테이블에 저장


        return endStageResponse;
    }

}

