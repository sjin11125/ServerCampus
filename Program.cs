using CloudStructures;
using Com2usServerCampus;
using MySqlConnector;
using static Microsoft.Extensions.Logging.ILogger;
using System.IO;
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Com2usServerCampus.Services;
using ZLogger;
using System.Text.Json;
using Com2usServerCampus.MiddleWare;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration = builder.Configuration;

builder.Services.AddLogging();

builder.Services.Configure<DBConfig>(builder.Configuration.GetSection("DBConnection"));

builder.Services.AddTransient<IAccountDB,AccountDB>();
//DI 컨테이너에 IAccountDb 인터페이스와 AccountDb 클래스를 등록해 IAccountDb를 요청할 때마다 AccountDb 클래스의 인스턴스를 생성해 반환

builder.Services.AddTransient<IGameDB, GameDB>();
//Transient: 각각의 요청, 서비스마다 새로운 객체를 제공하는 의존성 주입

builder.Services.AddTransient<IMasterDataDB, MasterDataDB>();

builder.Services.AddSingleton<IRedisDB, RedisDB>();
//Singleton: 서버를 시작할 때 부터 종료하는 순간까지의 객체를 가지는 의존성 주입


builder.Services.AddControllers();

SetLogger();        //로그 세팅

var app = builder.Build();


var loggerFactory=app.Services.GetRequiredService<ILoggerFactory>();
LogManager.SetLoggerFactory(loggerFactory,"Global");



app.UseMiddleware<CheckUserAuth>();     //인증 미들웨어 추가
//앱버전 체크 미들웨어 추가

app.UseRouting();
app.UseEndpoints(endpoints =>endpoints.MapControllers());


var redis = app.Services.GetRequiredService<IRedisDB>();            //RedisDB 객체 불러옴
redis.Init(builder.Configuration.GetSection("DBConnection")["RedisDB"]);        //레디스 연결 초기화
//레디스는 스레드 세이프 함으로 한번만 초기화한다

app.Run(configuration["ServerAddress"]);




void SetLogger()
{
    var logging = builder.Logging;
    //로깅 설정을 가져옴

    logging.ClearProviders();
    //이전에 등록되어 있던 로깅 프로바이더를 제거

    var path = configuration["logdir"];
    //appsettings.Development.json의 logdir 섹션의 스트링을 가져온다(파일을 저장할 경로)

    if (!Directory.Exists(path)) //경로가 존재하지 않는다
    {
        Directory.CreateDirectory(path);        //경로에 폴더를 만듬
    }

    logging.AddZLoggerRollingFile( //파일에 로그를 기록하는 Rolling File 로그 프로바이더를 추가함
        (time, x) => $"{path}{time.ToLocalTime():yyyy-MM-dd}_{x:000}.log",       //로그파일의 이름을 결정 (dt: DateTime, x: 인덱스)
        x => x.ToLocalTime().Date,  //로그 파일을 일 단위로 분리->날짜별로 로그파일을 생성함
        1024,  //파일 하나의 최대 크기
        options =>
        {     //로깅 포맷을 지정할 수 있음
            options.EnableStructuredLogging = true;   //로깅 정보를 구조화된 JSON 형태로 저장할 수 있음
            var time = JsonEncodedText.Encode("Timestamp");  //시간 형식을 정함(?)

            //DateTime.Now는 UTC+0 이고 한국은 UTC+9이므로 9시간을 더한 값을 출력한다.
            var timeValue = JsonEncodedText.Encode(DateTime.Now.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss"));

            options.StructuredLoggingFormatter = (writer, info) =>
            {
                writer.WriteString(time, timeValue);
                info.WriteToJsonWriter(writer);     //JSON으로 쓰기
            };
        }
        );

    logging.AddZLoggerConsole(options =>           //콘솔에 로그를 출력하는 콘솔 로그 프로바이더 추가
    {
        options.EnableStructuredLogging = true;
        var time = JsonEncodedText.Encode("EventTime");
        var timeValue = JsonEncodedText.Encode(DateTime.Now.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss"));

        options.StructuredLoggingFormatter = (writer, info) => {
            writer.WriteString(time,timeValue);
            info.WriteToJsonWriter(writer);
        };

    });
}