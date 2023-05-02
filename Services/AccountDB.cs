using Com2usServerCampus.Model;
using Microsoft.Extensions.Options;
using SqlKata.Execution;
using ZLogger;

namespace Com2usServerCampus.Services;

    public class AccountDB:IAccountDB
    {
    ILogger<AccountDB> logger;
    IOptions<AccountDB> options;

        public AccountDB(ILogger<AccountDB> logger, IOptions<AccountDB> options) { 
        this.logger = logger;
        this.options = options;
        }

    Task<long> IAccountDB.AddUser(string email, string hashedPassword)
    {
        throw new NotImplementedException();
    }

    Task<DBUserInfo> IAccountDB.CheckUser(string email)
    {
        throw new NotImplementedException();
    }
}

