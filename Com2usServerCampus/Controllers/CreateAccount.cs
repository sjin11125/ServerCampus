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

            using (var db=DBManager.GetDBQuery())
            {
                var userCode=db.Result.Query("account")
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
        public ErrorCode Result { get; set; }
    }
    
}
