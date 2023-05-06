
namespace Com2usServerCampus.Model;
public class InAppPurchaseReceipt
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Title { get; set; }   
    public string Content { get; set; } 
    public DateTime Time { get; set; }

    public int ExpiryTime { get; set; } 

}
public class InAppPurchaseItem
{
    public int Id { get; set; }
    public int Code { get; set; }
    public int Count { get; set; }


}

