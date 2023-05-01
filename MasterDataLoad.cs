using Microsoft.AspNetCore.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Com2usServerCampus
{
    public class MasterDataLoad
    {
        string[] MasterDataString = new string[6] { "아이템 번호,이름,특성,판매 금액,구입 금액,사용가능 레벨,공격력,방어력,마법력,최대 강화 가능 횟수\r\nCode,Name,Attribute,Sell,Buy,UseLv,Attack,Defence,Magic,EnhanceMaxCount\r\n1,돈,5,0,0,0,0,0,0,0\r\n2,작은 칼,1,10,20,1,10,5,1,10\r\n3,도금 칼,1,100,200,5,29,12,10,10\r\n4,나무 방패,2,7,15,1,3,10,1,10\r\n5,보통 모자,3,5,8,1,1,1,1,10\r\n6,포션,4,3,6,1,0,0,0,0" ,
        "특성 이름,값\r\nName,Code\r\n무기,1\r\n방어구,2\r\n복장,3\r\n마법도구,4\r\n돈,5",
        "날짜,아이템 코드,개수\r\nCode,ItemCode,Count\r\n1,1,100\r\n2,1,100\r\n3,1,100\r\n4,1,200\r\n5,1,200\r\n6,1,200\r\n7,2,1\r\n8,1,100\r\n9,1,100\r\n10,1,100\r\n11,6,5\r\n12,1,150\r\n13,1,150\r\n14,1,150\r\n15,1,150\r\n16,1,150\r\n17,1,150\r\n18,4,1\r\n19,1,200\r\n20,1,200\r\n21,1,200\r\n22,1,200\r\n23,1,200\r\n24,5,1\r\n25,1,250\r\n26,1,250\r\n27,1,250\r\n28,1,250\r\n29,1,250\r\n30,3,1",
        "상품번호,아이템코드,아이템이름,개수\r\nCode,ItemCode,ItemName,ItemCount\r\n1,1,돈,1000\r\n1,2,작은 칼,1\r\n1,3,도금 칼,1\r\n2,4,나무 방패,1\r\n2,5,보통 모자,1\r\n2,6,포션,10\r\n3,1,돈,2000\r\n3,2,작은 칼,1\r\n3,3,나무 방패,1\r\n3,5,보통 모자,1",
        "스테이지 단계,파밍 가능 아이템\r\nCode,ItemCode\r\n1,1\r\n1,2\r\n2,3\r\n2,3",
        "스테이지 단계,공격 NPC,개수,보상경험치. 1개당\r\nCode,NPCCode,Count,Exp\r\n1,101,10,10\r\n1,110,12,15\r\n2,201,40,20\r\n2,211,20,35\r\n2,221,1,50"};
        
        public Dictionary<int,ItemData> itemDatas = new Dictionary<int, ItemData>();
        public Dictionary<string, ItemAttribute> itemAttributes = new Dictionary<string, ItemAttribute>();
        public Dictionary<int, AttendanceReward>   attendanceRewards = new Dictionary<int, AttendanceReward>();
        public Dictionary<int, InAppProduct> inAppProducts = new Dictionary<int, InAppProduct>();
        public Dictionary<int, StageItem> stageItems = new Dictionary<int, StageItem>();
        public Dictionary<int, StageNPC> stageNPCs = new Dictionary<int, StageNPC>();

        public MasterDataLoad()
        {
            for (int i = 2; i < MasterDataString[0].Split("\n").Length; i++)        //아이템 정보 파싱
            {
                string dataString = MasterDataString[0];

                string[] data= dataString.Split("\n");

                ItemData items=new ItemData();
                items.Code=int.Parse(data[i].Split(",")[0]);
                items.Name = data[i].Split(',')[1];
                items.Attribute = int.Parse(data[2].Split(',')[2]);
                items.Sell = int.Parse(data[i].Split(',')[3]);
                items.Buy = int.Parse(data[i].Split(',')[4]);
                items.UseLv = int.Parse(data[i].Split(',')[5]);
                items.Attack = int.Parse(data[i].Split(',')[6]);
                items.Defence = int.Parse(data[i].Split(',')[7]);
                items.Magic = int.Parse(data[i].Split(',')[8]);
                items.EnhanceMaxCount = int.Parse(data[i].Split(',')[9]);
                itemDatas.Add(items.Code,items);
            }
            for (int i = 2; i < MasterDataString[1].Split("\n").Length; i++)        //아이템 특성 파싱
            {
                string dataString = MasterDataString[1];

                string[] data= dataString.Split("\n");

                ItemAttribute attributes =new ItemAttribute();

                attributes.Name = data[i].Split(',')[0];
                attributes.Code=int.Parse(data[i].Split(",")[1]);

                itemAttributes.Add(attributes.Name, attributes);
            }
            for (int i = 2; i < MasterDataString[2].Split("\n").Length; i++)        //출석부보상 파싱
            {
                string dataString = MasterDataString[2];

                string[] data= dataString.Split("\n");

                AttendanceReward reward =new AttendanceReward();

                reward.Code=int.Parse(data[i].Split(",")[0]);
                reward.ItemCode=int.Parse(data[i].Split(",")[1]);
                reward.Count=int.Parse(data[i].Split(",")[2]);

                attendanceRewards.Add(reward.Code, reward);
            }
            for (int i = 2; i < MasterDataString[3].Split("\n").Length; i++)        //인앱상품 파싱
            {
                string dataString = MasterDataString[3];

                string[] data = dataString.Split("\n");

                InAppProduct product = new InAppProduct();

                product.Code = int.Parse(data[i].Split(",")[0]);
                product.ItemCode = int.Parse(data[i].Split(",")[1]);
                product.ItemName = data[i].Split(",")[2];
                product.ItemCount = int.Parse(data[i].Split(",")[3]);

                inAppProducts.Add(product.Code, product);
            }
            for (int i = 2; i < MasterDataString[4].Split("\n").Length; i++)        //스테이지 아이템 파싱
            {
                string dataString = MasterDataString[4];

                string[] data = dataString.Split("\n");

                StageItem product = new StageItem();

                product.Code = int.Parse(data[i].Split(",")[0]);
                product.ItemCode = int.Parse(data[i].Split(",")[1]);

                stageItems.Add(product.Code, product);
            }
            for (int i = 2; i < MasterDataString[5].Split("\n").Length; i++)        //스테이지 공격NPC 파싱
            {
                string dataString = MasterDataString[5];

                string[] data = dataString.Split("\n");

                StageNPC product = new StageNPC();

                product.Code = int.Parse(data[i].Split(",")[0]);
                product.NPCCode= int.Parse(data[i].Split(",")[1]);
                product.Count= int.Parse(data[i].Split(",")[2]);
                product.Exp= int.Parse(data[i].Split(",")[3]);

                stageNPCs.Add(product.Code, product);
            }
        }
    } 
}
