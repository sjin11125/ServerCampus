﻿using CloudStructures;
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
using static Com2usServerCampus.LogManager;

namespace Com2usServerCampus.Controllers;
[ApiController]
[Route("[controller]")]

public class KillStageNPCController : ControllerBase
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


        var authUser = (AuthUser)HttpContext.Items[nameof(AuthUser)]!;

        var isGame = await _redisDB.CheckPlayGmae(authUser.Email, authUser.AuthToken,(int) authUser.AccountId); //게임중인지 확인
        if (isGame!=ErrorCode.None)         //아니라면 에러
        {
            killStageNPCResponse.Error = isGame;
            return killStageNPCResponse;
        }


        //해당 npc가 해당 스테이지 npc인지 확인 (마스터데이터)
        (var dataError, var stageNPCData) = _masterDataDB.GetStageNPC(npcInfo.StageCode);

        if (dataError != ErrorCode.None)
        {
            killStageNPCResponse.Error = dataError;
            return killStageNPCResponse;
        }




        //레디스에 해당 NPC 정보를 불러옴
        (var getUserStageNpcError, var npcIndex, var npcCount) = await _redisDB.GetUserStageNPC(npcInfo.UserId, npcInfo.NPCCode, npcInfo.StageCode);
        if (getUserStageNpcError != ErrorCode.None)
        {
            killStageNPCResponse.Error = getUserStageNpcError;
            return killStageNPCResponse;
        }

        try
        {

            StageNPC NpcData = stageNPCData.Find(x => x.NPCCode == npcInfo.NPCCode);         //마스터데이터에서 해당 npc 데이터 찾기

            if (NpcData == null)
            {
                killStageNPCResponse.Error = ErrorCode.NotMatchStageNPCData;
                return killStageNPCResponse;
            }
                npcCount += 1;
                //레디스에 있는 해당 NPC 처치 수 + 1(이번에 새로 처치한 수) 가 마스터 데이터와 맞는지 확인
                if (NpcData.Count < npcCount)
                {
                    killStageNPCResponse.Error = ErrorCode.NotMatchStageNPCData;
                    return killStageNPCResponse;
                }

            
        }
        catch (Exception e)
        {
            _logger.ZLogError(e, $"  UserId:{npcInfo.UserId} ");
            killStageNPCResponse.Error= ErrorCode.NotMatchStageNPCData;
            return killStageNPCResponse;
        }

        //검증 끝


        var setUserStageNpc = await _redisDB.SetUserStageNPC(npcInfo.UserId, npcInfo.NPCCode, npcInfo.StageCode, npcCount, npcIndex); //레디스에 npc 정보 저장
        if (setUserStageNpc != ErrorCode.None)
        {
            killStageNPCResponse.Error = setUserStageNpc;
            return killStageNPCResponse;
        }


        _logger.ZLogInformationWithPayload(EventIdDictionary[EventType.KillStageNPC], new { UserId = npcInfo.UserId }, $"KillStageNPC Success");

        return killStageNPCResponse;
    }

}

