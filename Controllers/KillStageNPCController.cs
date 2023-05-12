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
using System.Security.Cryptography;
using ZLogger;
namespace Com2usServerCampus.Controllers;
[ApiController]
[Route("[controller]")]

public class KillStageNPCController
{
    public ILogger<KillStageNPCController> _logger;
    public IRedisDB _redisDB;

    public KillStageNPCController(ILogger<KillStageNPCController> logger, IRedisDB redisDB)
    {
        _logger = logger;
        _redisDB = redisDB;
    }
    public async Task<KillStageNPCResponse> Post(KillStageNPCRequest npcInfo)
    {
        KillStageNPCResponse killStageNPCResponse = new KillStageNPCResponse();



        return killStageNPCResponse;
    }

}

