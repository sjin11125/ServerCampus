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
    public class CreateAccount:ControllerBase
    {
        ILogger Logger;
        public CreateAccount(ILogger<CreateAccount> logger)
        {
            Logger = logger;
        }

        [HttpPost]
        public async Task<CreateAccountResult> AccountPost(CreateAccountRequest UserInfo)
        {
            

            var Result=new CreateAccountResult();

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
                    await db.Result.Query("account").InsertAsync(new {          //account 테이블에 이메일, 비번 넣기
                        UserInfo.Email,
                        HashedPassword
                    });
                   // var radisId = new RedisString<string>(DBManager.RedisConnection,UserInfo.Email,TimeSpan.FromDays(1));//토큰 생성해서 레디스에 넣자(유효기간 1일)
                   // await radisId.SetAsync(UserInfo.Email);

                    Result.Success = SuccessCode.CreateAccount_Success; //성공 로그
                                                                        // EventId eventId;
                                                                        //  eventId.Id=
                                                                        // Logger.LogInformation();
                    Console.WriteLine(Result.ToString());
                    return Result;
                }
            }

            return Result;

        }
    }


    public class CreateAccountRequest //유저가 서버에게 주는 아이디, 비번 데이터 클래스
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class CreateAccountResponse //유저가 서버에게 주는 아이디, 비번 데이터 클래스
    {
        public string Email { get; set; }
        public string HashedPassword { get; set; }
    }
    public class CreateAccountResult
    {
        public ErrorCode Error { get; set; }
        public SuccessCode Success { get; set; }
    }
    
}
