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

[ApiController]
[Route("[controller]")]
public class CreateAccountController : ControllerBase
{
    readonly IAccountDB _accountDB;
    readonly IGameDB _gameDB;
    readonly ILogger _logger;
    public CreateAccountController(ILogger<CreateAccountController> logger, IAccountDB accountDB, IGameDB gameDB)
    {
        _logger = logger;
        _accountDB = accountDB;
        _gameDB = gameDB;
    }

    [HttpPost]
    public async Task<CreateAccountResponse> AccountPost(CreateAccountRequest UserInfo)
    {


        var Result = new CreateAccountResponse();

        var AccountErrorCode = await _accountDB.AddUser(UserInfo.Email, UserInfo.Password);        //계정추가

        if (AccountErrorCode != ErrorCode.None)      //실패라면
        {
            Result.Error = AccountErrorCode;
            return Result;
        }

        // gamedata_db에 기본 데이터 생성(기본 게임 데이터, 기본 아이템 데이터)
        UserInfo userInfo = new UserInfo(0, 1, 1);             //유저정보 초기화
       var UserInfoErrorCode= await _gameDB.InsertGameData(UserInfo.Email, userInfo);
        
        if (UserInfoErrorCode != ErrorCode.None)
        {
            Result.Error = UserInfoErrorCode;
            return Result;
        }

        UserItem userItem = new UserItem(UserInfo.Email, 1, 0, 10,true);                  //기본 아이템 돈 10원

        var UserItemErrorCode = await _gameDB.InsertItem(UserInfo.Email, userItem);
        if (UserItemErrorCode != ErrorCode.None)
        {
            Result.Error = UserItemErrorCode;
            return Result;
        }



        _logger.ZLogInformationWithPayload(EventIdDictionary[EventType.CreateAccount],new { Email=UserInfo.Email},$"CreateAccount Success");

        return Result;
    }



}
    



