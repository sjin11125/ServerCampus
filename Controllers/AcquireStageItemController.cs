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
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Security.Cryptography;
using ZLogger;
namespace Com2usServerCampus.Controllers;
[ApiController]
[Route("[controller]")]
public class AcquireStageItemController
{
    readonly ILogger<AcquireStageItemController> _logger;
    readonly IRedisDB _redisDB;

    public AcquireStageItemController(ILogger<AcquireStageItemController> logger, IRedisDB redisDB)
    {
        _logger = logger;
        _redisDB = redisDB;
    }
    [HttpPost]
    public async Task<AcquireStageItemResponse> Post(AcquireStageItemRequest stageItemInfo)
    {
        AcquireStageItemResponse acquireStageItemResponse = new AcquireStageItemResponse();

        var error = await _redisDB.SetUserStageItem(stageItemInfo.UserId, stageItemInfo.ItemCode, stageItemInfo.StageId); //레디스에 아이템 정보 저장
       
        if (error != ErrorCode.None)        //에러면
        {
            acquireStageItemResponse.Error = error;
            return acquireStageItemResponse;
        }

        return acquireStageItemResponse;
    }
}

