using CloudStructures;
using MySqlConnector;
using SqlKata.Execution;

namespace Com2usServerCampus
{
    
    public class DBManager

    {
       static string DBConnectString;
       static string RedisConnectString;

        public static RedisConnection RedisConnection { get; set; }
        public static void Init(IConfiguration configuration)
        {
            //var config=new //mysql 커넥션
            DBConnectString = configuration.GetSection("DBConnection")["MySqlGame"];
            RedisConnectString = configuration.GetSection("DBConnection")["Redis"];
            var config = new RedisConfig("basic", RedisConnectString);     //redis 커넥션
            RedisConnection=new RedisConnection(config);
        }

        public static async Task<QueryFactory> GetDBQuery()
        {
            var connection = new MySqlConnection(DBConnectString);
            connection.Open();  //DB 연동

            var compiler = new SqlKata.Compilers.MySqlCompiler();
            var queryFactory=new SqlKata.Execution.QueryFactory(connection,compiler);

            return queryFactory;
        }
    }


}
