using CloudStructures;
using Com2usServerCampus;
using MySqlConnector;
using static Microsoft.Extensions.Logging.ILogger;
using System.IO;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Com2usServerCampus.Services;

public class DBConfig
{

    public string AccountDB { get; set; }       //계정정보 DB
    public string GameDB { get; set; }            //게임정보 DB
    public string DataDB { get; set; }            //마스터데이터 DB
    public string RedisDB { get; set; }   //레디스 DB
}
