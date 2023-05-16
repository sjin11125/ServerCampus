using Com2usServerCampus.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace Com2usServerCampus.ModelReqRes;
[Serializable]
public class MailLoadRequest
{
    public string UserId { get; set; }
    public int Page { get; set; }
    public string Authtoken { get; set; }
    public MailLoadRequest(string email, int page, string authtoken)
    {
        UserId = email;
        Page = page;
        Authtoken = authtoken;
    }
}


public class MailLoadResponse
{

    public List<Mail> Mails { get; set; }
  

   public  ErrorCode Error { get; set; }

}
