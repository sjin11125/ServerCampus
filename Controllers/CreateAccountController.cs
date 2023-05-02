using CloudStructures;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using SqlKata.Execution;
using StackExchange.Redis;
using ZLogger;
namespace Com2usServerCampus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateAccountController : ControllerBase
    {
        ILogger Logger;
        public CreateAccountController(ILogger<CreateAccountController> logger)
        {
            Logger = logger;
        }

        [HttpPost]
        public async Task<DBUserInfo> AccountPost(CreateAccountRequest UserInfo)
        {


            var Result = new DBUserInfo();

            DBManager dBManager = new DBManager();

            var userCode = await dBManager.CheckUser(UserInfo.Email);
            //아이디가 account 테이블에 있는지 확인(중복 확인)

            if (userCode != null)//테이블에 있니?네
            {
                Result.Error = ErrorCode.CreateAccount_Fail_Dup; //에러로그(아이디 중복)

                return Result;
            }
            else                    //테이블에 없으면 계정 만들 수 있음
            {
                string HashedPassword = Security.Encrypt(UserInfo.Password);  //비번 암호화

                var AccountId = await dBManager.AddUser(UserInfo.Email, HashedPassword);        //계정추가

                UserInfo userInfo = new UserInfo(0, 1, 1);             //유저정보 초기화

                UserItem userItem = new UserItem(UserInfo.Email, 1, 0, 10);                  //기본 아이템 돈 10원

                // gamedata_db에 기본 데이터 생성(기본 게임 데이터, 기본 아이템 데이터)
                await dBManager.InsertItem(UserInfo.Email, userItem);
                await dBManager.InsertGameData(UserInfo.Email, userInfo);

                Result.Error = ErrorCode.None; //성공 로그
                                               // EventId eventId;
                                               //  eventId.Id=
                                               // Logger.LogInformation();
                Console.WriteLine(Result.ToString());

                //DB닫기
                // dBManager.CloseDB();
                //  dBManager.CloseGameDB();

                return Result;
            }



        }
    }


}
