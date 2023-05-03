using Com2usServerCampus.Model;
using Microsoft.Extensions.Options;
using SqlKata.Execution;
using ZLogger;

namespace Com2usServerCampus.Services;

public class AccountDB : IAccountDB
{
    ILogger<AccountDB> logger;
    IOptions<DBConfig> configuration;

    public AccountDB(ILogger<AccountDB> logger, IOptions<DBConfig> configuration)
    {
        this.logger = logger;
        this.configuration = configuration;
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


