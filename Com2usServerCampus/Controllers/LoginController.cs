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
                            Result.userInfo = await db.Result.Query("gamedata").Where("AccountId", userCode.AccountId).FirstOrDefaultAsync<UserInfo>(); //기본 게임데이터 로드
                            Result.ItemList = new List<UserItem>();

                            IEnumerable<UserItem> items = await db.Result.Query("itemdata").Where("AccountId", userCode.AccountId).GetAsync<UserItem>(); //아이템 데이터 로드
                            Result.ItemList =items.ToList();
                        }
                            Result.Error= ErrorCode.None;     //로그인 성공
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
    public class LoginAccountResponse //유저가 서버에게 주는 아이디, 비번 데이터 클래스
    {
        public ErrorCode Error { get; set; }
        public UserInfo userInfo { get; set; }
        public List<UserItem> ItemList { get; set; }
        public string Authtoken { get; set; }
    }
    class DBUserInfo
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }
    }
}
