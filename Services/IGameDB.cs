using Com2usServerCampus.Model;

namespace Com2usServerCampus.Services
{
    public interface IGameDB
    {
        public Task<UserInfo> GetGameData(string email);
        public Task<IEnumerable<UserItem>> GetItems(string email);
    }

}
