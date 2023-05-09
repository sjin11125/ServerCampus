﻿using CloudStructures;
using CloudStructures.Structures;
using Com2usServerCampus.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;
using Com2usServerCampus.ModelReqRes;
using Com2usServerCampus.Services;

namespace Com2usServerCampus.Controllers;

[Route("[controller]")]
[ApiController]
public class LoadMailController : ControllerBase
{
    ILogger<LoadMailController> logger;
    IGameDB _gameDB;
    public LoadMailController(ILogger<LoadMailController> logger, IGameDB gameDB)
    {
        this.logger = logger;
        this._gameDB = gameDB;
    }

    [HttpPost]
    public async Task<MailLoadResponse> MailPost(MailLoadRequest MailInfo)
    {
        MailLoadResponse mailLoadResponse = new MailLoadResponse();

        (var errorMail, var mailInfos) =await _gameDB.GetMails(MailInfo.Email, MailInfo.Page);

        if (errorMail != ErrorCode.None)         //메일이 없다면
        {
            mailLoadResponse.Error = errorMail;
            return mailLoadResponse;
        }
         mailLoadResponse.Mails = mailInfos;

        return mailLoadResponse;
    }
}
    

