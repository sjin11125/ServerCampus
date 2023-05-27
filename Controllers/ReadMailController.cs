using CloudStructures;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;
using  Com2usServerCampus.ModelReqRes;
using Com2usServerCampus.Services;
using Com2usServerCampus.Model;
using static Com2usServerCampus.LogManager;

namespace Com2usServerCampus.Controllers;
[ApiController]
[Route("[controller]")]

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

       (var errorContent, var Content) = await _gameDB.ReadMail(mailInfo.Id);
        if (errorContent!=ErrorCode.None)
        {
            readMaildResponse.Error = errorContent;
            return readMaildResponse;
        }
        readMaildResponse.Error = ErrorCode.None;
        readMaildResponse.Content = Content;



        _logger.ZLogInformationWithPayload(EventIdDictionary[EventType.ReadMail], new { Email = mailInfo.Email }, $"ReadMail Success");

        return readMaildResponse;
    }
}

