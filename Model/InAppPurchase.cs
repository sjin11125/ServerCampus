
namespace Com2usServerCampus.Model;
public class InAppPurchaseReceipt
{
    public string Id { get; set; }
    public string Email { get; set; }
    public string Title { get; set; }   
    public int Code { get; set; }   //상품 번호
    public string Content { get; set; } 
    public DateTime Time { get; set; }

    public int ExpiryTime { get; set; } 

}
public class InAppPurchaseItem
{
    public string Id { get; set; }
    public int Code { get; set; }


}

