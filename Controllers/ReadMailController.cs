using CloudStructures;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;
using  Com2usServerCampus.ModelReqRes;
using Com2usServerCampus.Services;

namespace Com2usServerCampus.Controllers;

public class ReadMailController : ControllerBase
{
    ILogger<ReadMailController> _logger;
    IGameDB _gameDB;

    public ReadMailController(ILogger<ReadMailController> logger, IGameDB gameDB)
    {
        _logger = logger;
        _gameDB = gameDB;
    }

    [HttpPost]
    public async Task<ReadMailResponse> MailReadPost(ReadMailRequest mailInfo)
    {
        ReadMailResponse readMaildResponse = new ReadMailResponse();

        var content = await _gameDB.ReadMail(mailInfo.Email, mailInfo.Id);
        if (content.Item1!=ErrorCode.None)
        {
            readMaildResponse.Error = content.Item1;
            return readMaildResponse;
        }
        readMaildResponse.Error = ErrorCode.None;
        readMaildResponse.Content = content.Item2;

        return readMaildResponse;
    }
}

