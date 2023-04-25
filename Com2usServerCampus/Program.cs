using CloudStructures;
using Com2usServerCampus;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


var app = builder.Build();

// Configure the HTTP request pipeline.
/*if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/
var configuration = app.Configuration;

app.UseRouting();
app.UseEndpoints(endpoints =>endpoints.MapControllers());

DBManager.Init(configuration);              //데이터 베이스 연동


app.Run(configuration["ServerAddress"]);
