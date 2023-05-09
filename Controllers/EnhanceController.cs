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
using ZLogger;
using static Com2usServerCampus.LogManager;

namespace Com2usServerCampus.Controllers;

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
        EnhanceResponse enhanceResult=new EnhanceResponse   ();

        (var errorGetItem, var item) = await _gameDB.GetItem(enhanceInfo.Email,enhanceInfo.Id); //아이템 데이터에서 해당 유저의 아이템 받아옴
        if (errorGetItem != ErrorCode.None)
        {
            enhanceResult.Error = errorGetItem;
            return enhanceResult;
        }

       (var errorItemdata, var itemdata) = _masterDataDB.GetItemData(item.ItemCode);  //해당 아이템의 마스터 데이터 불러옴
        if (errorItemdata != ErrorCode.None)
        {
            enhanceResult.Error = errorItemdata;
            return enhanceResult;
        }

       (var errorItemAttribute, var itemAttributedata) =  _masterDataDB.GetItemAttributeData(item.ItemCode);  //해당 아이템 특성의 마스터 데이터 불러옴
        if (errorItemAttribute != ErrorCode.None)
        {
            enhanceResult.Error = errorItemAttribute;
            return enhanceResult;
        }
        if (itemdata.EnhanceMaxCount==0)//무기, 방어구아니면 넘겨
        {
            enhanceResult.Error = ErrorCode.NotEnhanceType;
            return enhanceResult;
        }
        if (item.EnhanceCount>=10)//강화 횟수 10회 이상인지 검사
        {
            enhanceResult.Error = ErrorCode.MaxEnhanceCount;
            return enhanceResult;
        }

        Random rand=new Random();

        int EnhanceValue = rand.Next(1,101);//강화 성공 실패 결정(확률 30퍼)
        bool isSuccess = false;
        int before, after;
        if (EnhanceValue <= 30)//강화 성공
        {
            isSuccess=true; 

            int newAttack, newDefence;

            //성공하면 아이템의 강화 횟수 +1, 무기인 경우 공격력, 방어구일 경우 방어력 수치 10퍼 상승
            if (itemAttributedata.Name == "무기")
            {
                newAttack = (item.Attack + (int)(item.Attack * 0.1));
                newDefence = item.Defence;

                before = item.Attack;
                after = newAttack;
            }
            else
            {
                newAttack = item.Attack;
                newDefence = (item.Defence + (int)(item.Defence * 0.1));

                before = item.Defence;
                after = newDefence;
            }
            var itemUpdate = await _gameDB.UpdateItem(enhanceInfo.Email, new UserItem
            {      // 아이템 업데이트
                Eamil = enhanceInfo.Email,
                ItemCode = item.ItemCode,
                Id = enhanceInfo.Id,
                EnhanceCount = item.EnhanceCount + 1,
                ItemCount = item.ItemCount,
                Attack = newAttack,
                Defence = newDefence,
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
            var deleteReuslt = await _gameDB.DeleteItem(enhanceInfo.Email, enhanceInfo.Id);   //실패하면 아이템 지우기
            if (deleteReuslt != ErrorCode.None)
            {
                enhanceResult.Error = deleteReuslt;
                return enhanceResult;
            }

            if (itemAttributedata.Name == "무기")
            {
                before = item.Attack;
                after = item.Attack;
            }
            else
            {
                before = item.Defence;
                after = item.Defence;
            }
        }
        var enhanceInfoReuslt = await _gameDB.InsertEnhanceInfo(enhanceInfo.Email, new EnhanceItemInfo
        {   //강화 단계 이력 정보 추가

            Email = enhanceInfo.Email,
            Id = enhanceInfo.Id,
            ItemCode = item.ItemCode,
            EnhanceCount = item.EnhanceCount,
            Attribute = itemAttributedata.Name,
            BeforeValue = before,
            AfterValue = after,
            isSuccess = isSuccess,
            Date = DateTime.Now,

        });
        if (enhanceInfoReuslt != ErrorCode.None)
        {
            enhanceResult.Error = enhanceInfoReuslt;
            return enhanceResult;
        }

        return enhanceResult;
    }
}

