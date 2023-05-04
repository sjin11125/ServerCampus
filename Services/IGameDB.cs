using Com2usServerCampus.Model;

namespace Com2usServerCampus.Services
{
    public interface IGameDB
    {
        public Task<( ErrorCode,UserInfo)> GetGameData(string email);
        public Task<(ErrorCode, List<UserItem>)> GetItems(string email);
        public Task<ErrorCode> InsertItem(string email, UserItem useritem);
        public Task<ErrorCode> InsertGameData(string email, UserInfo userInfo);
    }

}
