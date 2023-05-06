using Com2usServerCampus.Model;

namespace Com2usServerCampus.Services
{
    public interface IMasterDataDB
    {
        public Task<(ErrorCode, ItemData)> GetItemData(int code);  
        public  Task<(ErrorCode, AttendanceReward)> GetAttendanceRewardData(int code);

        public Task<(ErrorCode, List<UserItem>)> GetInAppProduct(int code); 

    }

}
