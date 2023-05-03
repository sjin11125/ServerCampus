using CloudStructures;
using Microsoft.Extensions.Options;
using ZLogger;


namespace Com2usServerCampus.Services;
public class RedisDB : IRedisDB
{
    ILogger<RedisDB> logger;

    public RedisConnection RedisConnection { get; set; }
 

    public void Init(string connectString)      //레디스 연결 초기화 (한번만 실행)
    {
        var config = new RedisConfig("basic", connectString);
        RedisConnection= new RedisConnection(config);       //레디스 연결
    }
    
}

