using CloudStructures;
using CloudStructures.Structures;
using Com2usServerCampus.ModelReqRes;
using Com2usServerCampus.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using static Com2usServerCampus.LogManager;
using ZLogger;

namespace Com2usServerCampus.Controllers;

[Route("[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    ILogger<LoginController> _logger;
    IAccountDB _accountDB;
    IGameDB _gameDB;
    IRedisDB _redisDB;

    public LoginController(ILogger<LoginController> logger, IAccountDB accountDB, IGameDB gameDB,IRedisDB redisDB)
    {
        _logger = logger;
        _accountDB = accountDB;
        _gameDB = gameDB;
        _redisDB = redisDB;
    }

    [HttpPost]
    public async Task<LoginAccountResponse> Post(LoginAccountRequest UserInfo)
    {
        LoginAccountResponse Result = new LoginAccountResponse();


        (var errorUser,var userCode) = await _accountDB.CheckUser(UserInfo.Email,UserInfo.Password);
        //아이디가 account 테이블에 있는지 확인(중복 확인)

        if (errorUser != ErrorCode.None)     //성공하지 못했다면
        {
            Result.Error = errorUser;
            return Result;
        }

        //성공했다면

            //유저의 게임 데이터 로딩
       ( var errorUserGameData, var userGameData)=  await _gameDB.GetGameData(userCode.Email);
        if (errorUserGameData != ErrorCode.None)
        {
            Result.Error = errorUser;
            return Result;
        }
        Result.userInfo = userGameData;

        //유저의 아이템 데이터 로딩
      (var errorUserItemData  ,var userItemData) = await _gameDB.GetAllItems(userCode.Email);
        if (errorUserItemData != ErrorCode.None)
        {
            Result.Error = errorUser;
            return Result;
        }
        Result.itemList = userItemData;

        //공지 불러오기
        var notice = await _redisDB.LoadNotice();
        if (notice is not null)     //공지가 있다면
        {
            Result.NoticeList = notice;
        }
        else Result.NoticeList = null;

        //토큰 생성
        string tokenValue = Security.CreateAuthToken();     //토큰 생성
        var idDefaultExpiry = TimeSpan.FromDays(1);         //유효기간

        //레디스에 토큰 넣기
        var token = await _redisDB.SetUserToken(userCode.Email, tokenValue, userCode.AccountId);
        if (token!=ErrorCode.None)      //실패했다면
        {
            Result.Error =token;
            return Result;
        }

        Result.Authtoken= tokenValue;
        Result.Error = ErrorCode.None;



        _logger.ZLogInformationWithPayload(EventIdDictionary[EventType.Login], new { Email = UserInfo.Email}, $"Login Success");


        return Result;

    }

}
   

