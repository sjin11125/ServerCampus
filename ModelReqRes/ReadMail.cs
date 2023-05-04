using System.ComponentModel.DataAnnotations;
using Com2usServerCampus.Model;

namespace Com2usServerCampus.ModelReqRes;

public class ReadMaildRequest
{
    public string Email { get;set; }
    public int Id { get; set; }         //메일 id

}
public class ReadMaildResponse
{
    public int Id { get; set; }         //메일 id
    public string Content { get; set; }  
    public ErrorCode Error{ get; set; }
}

