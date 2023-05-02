using CloudStructures;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;

namespace Com2usServerCampus.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        string CurrentAppVersion = "v1.00.0";
        string CurrentDataVersion = "v1.00.0";

        [HttpPost]
        public async Task<LoginAccountResponse> Post(LoginAccountRequest UserInfo)
        {
            LoginAccountResponse Result = new LoginAccountResponse();

            DBManager dBManager = new DBManager();
            
            var userCode = await dBManager.CheckUser(UserInfo.Email);
            //아이디가 account 테이블에 있는지 확인(중복 확인)

            if (userCode != null)//테이블에 있으면 성공
            {
                string HashedPassword = Security.Encrypt(UserInfo.Password);  //비번 암호화 
                if (userCode.HashedPassword != HashedPassword) //비번 체크
                {
                    Result.Error = ErrorCode.Login_Fail_Password;       //로그인 실패(패스워드 불일치)


                    return Result;
                }


                //자신의 게임 데이터 로딩

                Result.userInfo = await dBManager.GetGameData(userCode.Email); //기본 게임데이터 로드

                IEnumerable<UserItem> items = await dBManager.GetItems(userCode.Email); //아이템 데이터 로드
                Result.itemList = items.ToList();

                //공지 불러옴
                var noticeRedis = new RedisList<Notice>(DBManager.RedisConnection, "Notice", TimeSpan.FromDays(1)); //키가 Notice인 인덱스의 Value리스트
                var noticeList = await noticeRedis.RangeAsync(0, -1);
                Result.NoticeList = noticeList.ToList();          //공지리스트


                string tokenValue = Security.CreateAuthToken();     //토큰 생성
                var idDefaultExpiry = TimeSpan.FromDays(1);         //유효기간
                var redisId = new RedisString<string>(DBManager.RedisConnection, userCode.Email, idDefaultExpiry);
                await redisId.SetAsync(tokenValue);



                Result.Authtoken = tokenValue;      //토큰 설정

                Result.Error = ErrorCode.None;     //로그인 성공

                return Result;

            }
            else                    //테이블에 없으면 실패
            {
                Result.Error = ErrorCode.Login_Fail_Email;


                return Result;
            }


        }

    }
   
}
