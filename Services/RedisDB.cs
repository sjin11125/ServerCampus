using CloudStructures;
using CloudStructures.Structures;
using Com2usServerCampus.Model;
using Microsoft.Extensions.Options;
using ZLogger;
using static Com2usServerCampus.LogManager;

namespace Com2usServerCampus.Services;
public class RedisDB : IRedisDB
{
    ILogger<RedisDB> logger=GetLogger<RedisDB>();

    public RedisConnection RedisConnection { get; set; }


    public void Init(string connectString)      //레디스 연결 초기화 (한번만 실행)
    {
        var config = new RedisConfig("basic", connectString);
        RedisConnection= new RedisConnection(config);       //레디스 연결
        logger.ZLogDebug($"userDBAdress: {connectString}");
    }

    public async Task<(bool, AuthUser)> GetUserAsync(string email)        //레디스에서 사용자 정보 조회(email)
    {
        var uid = "UID_" + email;

        try
        {
            var redis = new RedisString<AuthUser>(RedisConnection,uid,null);       //uid를 키로 설정
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
            var redis = new RedisString<AuthUser>(RedisConnection,key,null);
            var redisResult= await redis.DeleteAsync();     //데이터 삭제
            return redisResult;
        }
        catch 
        {
            return false;
        }
    }
    public async Task<bool> SetUserReqLockAsync(string key)
    {
        try
        {
            var redis=new RedisString<AuthUser>(RedisConnection,key, NxKeyTimeSpan());
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

    public TimeSpan NxKeyTimeSpan()
    {
        return TimeSpan.FromSeconds( RedisKeyExpireTime.NxKeyExpireSecond);
    }
}

