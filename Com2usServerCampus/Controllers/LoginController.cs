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
        /*public LoginController()
        {
        }*/

        [HttpPost]
        public async Task<LoginAccountResponse> Post(LoginAccountRequest UserInfo)
        {
            LoginAccountResponse Result = new LoginAccountResponse();
            using (var db=DBManager.GetDBQuery()) //QueryFactory 인스턴스 불러와
            {
                var userCode = await db.Result.Query("account").Where("Email", UserInfo.Email).FirstOrDefaultAsync<DBUserInfo>();
                //아이디가 account 테이블에 있는지 확인(중복 확인)

                if (userCode != null)//테이블에 있으면 성공
                {
                    string HashedPassword = Security.Encrypt(UserInfo.Password);  //비번 암호화 
                    if (userCode.HashedPassword==HashedPassword) //비번 체크
                    {
                        //자신의 게임 데이터 로딩
                        using (var gamedb=DBManager.GetGameDBQuery())
                        {
                            Result.userInfo = await gamedb.Result.Query("gamedata").Where("AccountId", userCode.AccountId).FirstOrDefaultAsync<UserInfo>(); //기본 게임데이터 로드
                           // Result.itemList = new List<UserItem>();

                            
                            IEnumerable<UserItem> items = await gamedb.Result.Query("itemdata").Where("AccountId", userCode.AccountId).GetAsync<UserItem>(); //아이템 데이터 로드
                            Result.itemList =items.ToList();
                        }
                        //공지 불러옴
                        var noticeRedis = new RedisList<Notice>(DBManager.RedisConnection, "Notice", TimeSpan.FromDays(1)); //키가 Notice인 인덱스의 Value리스트
                       var noticeList=await noticeRedis.RangeAsync(0,-1);
                        Result.NoticeList=noticeList.ToList();          //공지리스트
                        

                        string tokenValue = Security.CreateAuthToken();     //토큰 생성
                        var idDefaultExpiry = TimeSpan.FromDays(1);         //유효기간
                        var redisId = new RedisString<string>(DBManager.RedisConnection, userCode.Email, idDefaultExpiry);
                        await redisId.SetAsync(tokenValue);

                       

                        Result.Authtoken = tokenValue;      //토큰 설정

                        Result.Error = ErrorCode.None;     //로그인 성공
                        return Result;
                    }
                    else            //테이블에 없으면 실패
                    {
                        Result.Error = ErrorCode.Login_Fail_Password;       //로그인 실패(패스워드 불일치)
                        return Result;
                    }
                }
                else                    //테이블에 없으면 실패
                {
                    Result.Error = ErrorCode.Login_Fail_Email;
                    return Result;
                }
            }
          
        }

    }
    public class LoginAccountRequest //유저가 서버에게 주는 아이디, 비번 데이터 클래스
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    [Serializable]
    public class LoginAccountResponse //유저가 서버에게 주는 아이디, 비번 데이터 클래스
    {
        public ErrorCode Error { get; set; }
        public UserInfo userInfo { get; set; }
        public List<UserItem> itemList { get; set; }
        public List<Notice> NoticeList { get; set; }
        public string Authtoken { get; set; }
    }
    class DBUserInfo
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
    }
    public class Notice
    {
        public string Title { get; set;}
        public string Content { get; set;}
        public string Date { get; set; }
        public string isRead { get; set; }

        public Notice(string title, string content, string date, string isRead)
        {
            Title = title;
            Content = content;
            Date = date;
            this.isRead = isRead;
        }
    }
}
