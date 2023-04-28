namespace Com2usServerCampus
{
    public class UserInfo
    {
        public int Money { get; set; }  
        public int Exp { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }

        public UserInfo(int money, int exp, int attack, int defence)
        {
            Money = money;
            Exp = exp;
            Attack = attack;
            Defence = defence;
        }
    }
}
