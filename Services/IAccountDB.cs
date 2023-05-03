using Com2usServerCampus.Model;

namespace Com2usServerCampus.Services
{
    public interface IAccountDB
    {
        public Task<DBUserInfo> CheckUser(string email);
        public Task<ErrorCode> AddUser(string email, string hashedPassword);
    }
}
