using CloudStructures;
using CloudStructures.Structures;
using Com2usServerCampus.Controllers;
using MySqlConnector;
using SqlKata.Execution;

namespace Com2usServerCampus
{
    
    public class DBManager

    {
       static string AccountDBConnectString;
        static string GameDBConnectString;
       static string RedisConnectString;
        MySqlConnection connection;
        MySqlConnection connectionGame;

        public static RedisConnection RedisConnection { get; set; }
        public async static void Init(IConfiguration configuration)
        {
            //var config=new //mysql 커넥션
            AccountDBConnectString = configuration.GetSection("DBConnection")["MySqlAccount"];      //유저 데이터베이스 연결 스트링
            GameDBConnectString = configuration.GetSection("DBConnection")["MySqlGame"];        //게임 데이터베이스 연결 스트링
            RedisConnectString = configuration.GetSection("DBConnection")["Redis"];
            var config = new RedisConfig("basic", RedisConnectString);     //redis 커넥션
            RedisConnection=new RedisConnection(config);

        }

        public  async Task<QueryFactory> GetDBQuery()
        {
            connection = new MySqlConnection(AccountDBConnectString);
            await connection.OpenAsync();  //DB 연동


            var compiler = new SqlKata.Compilers.MySqlCompiler();
            var queryFactory=new SqlKata.Execution.QueryFactory(connection,compiler);
            
            return queryFactory;
        }

        public  async Task<QueryFactory> GetGameDBQuery()
        {
            connectionGame = new MySqlConnection(GameDBConnectString);
            await connectionGame.OpenAsync();  //DB 연동


            var compiler = new SqlKata.Compilers.MySqlCompiler();
            var queryFactory = new SqlKata.Execution.QueryFactory(connectionGame, compiler);

            return queryFactory;
        }

        public async void CloseDB()
        {
            await connection.CloseAsync();
        }
        public async void CloseGameDB()
        {
            await connectionGame.CloseAsync();
        }
    }


}
