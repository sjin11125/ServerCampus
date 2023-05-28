using CloudStructures;
using CloudStructures.Structures;
using Com2usServerCampus.Model;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Principal;
using ZLogger;
using static Com2usServerCampus.LogManager;

namespace Com2usServerCampus.Services;
public class RedisDB : IRedisDB
{
    ILogger<RedisDB> logger=GetLogger<RedisDB>();

    public RedisConnection redisConnection { get; set; }


    public void Init(string connectString)      //레디스 연결 초기화 (한번만 실행)
    {
        var config = new RedisConfig("basic", connectString);
        redisConnection= new RedisConnection(config);       //레디스 연결
        logger.ZLogDebug($"userDBAdress: {connectString}");
    }
  
    public async Task<(bool, AuthUser)> GetUserAsync(string email)        //레디스에서 사용자 정보 조회(email)
    {
        var uid = "UID_" + email;

        try
        {
            var redis = new RedisString<AuthUser>(redisConnection,uid,null);       //uid를 키로 설정
            var user =await redis.GetAsync();        //값 불러오기
            if (!user.HasValue)     //값이 없나?
            {
                logger.ZLogError(
                    $"RedisDB.UserStartCheckAsync: UID={uid},ErrorMEssage = Not Assigned User, RedisString get Error");
                return (false, null);
            }
            return (true, user.Value);        //값이 있으면 사용자 정보를 넘겨줌
        }
        catch 
        {
            logger.ZLogError($"UID:{uid}, ErrorMessage: ID dose Not Exist");    //레디스 키가 존재하지 않음 에러
            return (false, null);
        }
    }

  
    public async Task<bool> DelUserReqLockAsync(string key)     //락 풀기
    {
        if (string.IsNullOrEmpty(key))
            return false;
        try
        {
            var redis = new RedisString<AuthUser>(redisConnection,key,null);
            var redisResult= await redis.DeleteAsync();     //데이터 삭제
            return redisResult;
        }
        catch 
        {
            return false;
        }
    }
    public async Task<bool> SetUserReqLockAsync(string key)     //락 걸기
    {
        try
        {
            var redis=new RedisString<AuthUser>(redisConnection,key, NxKeyTimeSpan());
            if (await redis.SetAsync(new AuthUser { },NxKeyTimeSpan(),StackExchange.Redis.When.NotExists)==false)   //키가 존재하지 않을 경우 값 Set
            {
                return false;
            }
        }
        catch 
        {
            return false;
        }
        return true;
    }
    public async Task<List<Notice>> LoadNotice()               //공지 불러옴
    {
        var noticeRedis = new RedisList<Notice>(redisConnection, "Notice", TimeSpan.FromDays(1)); //키가 Notice인 인덱스의 Value리스트

        var noticeList = await noticeRedis.RangeAsync(0, -1);
        if (noticeList.Length != 0)        //공지가 있다면
            return noticeList.ToList();
        else return null;

        
    }
    public async Task<ErrorCode> SetUserToken(string email, string token, int accountId)            //레디스에 유저 토큰 넣기
    {
        var uid = "UID_" + email;

        var redisId = new RedisString<AuthUser>(redisConnection, uid, LoginTimeSpan());       //유효 기간 1일

        var userInfo = new AuthUser { AccountId = accountId, Email = email, AuthToken = token, State = UserState.Default };

        if (await redisId.SetAsync(userInfo, LoginTimeSpan()) == false) //실패햇다면
        {
            logger.ZLogError($"UID:{uid}, ErrorCode: {ErrorCode.SetUserTokenFail} UserId:{email} Token: {token} AccountId:{accountId}");    //레디스에 토큰 넣기 실패 에러
            return ErrorCode.SetUserTokenFail;
        }

        return ErrorCode.None;

    }
    public async Task<ErrorCode> CheckPlayGmae(string email, string token, int accountId)            //유저가 현재 게임하는 상태인지 아닌지 체크
    {
        var uid = "UID_" + email;

        var redisId = new RedisString<AuthUser>(redisConnection, uid, LoginTimeSpan());       //유효 기간 1일


        var tokenResult = await redisId.GetAsync();

        if (!tokenResult.HasValue) //실패햇다면
        {
            logger.ZLogError($"UID:{uid}, ErrorCode: {ErrorCode.GetUserTokenFail} UserId:{email} Token: {token} AccountId:{accountId}");    //레디스에 토큰 불러오기 실패 에러
            return ErrorCode.GetUserTokenFail;
        }

        if (tokenResult.Value.State != UserState.Game)          //게임하는 중이 아니라면
        {
            return ErrorCode.NotPlayGame;           
        }


        return ErrorCode.None;

    }
    
