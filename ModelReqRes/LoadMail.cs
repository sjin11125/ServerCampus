using Com2usServerCampus.Model;
using System;
using System.ComponentModel.DataAnnotations;

namespace Com2usServerCampus.ModelReqRes;
[Serializable]
public class MailLoadRequest
{
    public string Email { get; set; }
    string email;
    public int Page { get; set; }
    int page;
    public string Authtoken { get; set; }
    string authtoken;
    public MailLoadRequest(string email, int page, string authtoken)
    {
        Email = email;
        Page = page;
        Authtoken = authtoken;
    }
}
[Serializable]

public class MailLoadResponse
{
    List<Mail> Mails = new List<Mail>();
    public List<Mail> GetMailList()
    {
        return Mails;
    }

   public  ErrorCode Error { get; set; }

}
