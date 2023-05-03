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

        string AuthToken;       //유저가 보낸 토큰 
        string email;           //유저가 보낸 이메일
        string userLockKey = "";

        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 4096, true))         //요청 바디의 데이터를 문자열로 가져옴
        {
            var bodyStr = await reader.ReadToEndAsync();
            if (await IsNullBodyData(context, bodyStr))       //바디 문자열이 유효한지 검사 유효하면 false, 유효하지 않으면 true
                return;

            var document = JsonDocument.Parse(bodyStr);  //JSON 문자열 파싱

            if (document != null) { }
        }

    }
    async Task<bool> IsNullBodyData(HttpContext context, string bodystr)       //바디 문자열이 유효한지 검사
    {
        if (!string.IsNullOrEmpty(bodystr))     //바디 문자열이 비어있지 않다
            return false;

        var errorJsonResponse = JsonSerializer.Serialize(new MiddlewareResponse             //에러코드 설정
        {
            errorCode = ErrorCode.InvalidRequestHttpBody
        });
        var bytes = Encoding.UTF8.GetBytes(errorJsonResponse);
        await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);           //Response Body에 에러코드 반환

        return true;
    }
    
    bool IsInvalidJsonFormat()
    {

    }

    public class MiddlewareResponse
    {
        public ErrorCode errorCode
        {
            get; set;
        }
    }
}

