﻿using CloudStructures;
using CloudStructures.Structures;
using Com2usServerCampus.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;
using Com2usServerCampus.ModelReqRes;
using Com2usServerCampus.Services;
using Com2usServerCampus;

namespace APIServer.Controllers;
[ApiController]
[Route("[controller]")]
public class InAppPurchaseController : ControllerBase
{
    ILogger<InAppPurchaseController> _logger;
    IGameDB _gameDB;
    IMasterDataDB _masterDataDB;

    public InAppPurchaseController(ILogger<InAppPurchaseController> logger, IGameDB gameDB, IMasterDataDB masterDataDB)
    {
        _logger = logger;
        _gameDB = gameDB;
        _masterDataDB = masterDataDB;
    }

    [HttpPost]
    public async Task<InAppPurchaseResponse> Post(InAppPurchaseRequest receipt)
    {
        InAppPurchaseResponse result=new InAppPurchaseResponse();
        var check = await _gameDB.CheckDuplicateReceipt(receipt); //영수증 중복 검사
        if (check!=ErrorCode.None)          //중복됨
        { 
            result.Error = check;
            return result;
        }

       (var errorItemData, var data) =  _masterDataDB.GetInAppProduct(receipt.Code); //마스터데이터에서 해당 상품 데이터 불러오기
        if (errorItemData!=ErrorCode.None)
        {
            result.Error=errorItemData;
            return result;
        }
        List<UserItem> items = data.ConvertAll<UserItem>(i => i as UserItem) ;    //형변환
        var mailReust = await _gameDB.InsertMail(receipt.Email, items,MailType.InAppPurchase);  //메일 테이블에 메일 넣고 메일 아이템 테이블에 아이템 넣기
        if (mailReust != ErrorCode.None)
        { 
            result.Error = mailReust;
            return result;
        }
        result.Error = ErrorCode.None;
        return result;
    }
}

