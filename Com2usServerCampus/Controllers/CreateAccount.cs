using Microsoft.AspNetCore.Mvc;

namespace Com2usServerCampus.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreateAccount:ControllerBase
    {

        [HttpPost]
        public async Task<CreateAccountResponse> AccountPost(CreateAccountRequest UserInfo)
        {
            var Result=new CreateAccountResponse();

            using (var db=DBManager.GetDBQuery())       //accouunt_db 연결
            {
                var userCode = db.Result.Query("account").Where("Email", UserInfo.Email).Select();
                //아이디가 account 테이블에 있는지 확인(중복 확인)

                if (userCode != null)//테이블에 있니?네
                {
                    Result.Error = ErrorCode.CreateAccount_Fail_Dup; //에러로그(아이디 중복)
                    return Result;      //빠꾸
                }
                else                    //테이블에 없으면 계정 만들 수 있음
                {
                    //비번 암호화
                    //account 테이블에 이메일, 비번 넣기
                    //토큰 생성해서 레디스에 넣자
                    //성공 로그
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
    public class CreateAccountResponse
    {
        public ErrorCode Error { get; set; }
        public SuccessCode Success { get; set; }
    }
    
}
