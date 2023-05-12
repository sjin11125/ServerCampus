using CloudStructures;
using CloudStructures.Structures;
using Com2usServerCampus.Model;
using Com2usServerCampus.ModelReqRes;
using Com2usServerCampus.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using SqlKata.Execution;
using StackExchange.Redis;
using System.Net.Mail;
using System.Security.Cryptography;
using ZLogger;
namespace Com2usServerCampus.Controllers;
[ApiController]
[Route("[controller]")]
public class StageController : ControllerBase
{
    readonly ILogger<StageController> _logger;
    readonly IGameDB _gameDB;

    public StageController(ILogger<StageController> logger, IGameDB gameDB)
    {
        _logger = logger;
        _gameDB = gameDB;
    }
    [HttpPost]
    public async Task<StageResponse> StagePost(StageRequest stageInfo)
    {
        StageResponse stageResonse = new StageResponse();

        (var stageError, var stage) = await _gameDB.GetUserStageInfo(stageInfo.UserId);       //유저가 클리어한 스테이지 불러옴
        if (stageError != ErrorCode.None)
        {
            stageResonse.Error= stageError;
            return stageResonse;
        }

        stageResonse.Error=ErrorCode.None;
        stageResonse.StageId = stage;


        return stageResonse;



    }

}

