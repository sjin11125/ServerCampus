using CloudStructures;
using CloudStructures.Structures;
using Com2usServerCampus.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;
using Com2usServerCampus.ModelReqRes;

namespace Com2usServerCampus.Controllers;

    [Route("[controller]")]
    [ApiController]
    public class LoadMailController:ControllerBase
    {
        ILogger logger;
        public LoadMailController(ILogger<LoadMailController> logger)
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
              var mails= await gamedb.Result.Query("mail").Select("Id","Email", "Title", "Content",  "isRead", "isGet").Where("Email", MailInfo.Email).Where("isRead",false).PaginateAsync<Mail>(MailInfo.Page,20);
                //해당 유저의 안읽은 메일들 불러오기
                List<Mail> mailList = mailLoadResponse.GetMails();
                foreach (var mail in mails.List)
                {
                   var items= await gamedb.Result.Query("mailItem").Where("Id", mail.Id).Where("Email", mail.Email).GetAsync<MailItem>();
                    //유저의 메일에 대한 보상 아이템 리스트를 받아옴

                    mail.Items = items.ToList();

                    mailList.Add(mail);
                }
            }

                return mailLoadResponse;
        }
    }
    

