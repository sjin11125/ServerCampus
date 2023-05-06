using CloudStructures;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;
using  Com2usServerCampus.ModelReqRes;
using Com2usServerCampus.Services;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Com2usServerCampus.Controllers;
[ApiController]
[Route("[controller]")]
public class GetMailItemController : ControllerBase
{
    ILogger<ReadMailController> _logger;
    IGameDB _gameDB;
    IMasterDataDB _masterDataDB;

    public GetMailItemController(ILogger<ReadMailController> logger, IGameDB gameDB, IMasterDataDB masterDataDB)
    {
        _logger = logger;
        _gameDB = gameDB;
        _masterDataDB = masterDataDB;
    }

    [HttpPost]
    public async Task<GetMailItemResponse> MailItemPost(GetMailItemRequest mailInfo)
    {
        GetMailItemResponse getMailItemResponse = new GetMailItemResponse();

        var items = await _gameDB.GetMailItem(mailInfo.Email, mailInfo.Id);     //메일 아이템 불러오기

        if (items.Item1!=ErrorCode.None)        //실패라면
        {
            getMailItemResponse.Error = items.Item1;
            return getMailItemResponse;
        }
        //성공
        foreach (var item in items.Item2)
        {
            var itemData = await _masterDataDB.GetItemData(item.Code); //마스터데이터에서 아이템 데이터 찾아서 
            if (itemData.Item1!=ErrorCode.None)
            {
                getMailItemResponse.Error = itemData.Item1;
                return getMailItemResponse;
            }

            var result = await _gameDB.InsertItem(mailInfo.Email, new Model.UserItem {      //해당 계정에 아이템 넣기
            Eamil=mailInfo.Email,
            ItemCode=mailInfo.Id,
            EnhanceCount=0,
            ItemCount =item.Count,
            IsCount=itemData.Item2.isCount
            });

            if(result!=ErrorCode.None)
            {
                getMailItemResponse.Error = result;
                return getMailItemResponse;
            }
        }

        var updateResult = await _gameDB.UpdateMailItem(mailInfo.Email, mailInfo.Id);           //메일 테이블에 받았다고 업데이트
        if (updateResult != ErrorCode.None)
        {
            getMailItemResponse.Error = updateResult;
            return getMailItemResponse;
        }

        getMailItemResponse.Error = ErrorCode.None;

        return getMailItemResponse;
    }
}

