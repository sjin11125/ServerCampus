using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Com2usServerCampus.Model;
using Com2usServerCampus.Services;
using Microsoft.AspNetCore.Http;
using static Com2usServerCampus.MiddleWare.GetMasterData;

namespace Com2usServerCampus.MiddleWare;
    public  class GetMasterData
    {
        readonly RequestDelegate _next;

    public GetMasterData(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var pathString =context.Request.Path.Value;

        context.Request.EnableBuffering();  //여러번 읽을 수 있게

        string AppVersion = "";         //유저가 보낸 앱 버전
        string MasterDataVersion = "";        //유저가 보낸 마스터 데이터 버전

        using (var reader =new StreamReader(context.Request.Body,Encoding.UTF8,true,4096,true))
        {
            var bodyStr=await reader.ReadToEndAsync();
            if (await IsNullBodyData(context, bodyStr))
                return;

            var document=JsonDocument.Parse(bodyStr);


        }
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

    /*async Task<(bool, string,string)> IsInvalidJsonFormat(HttpContext context, JsonDocument document, string appVersion,string masterdataVersion)
    {
        try
        {

        }
        catch (Exception)
        {

            throw;
        }
    }*/
}

