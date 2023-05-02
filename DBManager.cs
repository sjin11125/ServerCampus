﻿using CloudStructures;
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

        string CurrentAppVersion="v1.00.0";
        string CurrentDataVersion = "v1.00.0";

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
        public async Task<DBUserInfo> CheckUser(string email)            //유저 정보가 있는지
        {
            using (var db = GetDBQuery())
            {
                var userCode = await db.Result.Query("account").Where("Email", email).FirstOrDefaultAsync<DBUserInfo>();
                CloseDB();
                return userCode;
            }
        }
        public async Task<long> AddUser(string email, string hashedPassword)
        {
            using (var db = GetDBQuery())
            {
                var AccountId = await db.Result.Query("account").InsertGetIdAsync<Int64>(new
                {          //account 테이블에 이메일, 비번 넣기
                    email,
                    hashedPassword,
                    CurrentAppVersion,
                    CurrentDataVersion
                });
                CloseDB();
                return AccountId;
            }
        }

        public async Task InsertItem(string email, UserItem userItem)           //아이템 넣기
        {
            using (var db = GetGameDBQuery())
            {
                var AccountId = await db.Result.Query("gamedata").InsertAsync(new
                {        
                    email,
                    userItem.ItemCode,
                    userItem.EnhanceCount,
                    userItem.Count
                });
                CloseGameDB();
            }
        }
        public async Task InsertGameData(string email, UserInfo userInfo)
        {
            using (var db = GetGameDBQuery())
            {

                await db.Result.Query("gamedata").InsertAsync(new
                {
                    email,
                    userInfo.Exp,
                    userInfo.Attack,
                    userInfo.Defence,
                });
                CloseGameDB();
            }
        }

        public async Task<UserInfo> GetGameData(string email)
        {
            using (var gamedb = GetGameDBQuery())
            {
               var userInfo= await gamedb.Result.Query("gamedata").Where("Email", email).FirstOrDefaultAsync<UserInfo>();
                CloseGameDB();
                return userInfo;
            }
        }
        public async Task<IEnumerable<UserItem>> GetItems(string email)
        {
            using (var gamedb = GetGameDBQuery())
            {
                var userItems = await gamedb.Result.Query("itemdata").Where("Email", email).GetAsync<UserItem>();
                CloseGameDB();
                return userItems;
            }
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
