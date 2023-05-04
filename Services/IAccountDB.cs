using Com2usServerCampus.Model;

namespace Com2usServerCampus.Services
{
    public interface IAccountDB
    {
        public Task<(ErrorCode, DBUserInfo)> CheckUser(string email, string password);
        public Task<ErrorCode> AddUser(string email, string hashedPassword);
    }
}
