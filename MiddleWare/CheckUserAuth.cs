using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Com2usServerCampus.Model;
using Com2usServerCampus.Services;
using Microsoft.AspNetCore.Http;

namespace Com2usServerCampus.MiddleWare;
public class CheckUserAuth
{
    readonly RequestDelegate _next;
    readonly IRedisDB _redisDB;

    public CheckUserAuth(RequestDelegate next, IRedisDB redisDB)  //CheckUserAuth 클래스가 미들웨어로 등록될 때 생성자 호출 
    {
        _redisDB = redisDB;
        _next = next;
    }
    public async Task Invoke(HttpContext context)
    {
        var pathString = context.Request.Path.Value;  //요청된 URL
        if (string.Compare(pathString, "/Login", StringComparison.OrdinalIgnoreCase) == 0 ||//대소문자 구분하면서 비교
            string.Compare(pathString, "/CreateAccount", StringComparison.OrdinalIgnoreCase) == 0)
        {
            //요청 URL이 /Login 또는 /CreateAccount인 경우 다음 단계 미들웨어를 호출
            await _next(context);
            return;

        }

        context.Request.EnableBuffering();      //요청 바디의 데이터를 여러 번 읽을 수 있게 함

        string AuthToken = "";       //유저가 보낸 토큰 
        string email = "";           //유저가 보낸 이메일
        string userLockKey = "";

        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 4096, true))         //요청 바디의 데이터를 문자열로 가져옴
        {
            var bodyStr = await reader.ReadToEndAsync();
            if (await IsNullBodyData(context, bodyStr))       //바디 문자열이 유효한지 검사 유효하면 false, 유효하지 않으면 true
                return;

            var document = JsonDocument.Parse(bodyStr);  //JSON 문자열 파싱

            var result = await IsInvalidJsonFormat(context, document, email, AuthToken);

            if (result.Item1)//사용자가 보낸 요청에서 email과 토큰 정보가 있는지 확인
                return;
            email = result.Item2;
            AuthToken = result.Item3;

            var (isOK, userInfo) = await _redisDB.GetUserAsync(email);//레디스에서 사용자 정보 조회

            if (!isOK)     //조회 안되면 리턴
                return;

            if (await IsInvalidUserAuthToken(context, userInfo, AuthToken)) //유저가 보낸 토큰과 레디스에 있던 토큰을 비교해 맞지 않다면
                return; //리턴

            userLockKey = "ULock_" + email;
            if (await SetLock(context, userLockKey))        //락이 걸려있는지 확인 걸려있으면
            {
                return;
            }
            context.Items[nameof(AuthUser)]=userInfo;
        }
        context.Request.Body.Position = 0;

        await _next(context);

        // 트랜잭션 해제(Redis 동기화 해제) 락 해제
        await _redisDB.DelUserReqLockAsync(userLockKey);


    }
    async Task<bool> IsNullBodyData(HttpContext context, string bodystr)       //바디 문자열이 유효한지 검사
    {
        if (!string.IsNullOrEmpty(bodystr))     //바디 문자열이 비어있지 않다
            return false;

        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse             //에러코드 설정
        {
            errorCode = ErrorCode.WrongdRequestHttpBody
        });
        var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);           //Response Body에 에러코드 반환

        return true;
    }
    public async Task<bool> IsInvalidUserAuthToken(HttpContext context, AuthUser userInfo,string authToken)
    {
        if(string.CompareOrdinal(userInfo.AuthToken,authToken)==0)     //두값이 같다면
            return false;

        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
        {
            errorCode = ErrorCode.InvalidAuthToken
        });

        var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
        await context.Response.Body.WriteAsync(bytes,0,bytes.Length);

        return true;
    }
    public async Task<bool> SetLock(HttpContext context, string AuthToken)
    {
        if (await _redisDB.SetUserReqLockAsync(AuthToken))      //락이 설정되어있지 않으면
            return false;

        //설정되어있다면

        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse
        {
            errorCode = ErrorCode.AuthTokenFailSetNx
        });
        var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);

        return true;
    }
   async Task<(bool,string,string)> IsInvalidJsonFormat(HttpContext context,JsonDocument document, string email, string authToken)//사용자가 보낸 요청에서 email과 토큰 정보가 있는지 확인
    {
        try
        {
            email = document.RootElement.GetProperty("Email").GetString();
            authToken = document.RootElement.GetProperty("AuthToken").GetString();

            return (false,email,authToken);
        }
        catch 
        {
            email = "";authToken = "";
            var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse { 
            
                errorCode = ErrorCode.WrongAuthTokenOrEmail
            });
            var bytes=Encoding.UTF8.GetBytes(errorJsonResponse);
           await context.Response.Body.WriteAsync(bytes,0,bytes.Length);
            return (true,null,null);
        }
    }

    public class MiddlewareResponse
    {
        public ErrorCode errorCode
        {
            get; set;
        }
    }
}

