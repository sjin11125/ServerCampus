using Com2usServerCampus.Model;
using Microsoft.Extensions.Options;
using System.Data;
using MySqlConnector;
using SqlKata.Execution;

namespace Com2usServerCampus.Services;
public class GameDB : IGameDB
{
    ILogger<GameDB> logger;
    IOptions<DBConfig> configuration;

    IDbConnection _dbconn;
    SqlKata.Compilers.MySqlCompiler compiler;
    SqlKata.Execution.QueryFactory queryFactory;

    public GameDB(ILogger<GameDB> logger, IOptions<DBConfig> configuration)
    {
        this.logger = logger;
        this.configuration = configuration;

        _dbconn = new MySqlConnection(configuration.Value.GameDB);
        compiler = new SqlKata.Compilers.MySqlCompiler();
        queryFactory = new SqlKata.Execution.QueryFactory(_dbconn, compiler);
    }

    public async Task<ErrorCode> InsertGameData(string email, UserInfo userInfo)
    {
      var count=  await queryFactory.Query("gamedata").InsertAsync(new
        {
            email,
            userInfo.Exp,
            userInfo.Attack,
            userInfo.Defence,
        });

        if (count != 1)       //실패
        {
            return ErrorCode.InsertGameDataDup;
        }
        return ErrorCode.None;
    }

    public async Task<ErrorCode> InsertItem(string email, UserItem userItem)
    {
        int result = 0;
        if (userItem.IsCount)       //겹치기가 가능한가
        {
            var count = await queryFactory.Query("gamedata").Where("Email", email).Where("Code", userItem.ItemCode).Select("Count").FirstOrDefaultAsync<int>();  //아이템 있는지 확인

            
            if (count == 0)//없으면 걍 넣고
            {
                result = await queryFactory.Query("gamedata").InsertAsync(new
                {
                    email,
                    userItem.ItemCode,
                    userItem.EnhanceCount,
                    userItem.Count
                });
            }
            else //있으면 기존 갯수 + 새로 들어온 갯수
            {
                int newCount = userItem.Count + count;
                 result = await queryFactory.Query("gamedata").InsertAsync(new
                {
                    email,
                    userItem.ItemCode,
                    userItem.EnhanceCount,
                    newCount
                });
            }
        }
        else                            //겹치기가 안돼면 걍 넣음
        {
             result = await queryFactory.Query("gamedata").InsertAsync(new
            {
                email,
                userItem.ItemCode,
                userItem.EnhanceCount,
                userItem.Count
            });
        }
        if (result!=1)  //실패
        {
            return ErrorCode.InsertItemDataFail;
        }
        else return ErrorCode.None;
    }

     async Task<(UserInfo,ErrorCode)> IGameDB.GetGameData(string email)
    {
        UserInfo userInfo = await queryFactory.Query("gamedata").Where("Email", email).FirstOrDefaultAsync<UserInfo>();
        if (userInfo==null)             //유저의 게임정보가 없다면
            return (null,ErrorCode.WrongGameData);
        else
            return (userInfo, ErrorCode.WrongGameData);
    }

    Task<IEnumerable<UserItem>> IGameDB.GetItems(string email)
    {
        throw new NotImplementedException();
    }
}

