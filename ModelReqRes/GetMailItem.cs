using System.ComponentModel.DataAnnotations;
using Com2usServerCampus.Model;

namespace Com2usServerCampus.ModelReqRes;

public class GetMailItemRequest
{
    public string UserId { get;set; }
    public int MailId { get; set; }         //메일 id

}
public class GetMailItemResponse
{
  public ErrorCode Error { get; set; }
}



