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
    public class CheckUserSessionMiddleWare
    {
        ILogger Logger;
        readonly RequestDelegate _next;

        public CheckUserSessionMiddleWare( RequestDelegate next, ILogger logger)
        {
            Logger = logger;
            _next = next;
        }
        public async Task<ErrorCode> Invoke()
        {
            ErrorCode error= ErrorCode.None;

           // var token=new RedisString<string>(DBManager.RedisConnection,)

            return error;
        }
    }
}
