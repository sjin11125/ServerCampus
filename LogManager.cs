using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Com2usServerCampus;

public static class LogManager
{
    public enum EventType       //이벤트 타입
    {
        CreateAccount=101,

        Login=201,
        LoginAddRedis=202,

        LoadMail=301,
        ReadMail=302,
        GetMailItem=303,
        DeleteMail=304,


        GetAttendance=401,
        SetAttendance=402,

        InAppPurchase = 501,


        Enhance = 601,


        GetUserStage = 701,
        StageSelect = 702,
        AcquireStageItem = 703,
        KillStageNPC = 704,
        EndStage= 705,

    }
    static ILoggerFactory LoggerFactory;
    public static ILogger Logger {  get; private set; }

    public static Dictionary<EventType,EventId> EventIdDictionary= new() {          //이벤트 ID 딕셔너리에 추가
    {EventType.CreateAccount,new EventId((int)EventType.CreateAccount,"CreateAccount")},            //이벤트 ID 등록

    {EventType.Login,new EventId((int)EventType.Login,"Login")},
    {EventType.LoginAddRedis,new EventId((int)EventType.LoginAddRedis,"LoginAddRedis")},

    {EventType.LoadMail,new EventId((int)EventType.LoadMail,"LoadMail")},
    {EventType.ReadMail,new EventId((int)EventType.ReadMail,"ReadMail")},
    {EventType.GetMailItem,new EventId((int)EventType.GetMailItem,"GetMailItem")},
    {EventType.DeleteMail,new EventId((int)EventType.DeleteMail,"DeleteMail")},



    {EventType.GetAttendance,new EventId((int)EventType.GetAttendance,"GetAttendance")},
    {EventType.SetAttendance,new EventId((int)EventType.SetAttendance,"SetAttendance")},



    {EventType.InAppPurchase,new EventId((int)EventType.InAppPurchase,"InAppPurchase")},


    {EventType.Enhance,new EventId((int)EventType.Enhance,"Enhance")},

    {EventType.GetUserStage,new EventId((int)EventType.GetUserStage,"GetUserStage")},
    {EventType.StageSelect,new EventId((int)EventType.StageSelect,"StageSelect")},
    {EventType.AcquireStageItem,new EventId((int)EventType.AcquireStageItem,"AcquireStageItem")},
    {EventType.KillStageNPC,new EventId((int)EventType.KillStageNPC,"KillStageNPC")},
    {EventType.EndStage,new EventId((int)EventType.EndStage,"EndStage")},
    };

    public static void SetLoggerFactory(ILoggerFactory loggerFactory, string categoryName)
    {
       LoggerFactory = loggerFactory ;
        Logger = loggerFactory.CreateLogger(categoryName);
    }

    public static ILogger<T> GetLogger<T>() where T : class
    {
        return LoggerFactory.CreateLogger<T>();
    }
}

