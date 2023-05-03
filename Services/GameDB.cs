using Com2usServerCampus.Model;
using Microsoft.Extensions.Options;

namespace Com2usServerCampus.Services;
public class GameDB : IGameDB
{
    ILogger<GameDB> logger;
    IOptions<DBConfig> configuration;

    public GameDB(ILogger<GameDB> logger, IOptions<DBConfig> configuration)
    {
        this.logger = logger;
        this.configuration = configuration;
    }

    Task<UserInfo> IGameDB.GetGameData(string email)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<UserItem>> IGameDB.GetItems(string email)
    {
        throw new NotImplementedException();
    }
}

