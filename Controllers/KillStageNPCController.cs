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

public class KillStageNPCController:ControllerBase
{
    readonly ILogger<KillStageNPCController> _logger;
    readonly IRedisDB _redisDB;
    readonly IMasterDataDB _masterDataDB;

    public KillStageNPCController(ILogger<KillStageNPCController> logger, IRedisDB redisDB, IMasterDataDB masterDataDB)
    {
        _logger = logger;
        _redisDB = redisDB;
        _masterDataDB = masterDataDB;
    }
    public async Task<KillStageNPCResponse> Post(KillStageNPCRequest npcInfo)
    {
        KillStageNPCResponse killStageNPCResponse = new KillStageNPCResponse();



        //해당 npc가 해당 스테이지 npc인지 확인 (마스터데이터)
        (var dataError, var stageNPCData) = _masterDataDB.GetStageNPC(npcInfo.StageCode);

        if(dataError!=ErrorCode.None)
        {
            killStageNPCResponse.Error= dataError;
            return killStageNPCResponse;
        }




        //레디스에 해당 NPC 정보를 불러옴
        (var getUserStageNpcError, var npcIndex, var npcCount) =await _redisDB.GetUserStageNPC(npcInfo.UserId,npcInfo.NPCCode,npcInfo.StageCode);
        if (getUserStageNpcError!=ErrorCode.None)
        {
            killStageNPCResponse.Error = getUserStageNpcError;
            return killStageNPCResponse;
        }


        StageNPC NpcData = stageNPCData.Find(x => x.NPCCode == npcInfo.NPCCode);         //마스터데이터에서 해당 npc 데이터 찾기


        //레디스에 있는 해당 NPC 처치 수 + 1(이번에 새로 처치한 수) 가 마스터 데이터와 맞는지 확인
        if (NpcData.Count < npcCount + 1 )
        {
            killStageNPCResponse.Error = ErrorCode.NotMatchStageNPCData;
            return killStageNPCResponse;
        }
        //검증 끝


        var setUserStageNpc = await _redisDB.SetUserStageNPC(npcInfo.UserId,npcInfo.NPCCode, npcInfo.StageCode,( npcCount+1), npcIndex); //레디스에 npc 정보 저장
        if (setUserStageNpc!=ErrorCode.None)
        {
            killStageNPCResponse.Error= setUserStageNpc;
            return killStageNPCResponse;
        }
        
        
        
        return killStageNPCResponse;
    }

}

