using Com2usServerCampus.Model;

namespace Com2usServerCampus.Services
{
    public interface IAccountDB
    {
        public Task<DBUserInfo> CheckUser(string email);
        public Task<long> AddUser(string email, string hashedPassword);
    }
}
