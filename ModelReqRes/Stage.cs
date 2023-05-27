using Com2usServerCampus.Model;

namespace Com2usServerCampus.ModelReqRes;
    public class StageRequest
    {
    public string UserId { get; set; }  

    }   
public class StageResponse
    {
    public int StageId { get; set;}
    public ErrorCode Error { get; set; }    
    }

