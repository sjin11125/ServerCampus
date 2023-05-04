using Com2usServerCampus.Model;

namespace Com2usServerCampus.Services
{
    public interface IGameDB
    {
        public Task<( ErrorCode,UserInfo)> GetGameData(string email);
        public Task<(ErrorCode, List<UserItem>)> GetItems(string email);
        public Task<ErrorCode> InsertItem(string email, UserItem useritem);
        public Task<ErrorCode> InsertGameData(string email, UserInfo userInfo);
        public  Task<(ErrorCode, List<Mail>)> GetMails(string email, int page);
        public Task<(ErrorCode, string)> ReadMail(string email, int id);


    }

}
