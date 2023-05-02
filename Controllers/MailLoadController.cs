using CloudStructures;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;

namespace Com2usServerCampus.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MailLoadController:ControllerBase
    {
        ILogger logger;
        public MailLoadController(ILogger<MailLoadController> logger)
        {
            this.logger = logger;
        }

        [HttpPost]
        public async Task<MailLoadResponse> MailPost(MailLoadRequest MailInfo)
        {
            MailLoadResponse mailLoadResponse=new MailLoadResponse();

            DBManager dBManager = new DBManager();

            using (var gamedb= dBManager.GetGameDBQuery())
            {
              var mails= await gamedb.Result.Query("mail").Select("Email", "Title", "Content", "Code", "isRead", "isGet").Where("Email", MailInfo.Email).PaginateAsync<Mail>(MailInfo.Page,20);
                foreach (var mail in mails.List)
                {
                    mailLoadResponse.Mails.Add(mail);
                }
            }

                return mailLoadResponse;
        }
    }
    [Serializable]
    public class MailLoadRequest
    {
        public string Email { get; set; }   
        public int Page { get; set; }

    }
    [Serializable]

    public class MailLoadResponse
    {
      public  List<Mail> Mails=new List<Mail>();

    }
}
