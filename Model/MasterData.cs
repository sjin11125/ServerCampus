namespace Com2usServerCampus.Model;



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
    public bool isCount { get; set; }   
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

public class InAppProduct:UserItem       //인앱상품
{
    public int Code { get; set; }
    public string ItemName { get; set; }
}

public class StageItem      //스테이지 아이템
{
    public int Code { get; set; }           //스테이지 넘버
    public int ItemCode { get; set; }
}
public class StageNPC   //스테이지 공격 NPC
{
    public int Code { get; set; }
    public int NPCCode { get; set; }
    public int Count { get; set; }
    public int Exp { get; set; }
}


