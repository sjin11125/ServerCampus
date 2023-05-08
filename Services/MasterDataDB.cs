using Com2usServerCampus.Model;
using Microsoft.Extensions.Options;
using System.Data;
using MySqlConnector;
using SqlKata.Execution;
using SqlKata;
using System.Dynamic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Com2usServerCampus.Services;
public class MasterDataDB : IMasterDataDB
{
    ILogger<MasterDataDB> logger;
    IOptions<DBConfig> configuration;

    IDbConnection _dbconn;
    SqlKata.Compilers.MySqlCompiler compiler;
    SqlKata.Execution.QueryFactory queryFactory;

    public MasterDataDB(ILogger<MasterDataDB> logger, IOptions<DBConfig> configuration)
    {
        this.logger = logger;
        this.configuration = configuration;

        _dbconn = new MySqlConnection(configuration.Value.DataDB);
        compiler = new SqlKata.Compilers.MySqlCompiler();
        queryFactory = new SqlKata.Execution.QueryFactory(_dbconn, compiler);
    }

   public async Task<(ErrorCode,ItemData)> GetItemData(int code)   //아이템 데이터 불러오기
    {
        var data=await queryFactory.Query("item").Where("Code",code).FirstOrDefaultAsync<ItemData>();

        if (data is null)
            return (ErrorCode.InvalidItemData, null);

        return (ErrorCode.None, data);

    }
    public async Task<(ErrorCode, string)> GetItemAttributeData(int code)   //아이템 특성 불러오기
    {
        var data = await queryFactory.Query("itemAttribute").Where("Code", code).Select("Name").FirstOrDefaultAsync<string>();

        if (string.IsNullOrEmpty(data))
            return (ErrorCode.InvanlidItemAttributeData, null);

        return (ErrorCode.None, data);

    }
    public async Task<(ErrorCode, AttendanceReward)> GetAttendanceRewardData(int code)      //출석 보상 불러와
    {
        var reward= await queryFactory.Query("attendance").Where("Code", code).FirstOrDefaultAsync<AttendanceReward>();
        if (reward is null)
            return (ErrorCode.InvalidItemData, null);

        return (ErrorCode.None, reward);

    }

    public async Task<(ErrorCode,List<InAppProduct> )> GetInAppProduct(int code)    //인앱상품 데이터 불러오기
    { 
        var data=await queryFactory.Query("inAppProduct").Where("Code",code).GetAsync<InAppProduct>();

        if (data is null)
            return (ErrorCode.InvalidItemData, null);


        return (ErrorCode.None, data.ToList());
    }
}

