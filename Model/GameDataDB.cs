namespace Com2usServerCampus.Model;
public struct UserInfo
{
    public int AccountId { get; set; }
    public int Exp { get; set; }
    public int Attack { get; set; }
    public int Defence { get; set; }



    public UserInfo(int exp, int attack, int defence)
    {
        Exp = exp;
        Attack = attack;
        Defence = defence;
    }
}

public struct UserItem
{
    public string Eamil { get; set; }     //계정번호
    public int ItemCode { get; set; }        //아이템 코드
    public int EnhanceCount { get; set; }        //강화횟수
    public int Count { get; set; }           //몇개인지

    public UserItem(string email, int itemCode, int enhanceCount, int count)
    {
        Eamil = email;
        ItemCode = itemCode;
        EnhanceCount = enhanceCount;
        Count = count;
    }
}
public class Notice
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string Date { get; set; }
    public string isRead { get; set; }

    public Notice(string title, string content, string date, string isRead)
    {
        Title = title;
        Content = content;
        Date = date;
        this.isRead = isRead;
    }
}
public class Mail
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public bool isRead { get; set; }
    public bool isGet { get; set; }

    public List<MailItem> Items { get; set; }
}
public class MailItem       //메일에 있는 보상 아이템 리스트
{
    public int Id { get; set; }
    public int Email { get; set; }
    public int Code { get; set; }
    public int Count { get; set; }
}
