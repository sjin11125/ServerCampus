﻿namespace Com2usServerCampus.ModelReqRes;
public class EnhanceRequest
{
    public string UserId { get; set; }
    public int ItemId { get; set; }
}

public class EnhanceResponse
{
   public ErrorCode Error { get; set; }
}

 