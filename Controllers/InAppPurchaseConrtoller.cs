using CloudStructures;
using CloudStructures.Structures;
using Com2usServerCampus.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;
using Com2usServerCampus.ModelReqRes;
using Com2usServerCampus.Services;

namespace APIServer.Controllers;
[Route("[controlelr]")]
[ApiController]
public class InAppPurchaseConrtoller : ControllerBase
{
    ILogger<InAppPurchaseConrtoller> _logger;
    IGameDB _gameDB;
    IMasterDataDB _masterDataDB;

    public InAppPurchaseConrtoller(ILogger<InAppPurchaseConrtoller> logger, IGameDB gameDB, IMasterDataDB masterDataDB)
    {
        _logger = logger;
        _gameDB = gameDB;
        _masterDataDB = masterDataDB;
    }

    [HttpPost]
}

