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

var builder = WebApplication.CreateBuilder(args);       



builder.Services.AddLogging();

builder.Services.AddTransient<IAccountDB,AccountDB>();
//DI 컨테이너에 IAccountDb 인터페이스와 AccountDb 클래스를 등록해 IAccountDb를 요청할 때마다 AccountDb 클래스의 인스턴스를 생성해 반환

builder.Services.AddTransient<IGameDB, GameDB>();
//Transient: 각각의 요청, 서비스마다 새로운 객체를 제공하는 의존성 주입

builder.Services.AddSingleton<IRedisDB, RedisDB>();
//Singleton: 서버를 시작할 때 부터 종료하는 순간까지의 객체를 가지는 의존성 주입
//레디스는 스레드 세이프 함으로 한번만 호출해도 된다

builder.Services.AddControllers();



var app = builder.Build();


var configuration = app.Configuration;

//인증 미들웨어 추가
//앱버전 체크 미들웨어 추가
app.UseRouting();
app.UseEndpoints(endpoints =>endpoints.MapControllers());




app.Run(configuration["ServerAddress"]);
