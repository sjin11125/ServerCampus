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
    public async Task<bool> SetUserReqLockAsync(string key)     //락 걸기
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
    public async Task<List<Notice>> LoadNotice()               //공지 불러옴
    {
        var noticeRedis = new RedisList<Notice>(RedisConnection, "Notice", TimeSpan.FromDays(1)); //키가 Notice인 인덱스의 Value리스트

        var noticeList = await noticeRedis.RangeAsync(0, -1);
        if (noticeList.Length != 0)        //공지가 있다면
            return noticeList.ToList();
        else return null;

        
    }
    public async Task<ErrorCode> SetUserToken(string email, string token, int accountId)            //레디스에 유저 토큰 넣기
    {
        var uid = "UID_" + email;
        var redisId = new RedisString<AuthUser>(RedisConnection, uid, LoginTimeSpan());       //유효 기간 1일
        var userInfo = new AuthUser { AccountId = accountId, Email = email, AuthToken = token, State = "Default" };

        if (await redisId.SetAsync(userInfo, LoginTimeSpan()) == false) //실패햇다면
        {
            logger.ZLogError($"UID:{uid}, ErrorCode: {ErrorCode.SetUserTokenFail} Email:{email} Token: {token} AccountId:{accountId}");    //레디스에 토큰 넣기 실패 에러
            return ErrorCode.SetUserTokenFail;
        }

        return ErrorCode.None;

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