    public async Task<ErrorCode> UpdateUserToken(string email, string token, int accountId)            //레디스에 유저 토큰 업뎃
    {
        var uid = "UID_" + email;

        var redisId = new RedisString<AuthUser>(redisConnection, uid, StageTimeSpan());       //유효 기간 1시간

        var userInfo = new AuthUser { AccountId = accountId, Email = email, AuthToken = token, State = UserState.Game };

        if (await redisId.SetAsync(userInfo, LoginTimeSpan()) == false) //실패햇다면
        {
            logger.ZLogError($"UID:{uid}, ErrorCode: {ErrorCode.UpdateUserTokenFail} UserId:{email} Token: {token} AccountId:{accountId}");    //레디스에 토큰 넣기 실패 에러
            return ErrorCode.UpdateUserTokenFail;
        }

        return ErrorCode.None;

    }
    
 

    public async Task<(ErrorCode, int, int)> GetUserStageItem(string userId, int itemCode, int stageCode)            
        //레디스에 해당 유저의 현재 진행중인 스테이지의 특정 아이템 정보를 불러옴
    {
     

            var uid = "StageItem_" + stageCode + "_" + userId;
        try
        {
            var redisId = new RedisList<AcquireStageItem>(redisConnection, uid, StageItemTimeSpan());

            var stageItems = await redisId.RangeAsync(0, -1);  //모든 아이템 목록 불러오기

            int itemCount = 0;
            int index = 0;

            bool isExist = false;

            for (int i = 0; i < stageItems.Length; i++)       //해당 아이템 정보 찾기
            {
                if (stageItems[i].ItemCode == itemCode)
                {
                    itemCount = stageItems[i].ItemCount;
                    index = i;
                    isExist = true;
                    break;
                }
            }
            if (!isExist)       //해당 아이템이 존재하지 않음
            {
                return (ErrorCode.None, -1, 0);
            }
            else            //존재함
            {
                return (ErrorCode.None, index, itemCount);

            }

        }
        catch (Exception e)
        {
            logger.ZLogError(e, $"UID:{uid}, ErrorCode: {ErrorCode.GetUserStageItemFail} UserId:{userId} ItemCode:{itemCode} StageNum: {stageCode} ");    //레디스에 스테이지 아이템 넣기 실패 에러
            return (ErrorCode.GetUserStageItemFail, -1, -1);

        }

    }
    public async Task<ErrorCode> SetUserStageItem(string userId, int stageCode, int itemCode,int itemCount,int index)            
        //유저가 스테이지 아이템 파밍했을 때 레디스에 정보넣기
    {
        var uid =  "StageItem_" + stageCode + "_" + userId;

        var redisId = new RedisList<AcquireStageItem>(redisConnection, uid, StageItemTimeSpan()) ;

        if (index!=-1)          //레디스에 이미 같은 종류의 아이템이 있다면
        {
            try
            {
             await redisId.SetByIndexAsync(index,new AcquireStageItem { ItemCode=itemCode, Code=stageCode, ItemCount=itemCount}); //set

            }
            catch (Exception e)
            {
                logger.ZLogError(e,$"UID:{uid}, ErrorCode: {ErrorCode.SetStageItemFail} UserId:{userId} ItemCode:{itemCode} StageNum: {stageCode} ");    
                //레디스에 스테이지 아이템 넣기 실패 에러
                return ErrorCode.SetStageItemFail;
            }
          
            return ErrorCode.None;
        }
        else                //없다면
        {
            var stageItemPush = await redisId.RightPushAsync(new AcquireStageItem { ItemCode = itemCode, Code = stageCode,ItemCount=itemCount }, StageItemTimeSpan()); //푸쉬

            if (stageItemPush == -1)      //실패
            {
                logger.ZLogError($"UID:{uid}, ErrorCode: {ErrorCode.PushStageItemFail} UserId:{userId} ItemCode:{itemCode} StageNum: {stageCode} ");    
                //레디스에 스테이지 아이템 넣기 실패 에러
                return ErrorCode.PushStageItemFail;
            }
            return ErrorCode.None;
        }



    }
    public async Task<(ErrorCode, int, int)> GetUserStageNPC(string userId, int npcCode, int stageCode)     
        //레디스에 해당 유저의 현재 진행중인 스테이지의 특정 npc 정보를 불러옴
    {
        var uid = "StageNPC_" + stageCode + "_" + userId;
        try
        {

            var redisId = new RedisList<KillStageNPC>(redisConnection, uid, StageNPCTimeSpan());

            var npcInfos = await redisId.RangeAsync(0, -1); //레디스에서 이때까지 죽인 npc 정보들 불러와서 있으면 정보 업데이트



            int npcCount = 0;
            int index = 0;

            bool isExist = false;

            for (int i = 0; i < npcInfos.Length; i++)       //해당 npc 정보 찾기
            {
                if (npcInfos[i].NPCCode == npcCode)
                {
                    npcCount = npcInfos[i].Count;
                    index = i;
                    isExist = true;
                    break;
                }
            }
            if (!isExist)       //해당 npc가 존재하지 않음
            {
                return (ErrorCode.None, -1, 0);
            }
            else            //존재함
            {
                return (ErrorCode.None, index, npcCount);

            }

        }
        catch (Exception e)
        {
            logger.ZLogError(e, $"UID:{uid}, ErrorCode: {ErrorCode.GetUserStageNPCFail} UserId:{userId} NPCCode:{npcCode} StageNum: {stageCode} ");    
            //레디스에 스테이지 npc 불러오기 실패 에러
            return (ErrorCode.GetUserStageNPCFail, -1, -1);

        }
    }
    public async Task<(ErrorCode, List<AcquireStageItem>)> GetAllUserStageItem(string userId,  int stageCode)
    {
        var uid = "StageItem_" + stageCode + "_" + userId;

        var redisId = new RedisList<AcquireStageItem>(redisConnection, uid, StageItemTimeSpan());

        var stageItems = await redisId.RangeAsync(0, -1);  //모든 아이템 목록 불러오기

        if (stageItems.Length==0)
        {
            return (ErrorCode.None, null);
        }

        return (ErrorCode.None, stageItems.ToList());

    }
    public async Task<(ErrorCode, List<KillStageNPC>)> GetAllUserStageNPC(string userId,  int stageCode)
    {
        var uid = "StageNPC_" + stageCode + "_" + userId;

        var redisId = new RedisList<KillStageNPC>(redisConnection, uid, StageItemTimeSpan());

        var stageItems = await redisId.RangeAsync(0, -1);  //모든 아이템 목록 불러오기

        if (stageItems.Length==0)
        {
            return (ErrorCode.None, null);
        }

        return (ErrorCode.None, stageItems.ToList());

    }
    public async Task<ErrorCode> SetUserStageNPC(string userId, int npcCode, int stageCode, int npcCount, int index)
    {
        var uid = "StageNPC_" + stageCode + "_" + userId;

        var redisId = new RedisList<KillStageNPC>(redisConnection, uid, StageNPCTimeSpan());
        if (index != -1)          //레디스에 이미 같은 종류의 npc가 있다면
        {
            try
            {
                await redisId.SetByIndexAsync(index, new KillStageNPC { NPCCode = npcCode, StageCode = stageCode, Count = npcCount });

            }
            catch (Exception e)
            {
                logger.ZLogError(e, $"UID:{uid}, ErrorCode: {ErrorCode.SetStageItemFail} UserId:{userId} NpcCode:{npcCode} StageNum: {stageCode} ");   
                //레디스에 스테이지 npc 넣기 실패 에러
                return ErrorCode.SetStageItemFail;
            }

            return ErrorCode.None;
        }
        else                //없다면
        {
            var stageNpcPush = await redisId.RightPushAsync(new KillStageNPC { NPCCode = npcCode, StageCode = stageCode, Count = npcCount }, StageNPCTimeSpan()); //푸쉬

            if (stageNpcPush == -1)      //실패
            {
                logger.ZLogError($"UID:{uid}, ErrorCode: {ErrorCode.PushStageItemFail} UserId:{userId} NpcCode:{npcCode} StageNum: {stageCode} ");    
                //레디스에 스테이지 npc 넣기 실패 에러
                return ErrorCode.PushStageItemFail;
            }
            return ErrorCode.None;
        }
    }

