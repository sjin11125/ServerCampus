using Com2usServerCampus.Model;

namespace Com2usServerCampus.ModelReqRes;

public class InAppPurchaseRequest
{
    public InAppPurchaseReceipt InAppPurchaseReceipt { get; set; }
    
}
public class InAppPurchaseResponse
{
    ErrorCode Error { get; set; }
}

