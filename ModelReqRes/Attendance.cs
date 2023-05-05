namespace Com2usServerCampus.ModelReqRes;
public class AttendanceRequest
{
   public  string Email { get; set; }
}
public class AttendanceResponse
{
    public ErrorCode Error { get; set; }
}

