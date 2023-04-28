using CloudStructures;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SqlKata.Execution;
using StackExchange.Redis;
using ZLogger;

namespace Com2usServerCampus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateAccountController:ControllerBase
    {
        ILogger Logger;
        public CreateAccountController(ILogger<CreateAccountController> logger)
        {
            Logger = logger;
        }

        [HttpPost]
        public async Task<CreateAccountResponse> AccountPost(CreateAccountRequest UserInfo)
        {
            

            var Result=new CreateAccountResponse();

            using (var db=DBManager.GetDBQuery())       //accouunt_db 연결
            {
                var userCode = await db.Result.Query("account").Where("Email", UserInfo.Email).FirstOrDefaultAsync<CreateAccountResponse>();
                //아이디가 account 테이블에 있는지 확인(중복 확인)
               
                if (userCode != null)//테이블에 있니?네
                {
                    Result.Error = ErrorCode.CreateAccount_Fail_Dup; //에러로그(아이디 중복)
                    return Result;      //빠꾸
                }
                else                    //테이블에 없으면 계정 만들 수 있음
                {
                    string HashedPassword = Security.Encrypt(UserInfo.Password);  //비번 암호화

                   var AccountId = await db.Result.Query("account").InsertGetIdAsync<Int64>(new {          //account 테이블에 이메일, 비번 넣기
                        UserInfo.Email,
                        HashedPassword
                    });
                    UserInfo userInfo = new UserInfo(0,1,1);             //유저정보 초기화

                    UserItem userItem;                  //기본 아이템 돈 10원
                    userItem.ItemCode = 1;
                    userItem.EnhanceCount = 0;
                    userItem.Count = 10;

                    using (var gamedb=DBManager.GetGameDBQuery()) // gamedata_db에 기본 데이터 생성(기본 게임 데이터, 기본 아이템 데이터)
                    {
                        await gamedb.Result.Query("gamedata").InsertAsync(new {
                            AccountId,
                            userInfo.Exp,
                            userInfo.Attack,
                            userInfo.Defence,
                        });
                        await gamedb.Result.Query("itemdata").InsertAsync(new
                        {
                            AccountId,
                            userItem.ItemCode,
                            userItem.EnhanceCount,
                            userItem.Count
                        });


                    }
                        Result.Success = SuccessCode.CreateAccount_Success; //성공 로그
                                                                        // EventId eventId;
                                                                        //  eventId.Id=
                                                                        // Logger.LogInformation();
                    Console.WriteLine(Result.ToString());
                    return Result;
                }
            }


        }
    }


    public class CreateAccountRequest //유저가 서버에게 주는 아이디, 비번 데이터 클래스
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class CreateAccountResponse //서버가 유저에게 주는 응답 클래스
    {
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public ErrorCode Error { get; set; }
        public SuccessCode Success { get; set; }
    }
    
}
