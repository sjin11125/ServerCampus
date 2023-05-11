namespace Com2usServerCampus.ModelReqRes;
public class EnhanceRequest
{
    public string Email { get; set; }
    public int Id { get; set; }
}

public class EnhanceResponse
{
   public ErrorCode Error { get; set; }
}

 