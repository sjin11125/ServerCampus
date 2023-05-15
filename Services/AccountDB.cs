using Com2usServerCampus.Model;
using Microsoft.Extensions.Options;
using SqlKata.Execution;
using ZLogger;
using MySqlConnector;
using System.Data;
using Com2usServerCampus.ModelReqRes;

namespace Com2usServerCampus.Services;

public class AccountDB : IAccountDB
{
    ILogger<AccountDB> _logger;
    IOptions<DBConfig> configuration;

    IDbConnection _dbconn;
    SqlKata.Compilers.MySqlCompiler compiler;
    SqlKata.Execution.QueryFactory queryFactory;

    public AccountDB(ILogger<AccountDB> logger, IOptions<DBConfig> configuration)
    {
        this._logger = logger;
        this.configuration = configuration;

        _dbconn = new MySqlConnection(configuration.Value.AccountDB);//mysql 연결
        _dbconn.Open();

        compiler = new SqlKata.Compilers.MySqlCompiler();
        queryFactory = new SqlKata.Execution.QueryFactory(_dbconn, compiler);

    }
    public void Dispose()
    {
        _dbconn.Close();
    }

    public async Task<ErrorCode> AddUser(string email, string password)
    {
        try
        {
            string HashedPassword = Security.Encrypt(password);  //비번 암호화

       
            var count = await queryFactory.Query("account").InsertAsync(new {   //데이터 삽입 (성공 1, 실패 0)
            Email=email,
            HashedPassword=HashedPassword

            });
            _logger.ZLogDebug($"CreateAccount Emila:{email}, HashedPassword: {HashedPassword}");

            if (count!=1)       //실패
            {
                return ErrorCode.CreateAccount_Fail_Dup;
            }
            return ErrorCode.None;
        }
        catch (Exception e)
        {
            _logger.ZLogError(e,$"{e.Message} ErrorCode: {ErrorCode.CreateAccount_Fail_Exception}, UserId:{email}");
            return ErrorCode.CreateAccount_Fail_Exception;
        }
    }

   public async Task<(ErrorCode,DBUserInfo)> CheckUser(string email,string password)
    {
        try
        {
        string HashedPassword = Security.Encrypt(password);  //비번 암호화 

       var count = await queryFactory.Query("account").Where("UserId", email).FirstOrDefaultAsync<DBUserInfo>();

        if (count is null)           //이메일 틀리면?
        {
            _logger.ZLogError($"AccountDB.CheckUser  ErrorCode: {ErrorCode.Login_Fail_Email}  UserId: {email}");
            return (ErrorCode.Login_Fail_Email, null); }

        if (HashedPassword != count.HashedPassword)       //비번 틀리면?
        {
            _logger.ZLogError($"AccountDB.CheckUser  ErrorCode: {ErrorCode.Login_Fail_Password}  UserId: {email} Password: {password}");
            return (ErrorCode.Login_Fail_Password, null); }

        //다 맞으면

        return (ErrorCode.None,count);

        }
        catch (Exception e)
        {
            _logger.ZLogError(e,$"AccountDB.CheckUser Exception ErrorCode: {ErrorCode.Login_Fail_Password}  UserId: {email} Password: {password}");

            throw;
        }
    }
}


