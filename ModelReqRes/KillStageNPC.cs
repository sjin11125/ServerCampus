namespace Com2usServerCampus.ModelReqRes;
public class KillStageNPCRequest
{
    public string UserId { get; set; }
    public int StageCode { get; set; }
    public int NPCCode { get; set; }
}
public class KillStageNPCResponse
{
    public ErrorCode Error { get; set; }
}

