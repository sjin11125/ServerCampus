namespace Com2usServerCampus.ModelReqRes;
public class AcquireStageItemRequest
{
    public string UserId { get; set; }  
    public int StageCode { get; set; }
    public int ItemCode { get; set; }    

}   
public class AcquireStageItemResponse
{
    public ErrorCode Error { get; set; }
}

