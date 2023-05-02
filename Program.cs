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
//DI �����̳ʿ� IAccountDb �������̽��� AccountDb Ŭ������ ����� IAccountDb�� ��û�� ������ AccountDb Ŭ������ �ν��Ͻ��� ������ ��ȯ

builder.Services.AddTransient<IGameDB, GameDB>();
//Transient: ������ ��û, ���񽺸��� ���ο� ��ü�� �����ϴ� ������ ����

builder.Services.AddSingleton<IRedisDB, RedisDB>();
//Singleton: ������ ������ �� ���� �����ϴ� ���������� ��ü�� ������ ������ ����
//���𽺴� ������ ������ ������ �ѹ��� ȣ���ص� �ȴ�

builder.Services.AddControllers();



var app = builder.Build();


var configuration = app.Configuration;

//���� �̵���� �߰�
//�۹��� üũ �̵���� �߰�
app.UseRouting();
app.UseEndpoints(endpoints =>endpoints.MapControllers());




app.Run(configuration["ServerAddress"]);
