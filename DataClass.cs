namespace Com2usServerCampus
{

    public class CreateAccountRequest //유저가 서버에게 주는 아이디, 비번 데이터 클래스
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class CreateAccountResponse //서버가 유저에게 주는 응답 클래스
    {
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public ErrorCode Error { get; set; }

    }
    public class ItemData    //아이템
    {
        public int Code { get; set; }   
        public string Name { get; set; }
        public int Attribute { get; set; }
        public int Sell { get; set; }
        public int Buy { get; set; }
        public int UseLv { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }
        public int Magic { get; set; }
        public int EnhanceMaxCount { get; set; }

    }

    public class ItemAttribute  //아이템 특성
    {
        public string Name { get; set; }   
        public int Code { get; set; }
    }

    public class AttendanceReward       //출석보상
    {
        public int Code { get; set; }
        public int ItemCode { get; set; }
        public int Count { get; set; }
    }

    public class InAppProduct       //인앱상품
    {
        public int Code { get; set; }
        public int ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ItemCount { get; set; }
    }

    public class StageItem      //스테이지 아이템
    {
        public int Code { get; set; }
        public int ItemCode { get; set; }
    }
    public class StageNPC   //스테이지 공격 NPC
    {
        public int Code { get; set; }
        public int NPCCode { get; set; }
        public int Count { get; set; }
        public int Exp { get; set; }
    }
    public class LoginAccountRequest //유저가 서버에게 주는 아이디, 비번 데이터 클래스
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginAccountResponse //서버가 유저에게 주는 응답 클래스
    {
        public ErrorCode Error { get; set; }
        public UserInfo userInfo { get; set; }
        public List<UserItem> itemList { get; set; }
        public List<Notice> NoticeList { get; set; }
        public string Authtoken { get; set; }
    }
    public class DBUserInfo
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }

      public  ErrorCode Error { get; set; }
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
}
