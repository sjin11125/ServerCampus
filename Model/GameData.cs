namespace Com2usServerCampus.Model;
public class UserInfo
{
    public string Email { get; set; }
    public int Exp { get; set; }
    public int Attack { get; set; }
    public int Defence { get; set; }
    public DateTime Attendance { get; set; }    
    public int AttendanceCount { get; set; }    


    public UserInfo(string email,int exp, int attack, int defence, DateTime attendance)
    {
        Email = email;
        Exp = exp;
        Attack = attack;
        Defence = defence;
        Attendance = attendance;
    }
}

public struct UserItem
{
    public string Eamil { get; set; }     //계정번호
    public int ItemCode { get; set; }        //아이템 코드
    public int EnhanceCount { get; set; }        //강화횟수
    public int Count { get; set; }           //몇개인지

    public bool IsCount { get; set; }       //겹칠수있는지

    public UserItem(string email, int itemCode, int enhanceCount, int count,bool isCount)
    {
        Eamil = email;
        ItemCode = itemCode;
        EnhanceCount = enhanceCount;
        Count = count;
        IsCount = isCount;
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

