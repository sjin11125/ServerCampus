using System.ComponentModel.DataAnnotations;
using Com2usServerCampus.Model;

namespace Com2usServerCampus.ModelReqRes;

public class ReadMailRequest
{
    public string Email { get;set; }
    public int Id { get; set; }         //메일 id
    public string AuthToken { get; set; }         //메일 id

}
public class ReadMailResponse
{
    public int Id { get; set; }         //메일 id
    public string Content { get; set; }  
    public ErrorCode Error{ get; set; }
}

