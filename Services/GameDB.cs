using Com2usServerCampus.Model;

namespace Com2usServerCampus.Services;
public class GameDB : IGameDB
{
    Task<UserInfo> IGameDB.GetGameData(string email)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<UserItem>> IGameDB.GetItems(string email)
    {
        throw new NotImplementedException();
    }
}

