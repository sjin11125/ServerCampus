using CloudStructures;
using Com2usServerCampus;
using MySqlConnector;
using static Microsoft.Extensions.Logging.ILogger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddLogging();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


var app = builder.Build();


var configuration = app.Configuration;

app.UseRouting();
app.UseEndpoints(endpoints =>endpoints.MapControllers());

DBManager.Init(configuration);              //데이터 베이스 연동


app.Run(configuration["ServerAddress"]);
