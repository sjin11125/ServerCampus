namespace Com2usServerCampus
{
    [Serializable]
    public struct UserInfo
    {
        public int AccountId { get; set; }  
        public int Exp { get; set; }
        public int Attack { get; set; }
        public int Defence { get; set; }


      
        public UserInfo( int exp, int attack, int defence)
        {
            Exp = exp;
            Attack = attack;
            Defence = defence;
        }
    }
    [Serializable]
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
}
