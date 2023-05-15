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

public class EnhanceController : ControllerBase
{
    ILogger<EnhanceController> _logger;
    IGameDB _gameDB;
    IMasterDataDB _masterDataDB;

    public EnhanceController(ILogger<EnhanceController> logger, IGameDB gameDB, IMasterDataDB masterDataDB)
    {
        _logger = logger;
        _gameDB = gameDB;
        _masterDataDB = masterDataDB;
    }

    [HttpPost]
    public async Task<EnhanceResponse> Post(EnhanceRequest enhanceInfo)
    {
        var userInfo = (AuthUser)HttpContext.Items[nameof(AuthUser)]!;

        EnhanceResponse enhanceResult=new EnhanceResponse();

        (var error, var item, var itemdata, var itemAttributedata) = await GetData(enhanceInfo.ItemId);     //유저 아이템, 아이템 마스터데이터, 아이템 속성 마스터데이터 불러오기

        if (error!=ErrorCode.None)
        {
            enhanceResult.Error = error;
            return enhanceResult;
        }

        if (itemdata.EnhanceMaxCount==0)//무기, 방어구아니면 넘겨
        {
            enhanceResult.Error = ErrorCode.NotEnhanceType;
            return enhanceResult;
        }
        if (item.EnhanceCount>=itemdata.EnhanceMaxCount)//강화 횟수 10회 이상인지 검사
        {
            enhanceResult.Error = ErrorCode.MaxEnhanceCount;
            return enhanceResult;
        }


        (var isSuccess, var attack, var defence) = Enhancement(itemAttributedata, item.Attack, item.Defence);
        if (isSuccess) {

            var itemUpdate = await _gameDB.UpdateItem( new UserItem
            {      // 아이템 업데이트
                UserId = enhanceInfo.Email,
                ItemCode = item.ItemCode,
                ItemId = enhanceInfo.ItemId,
                EnhanceCount = item.EnhanceCount + 1,
                ItemCount = item.ItemCount,
                Attack = attack,
                Defence = defence,
                Magic = item.Magic

            });
            if (itemUpdate != ErrorCode.None)
            {
                enhanceResult.Error = itemUpdate;
                return enhanceResult;
            }

        }
        else
        {
            var deleteReuslt = await _gameDB.DeleteItem( enhanceInfo.ItemId);   //실패하면 아이템 지우기
            if (deleteReuslt != ErrorCode.None)
            {
                enhanceResult.Error = deleteReuslt;
                return enhanceResult;
            }
        }

        return enhanceResult;
    }



    public (bool,int,int) Enhancement(ItemAttribute attribute,int attack,int defence)
    {

        Random rand = new Random(DateTime.Now.Millisecond);
        int EnhanceValue = rand.Next(1, 101);//강화 성공 실패 결정(확률 30퍼)

        if (EnhanceValue > 30)          //실패
        {
            return (false,attack,defence);
        }


        //성공하면 아이템의 강화 횟수 +1, 무기인 경우 공격력, 방어구일 경우 방어력 수치 10퍼 상승
        if (attribute.Name == "무기")
        {
            attack += (int)(attack * 0.1);

        }
        else
        {
            defence += (int)(defence * 0.1);

        }
        return (true,attack,defence);


    }

    public async Task<(ErrorCode,UserItem,ItemData,ItemAttribute)> GetData(int id)
    {
        (var errorGetItem, var item) = await _gameDB.GetItem(id); //아이템 데이터에서 해당 유저의 아이템 받아옴
        if (errorGetItem != ErrorCode.None)
        {
            return (errorGetItem, null,null,null);
        }

        (var errorItemdata, var itemdata) = _masterDataDB.GetItemData(item.ItemCode);  //해당 아이템의 마스터 데이터 불러옴
        if (errorItemdata != ErrorCode.None)
        {
            return (errorItemdata,null,null,null);
        }

        (var errorItemAttribute, var itemAttributedata) = _masterDataDB.GetItemAttributeData(item.ItemCode);  //해당 아이템 특성의 마스터 데이터 불러옴
        if (errorItemAttribute != ErrorCode.None)
        {
            return (errorItemAttribute, null, null, null);
        }

        return (ErrorCode.None, item, itemdata, itemAttributedata);
    }
}