    public async Task<ErrorCode> DeleteUserStageItemData(string userId, int stageCode)
    {
        var uid = "StageItem_" + stageCode + "_" + userId;

        var deleteKey = redisConnection.GetConnection().GetDatabase();


        if (deleteKey.KeyExists(uid))     //해당 키가 있나    
        {
            var isDelete = await deleteKey.KeyDeleteAsync(uid);//해당 키 제거


            if (!isDelete)     //키가 제거가 안됐다면
            {
                return ErrorCode.RemoveStageItemKeyFail;
            }
        }
        return ErrorCode.None;


    }
    public async Task<ErrorCode> DeleteUserStageNPCData(string userId,int stageCode)
    {
        var uid = "StageNPC_" + stageCode + "_" + userId;

        var deleteKey = redisConnection.GetConnection().GetDatabase();


        if (deleteKey.KeyExists(uid))       //해당 키가 있나
        {
            var isDelete = await deleteKey.KeyDeleteAsync(uid);//해당 키 제거


            if (!isDelete)     //키가 제거가 안됐다면
            {
                return ErrorCode.RemoveStageItemKeyFail;
            }
        }

        return ErrorCode.None;

    }



    public TimeSpan StageNPCTimeSpan()
    {
        return TimeSpan.FromSeconds(RedisKeyExpireTime.StageNPCExpireSecond);

    }
    public TimeSpan StageItemTimeSpan()
    {
        return TimeSpan.FromSeconds(RedisKeyExpireTime.StageItemExpireSecond);

    }
    public TimeSpan StageTimeSpan()
    {
        return TimeSpan.FromSeconds(RedisKeyExpireTime.StageExpireSecond);

    }
    public TimeSpan NxKeyTimeSpan()
    {
        return TimeSpan.FromSeconds( RedisKeyExpireTime.NxKeyExpireSecond);
    }
    public TimeSpan LoginTimeSpan()
    {
        return TimeSpan.FromSeconds( RedisKeyExpireTime.RedisKeyExpireSecond);
    }
}

