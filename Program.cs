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
//DI �����̳ʿ� IAccountDb �������̽��� AccountDb Ŭ������ ����� IAccountDb�� ��û�� ������ AccountDb Ŭ������ �ν��Ͻ��� ������ ��ȯ

builder.Services.AddTransient<IGameDB, GameDB>();
//Transient: ������ ��û, ���񽺸��� ���ο� ��ü�� �����ϴ� ������ ����

builder.Services.AddSingleton<IMasterDataDB, MasterDataDB>();

builder.Services.AddSingleton<IRedisDB, RedisDB>();
//Singleton: ������ ������ �� ���� �����ϴ� ���������� ��ü�� ������ ������ ����


builder.Services.AddControllers();

SetLogger();        //�α� ����

var app = builder.Build();


var loggerFactory=app.Services.GetRequiredService<ILoggerFactory>();
LogManager.SetLoggerFactory(loggerFactory,"Global");



app.UseMiddleware<CheckUserAuth>();     //���� �̵���� �߰�
//�۹��� üũ �̵���� �߰�

app.UseRouting();
app.UseEndpoints(endpoints =>endpoints.MapControllers());


var redis = app.Services.GetRequiredService<IRedisDB>();            //RedisDB ��ü �ҷ���
redis.Init(builder.Configuration.GetSection("DBConnection")["RedisDB"]);        //���� ���� �ʱ�ȭ
//���𽺴� ������ ������ ������ �ѹ��� �ʱ�ȭ�Ѵ�

var masterData = app.Services.GetRequiredService<IMasterDataDB>();
var result = await masterData.Init();        //마스터데이터 불러오기
if (result != ErrorCode.None)     //정상적으로 안됐다
    return;
    


app.Run(configuration["ServerAddress"]);




void SetLogger()
{
    var logging = builder.Logging;
    //�α� ������ ������

    logging.ClearProviders();
    //������ ��ϵǾ� �ִ� �α� ���ι��̴��� ����

    var path = configuration["logdir"];
    //appsettings.Development.json�� logdir ������ ��Ʈ���� �����´�(������ ������ ���)

    if (!Directory.Exists(path)) //��ΰ� �������� �ʴ´�
    {
        Directory.CreateDirectory(path);        //��ο� ������ ����
    }

    logging.AddZLoggerRollingFile( //���Ͽ� �α׸� ����ϴ� Rolling File �α� ���ι��̴��� �߰���
        (time, x) => $"{path}{time.ToLocalTime():yyyy-MM-dd}_{x:000}.log",       //�α������� �̸��� ���� (dt: DateTime, x: �ε���)
        x => x.ToLocalTime().Date,  //�α� ������ �� ������ �и�->��¥���� �α������� ������
        1024,  //���� �ϳ��� �ִ� ũ��
        options =>
        {     //�α� ������ ������ �� ����
            options.EnableStructuredLogging = true;   //�α� ������ ����ȭ�� JSON ���·� ������ �� ����
            var time = JsonEncodedText.Encode("Timestamp");  //�ð� ������ ����(?)

            //DateTime.Now�� UTC+0 �̰� �ѱ��� UTC+9�̹Ƿ� 9�ð��� ���� ���� ����Ѵ�.
            var timeValue = JsonEncodedText.Encode(DateTime.Now.AddHours(9).ToString("yyyy-MM-dd HH:mm:ss"));

            options.StructuredLoggingFormatter = (writer, info) =>
            {
                writer.WriteString(time, timeValue);
                info.WriteToJsonWriter(writer);     //JSON���� ����
            };
        }
        );

    logging.AddZLoggerConsole(options =>           //�ֿܼ� �α׸� ����ϴ� �ܼ� �α� ���ι��̴� �߰�
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

 async Task GetMasterData(WebApplication app)
{
    
}