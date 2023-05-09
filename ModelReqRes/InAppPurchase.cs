using Com2usServerCampus.Model;

namespace Com2usServerCampus.ModelReqRes;

public class InAppPurchaseRequest
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Title { get; set; }
    public int Code { get; set; }   //상품 코드
    public string Content { get; set; }
    public DateTime Time { get; set; }

    public int ExpiryTime { get; set; }

}
public class InAppPurchaseResponse
{
  public  ErrorCode Error { get; set; }
}

