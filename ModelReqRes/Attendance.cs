namespace Com2usServerCampus.ModelReqRes;
public class AttendanceRequest
{
   public  string UserId { get; set; }
}
public class AttendanceResponse
{
    public ErrorCode Error { get; set; }
}

