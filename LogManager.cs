using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Com2usServerCampus;

public static class LogManager
{
    public enum EventType       //이벤트 타입
    {
        CreateAccount=101,

        Login=201,
        LoginAAddRedis=202,

        LoadMail=301,
        ReadMail=302,
        GetMailItem=303,
        DeleteMail=304,
    }
    static ILoggerFactory LoggerFactory;
    public static ILogger Logger {  get; private set; }

    public static Dictionary<EventType,EventId> EventIdDictionary= new() {          //이벤트 ID 딕셔너리에 추가
    {EventType.CreateAccount,new EventId((int)EventType.CreateAccount,"CreateAccount")},            //이벤트 ID 등록

    {EventType.Login,new EventId((int)EventType.CreateAccount,"Login")},
    {EventType.LoginAAddRedis,new EventId((int)EventType.CreateAccount,"LoginAAddRedis")},

    {EventType.LoadMail,new EventId((int)EventType.CreateAccount,"LoadMail")},
    {EventType.ReadMail,new EventId((int)EventType.CreateAccount,"ReadMail")},
    {EventType.GetMailItem,new EventId((int)EventType.CreateAccount,"GetMailItem")},
    {EventType.DeleteMail,new EventId((int)EventType.CreateAccount,"DeleteMail")},
    
    };

    public static void SetLoggerFactory(ILoggerFactory loggerFactory, string categoryName)
    {
        loggerFactory=LoggerFactory;
        Logger = loggerFactory.CreateLogger(categoryName);
    }

    public static ILogger<T> GetLogger<T>() where T : class
    {
        return LoggerFactory.CreateLogger<T>();
    }
}

