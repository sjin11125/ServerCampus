using Com2usServerCampus.Model;
using Microsoft.Extensions.Options;
using System.Data;
using MySqlConnector;
using SqlKata.Execution;
using SqlKata;
using System.Dynamic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq;

namespace Com2usServerCampus.Services;
public class MasterDataDB : IMasterDataDB
{
    ILogger<MasterDataDB> logger;
    IOptions<DBConfig> configuration;

    IDbConnection _dbconn;
    SqlKata.Compilers.MySqlCompiler compiler;
    SqlKata.Execution.QueryFactory queryFactory;

    public List<ItemData> ItemDataList { get; set; }
    public List<ItemAttribute> ItemAttributeDataList { get; set; }  
    public List<AttendanceReward> AttendanceRewardDataList { get; set; }

    public List<InAppProduct> InAppProductDataList { get; set; }    

    public MasterDataDB(ILogger<MasterDataDB> logger, IOptions<DBConfig> configuration)
    {
        this.logger = logger;
        this.configuration = configuration;

        _dbconn = new MySqlConnection(configuration.Value.DataDB);
        compiler = new SqlKata.Compilers.MySqlCompiler();
        queryFactory = new SqlKata.Execution.QueryFactory(_dbconn, compiler);
    }

    public async Task<ErrorCode> Init()
    {
       var  itemDatas = await queryFactory.Query("item").GetAsync<ItemData>();
        if (itemDatas.Count() == 0)
            return ErrorCode.InvalidItemData;


            ItemDataList = itemDatas.ToList();
        

        var itemAttributes = await queryFactory.Query("itemAttribute").GetAsync<ItemAttribute>();
        if (itemAttributes.Count() == 0)
            return ErrorCode.InvalidItemData;

        ItemAttributeDataList = itemAttributes.ToList();



        var attendances = await queryFactory.Query("attendance").GetAsync<AttendanceReward>();
        if (attendances.Count() == 0)
            return ErrorCode.InvalidItemData;

        AttendanceRewardDataList =attendances.ToList();



        var inAppProducts=await queryFactory.Query("inAppProduct").GetAsync<InAppProduct>();
        if (inAppProducts.Count() == 0)
            return ErrorCode.InvalidItemData;

        InAppProductDataList = inAppProducts.ToList();

        return ErrorCode.None;


    }
   public (ErrorCode,ItemData) GetItemData(int code)   //아이템 데이터 불러오기
    {
        var data =ItemDataList.Find(x => x.Code==code);
        if (data is not null)
        {
            return (ErrorCode.None, data);
        }
        else
            return (ErrorCode.InvalidItemData, null);

       

    }
    public  (ErrorCode, ItemAttribute) GetItemAttributeData(int code)   //아이템 특성 불러오기
    {
        var data = ItemAttributeDataList.Find(x => x.Code == code);
        if (data is not null)
        {
            return (ErrorCode.None, data);
        }
        else
            return (ErrorCode.InvalidItemData, null);


    }
    public  (ErrorCode, AttendanceReward) GetAttendanceRewardData(int code)      //출석 보상 불러와
    {
        var data = AttendanceRewardDataList.Find(x => x.Code == code);
        if (data is not null)
        {
            return (ErrorCode.None, data);
        }
        else
            return (ErrorCode.InvalidItemData, null);

    }

    public  (ErrorCode,List<InAppProduct> ) GetInAppProduct(int code)    //인앱상품 데이터 불러오기
    {
        var data = InAppProductDataList.FindAll(x=>x.Code==code);
        if (data is not null)
        {
            return (ErrorCode.None, data);
        }
        else
            return (ErrorCode.InvalidItemData, null);
    }
}

