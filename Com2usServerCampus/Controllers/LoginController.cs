﻿using Microsoft.AspNetCore.Http;
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
        public async Task<LoginAccountResult> Post(LoginAccountRequest UserInfo)
        {
            LoginAccountResult Result = new LoginAccountResult();
            using (var db=DBManager.GetDBQuery()) //QueryFactory 인스턴스 불러와
            {
                var userCode = await db.Result.Query("account").Where("Email", UserInfo.Email).FirstOrDefaultAsync<LoginAccountResponse>();
                //아이디가 account 테이블에 있는지 확인(중복 확인)

                if (userCode != null)//테이블에 있으면 성공
                {
                    string HashedPassword = Security.Encrypt(UserInfo.Password);  //비번 암호화 
                    if (userCode.HashedPassword==HashedPassword) //비번 체크
                    {
                        Result.Success = SuccessCode.Login_Success;     //로그인 성공
                        return Result;
                    }
                    else
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
            return Result;
        }

    }
    public class LoginAccountRequest //유저가 서버에게 주는 아이디, 비번 데이터 클래스
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginAccountResponse //유저가 서버에게 주는 아이디, 비번 데이터 클래스
    {
        public string Email { get; set; }
        public string HashedPassword { get; set; }
    }
    public class LoginAccountResult
    {
        public ErrorCode Error { get; set; }
        public SuccessCode Success { get; set; }
    }
}