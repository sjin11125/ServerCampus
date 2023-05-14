namespace Com2usServerCampus.ModelReqRes;

public class EndStageRequest
{
    public string UserId { get; set; }
    public int StageCode { get; set; }
    public bool isClear { get; set; }

}

public class EndStageResponse
{
    public ErrorCode Error { get; set; }
}


