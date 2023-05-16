using CloudStructures;
using CloudStructures.Structures;
using Com2usServerCampus.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;
using Com2usServerCampus.ModelReqRes;
using Com2usServerCampus.Services;
using static Com2usServerCampus.LogManager;
using System.Dynamic;

namespace Com2usServerCampus.Controllers;

[Route("[controller]")]
[ApiController]
public class LoadMailController : ControllerBase
{
    ILogger<LoadMailController> _logger;
    IGameDB _gameDB;
    public LoadMailController(ILogger<LoadMailController> logger, IGameDB gameDB)
    {
        this._logger = logger;
        this._gameDB = gameDB;
    }

    [HttpPost]
    public async Task<MailLoadResponse> MailPost(MailLoadRequest MailInfo)
    {
        MailLoadResponse mailLoadResponse = new MailLoadResponse();

        (var errorMail, var mailInfos) =await _gameDB.GetMails(MailInfo.UserId, MailInfo.Page);

        if (errorMail != ErrorCode.None)         //메일이 없다면
        {
            mailLoadResponse.Error = errorMail;
            return mailLoadResponse;
        }
        mailLoadResponse.Mails = mailInfos;


        _logger.ZLogInformationWithPayload(EventIdDictionary[EventType.LoadMail], new { UserId = MailInfo.UserId }, $"LoadMail Success");


        return mailLoadResponse;
    }
}
    

