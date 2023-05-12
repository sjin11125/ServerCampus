using Com2usServerCampus.Model;

namespace Com2usServerCampus.ModelReqRes;
public class StageSelectRequest
{
    public string UserId { get; set; }
    public int StageId { get; set; }
}
public class StageSelectResponse
{

    public List<StageItem> StageItems { get; set; }
    public List<StageNPC> StageNPCs { get; set; }
    public ErrorCode Error { get; set; }
}


