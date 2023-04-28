namespace Com2usServerCampus
{
    public class UserInfo
    {
        public int Exp { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }

        public List<UserItem> ItemList { get; set; }
        public UserInfo( int exp, int attack, int defence)
        {
            Exp = exp;
            Attack = attack;
            Defence = defence;
        }
    }
    public struct UserItem
    {
        public int AccountId;       //계정번호
        public int ItemCode;        //아이템 코드
        public int EnhanceCount;        //강화횟수
        public int Count;           //몇개인지
    }
}
