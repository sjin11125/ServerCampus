using Com2usServerCampus.Model;

namespace Com2usServerCampus.Services
{
    public interface IMasterDataDB
    {
        public Task<(ErrorCode, ItemData)> GetItemData(int code);   //아이템 데이터 불러오기


    }

}
