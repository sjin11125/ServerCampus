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
using System.Linq;
using System;
using static Com2usServerCampus.LogManager;

namespace Com2usServerCampus.Controllers;
[ApiController]
[Route("[controller]")]
public class AcquireStageItemController
{
    readonly ILogger<AcquireStageItemController> _logger;
    readonly IRedisDB _redisDB;
    readonly IMasterDataDB _masterDataDB;

    public AcquireStageItemController(ILogger<AcquireStageItemController> logger, IRedisDB redisDB, IMasterDataDB masterDataDB)
    {
        _logger = logger;
        _redisDB = redisDB;
        _masterDataDB = masterDataDB;
    }
    [HttpPost]
    public async Task<AcquireStageItemResponse> Post(AcquireStageItemRequest stageItemInfo)
    {
        AcquireStageItemResponse acquireStageItemResponse = new AcquireStageItemResponse();

        (var stageItemData, var stageItemList) = _masterDataDB.GetStageItem(stageItemInfo.StageCode); //해당 아이템이 해당 스테이지 아이템인지 확인 (마스터데이터)

        if (stageItemData != ErrorCode.None)          //정보가 없다면
        {
            acquireStageItemResponse.Error = stageItemData;
            return acquireStageItemResponse;
        }


        (var getStageItemError, var index, var itemCount) = await _redisDB.GetUserStageItem(stageItemInfo.UserId, stageItemInfo.ItemCode, stageItemInfo.StageCode);
        //레디스에 해당 아이템 정보를 불러옴

        if (getStageItemError != ErrorCode.None)
        {
            acquireStageItemResponse.Error = getStageItemError;
            return acquireStageItemResponse;
        }
        try
        {

            int masterDataStageItemCount = stageItemList.Count(x => x.ItemCode == stageItemInfo.ItemCode);


            itemCount += 1;

            //레디스에 있는 해당 아이템 수 + 1(이번에 새로 얻은 수) 가 마스터 데이터와 맞는지 확인
            if (masterDataStageItemCount < itemCount)                   //맞지않으면 에러 리턴
            {
                acquireStageItemResponse.Error = ErrorCode.NotMatchStageItemData;
                return acquireStageItemResponse;
            }

        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"  UserId:{stageItemInfo.UserId} ");
        }
        //검증 끝
        //레디스에 아이템 정보 저장
        var error = await _redisDB.SetUserStageItem(stageItemInfo.UserId, stageItemInfo.StageCode, stageItemInfo.ItemCode, itemCount, index); //레디스에 아이템 정보 저장

        if (error != ErrorCode.None)        //에러면
        {
            acquireStageItemResponse.Error = error;
            return acquireStageItemResponse;
        }



        _logger.ZLogInformationWithPayload(EventIdDictionary[EventType.AcquireStageItem], new { UserId = stageItemInfo.UserId }, $"AcquireStageItem Success");

        return acquireStageItemResponse;
    }

}

