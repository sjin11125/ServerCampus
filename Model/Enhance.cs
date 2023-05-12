namespace Com2usServerCampus.Model;
public class EnhanceItemInfo             //강화한 아이템 정보
{
    public string Email { get; set; }   
    public int ItemCode { get; set; }
    public int ItemId { get; set; }
    public int EnhanceCount { get; set; }
    public string Attribute { get; set; }
    public int BeforeValue { get; set; }
    public int AfterValue { get; set; }
    public bool isSuccess { get; set; } 
    public DateTime Date { get; set; }


}
