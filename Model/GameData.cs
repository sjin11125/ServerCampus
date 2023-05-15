namespace Com2usServerCampus.Model;
public class UserInfo
{
    public string UserId { get; set; }
    public int Exp { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public DateTime Attendance { get; set; }    
    public int AttendanceCount { get; set; }    
    public int Stage { get; set; }  


    public UserInfo(string email, int exp, int attack, int defense, DateTime attendance, int attendanceCount, int stage)
    {
        UserId = email;
        Exp = exp;
        Attack = attack;
        Defense = defense;
        Attendance = attendance;
        AttendanceCount = attendanceCount;
        Stage = stage;
    }
}

public class UserItem
{
    public string UserId { get; set; }     //계정번호
    public int ItemId { get; set; }     //아이템 Id
    public int ItemCode { get; set; }        //아이템 코드
    public int EnhanceCount { get; set; }        //강화횟수
    public int ItemCount { get; set; }           //몇개인지

    public int Attack { get; set; }
    public int Defence { get; set; }
    public int Magic { get; set; }


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

