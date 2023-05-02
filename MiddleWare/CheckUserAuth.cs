using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Com2usServerCampus.MiddleWare
{
    public class CheckUserAuth
    {
        ILogger Logger;
        readonly RequestDelegate _next;

        public CheckUserAuth( RequestDelegate next, ILogger logger)     //CheckUserAuth 클래스가 미들웨어로 등록될 때 생성자 호출 
        {
            Logger = logger;
            _next = next;
        }
        public async Task<ErrorCode> Invoke(HttpClient httpClient)
        {
            ErrorCode error= ErrorCode.None;

           // var token=new RedisString<string>(DBManager.RedisConnection,)

            return error;
        }
    }
}
