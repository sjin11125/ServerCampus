using Com2usServerCampus.Model;
using Microsoft.Extensions.Options;
using System.Data;
using MySqlConnector;
using SqlKata.Execution;
using SqlKata;
using System.Dynamic;
using System.Collections.Generic;

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

    public async Task<ErrorCode> InsertGameData(string email, UserInfo userInfo)        //유저 게임 정보 넣기
    {
        userInfo.Attendance = DateTime.Today;
      var count=  await queryFactory.Query("gamedata").InsertAsync(new 
        {
           userInfo.Email,
            userInfo.Exp,
            userInfo.Attack,
            userInfo.Defense,
          userInfo.Attendance,
          userInfo.AttendanceCount
      });

        if (count != 1)       //실패
        {
            return ErrorCode.InsertGameDataDup;
        }
        return ErrorCode.None;
    }

    public async Task<ErrorCode> InsertItem(string email, UserItem userItem)        //게임 아이템 넣기
    {
        int result = 0;
        if (userItem.IsCount)       //겹치기가 가능한가
        {
            var count = await queryFactory.Query("itemdata").Where("Email", email).Where("ItemCode", userItem.ItemCode).Select("ItemCount").FirstOrDefaultAsync<int>();  //아이템 있는지 확인

            
            if (count == 0)//없으면 걍 넣고
            {
                result = await queryFactory.Query("itemdata").InsertAsync(new
                {
                    email,
                    userItem.ItemCode,
                    userItem.EnhanceCount,
                    userItem.ItemCount
                });
            }
            else //있으면 기존 갯수 + 새로 들어온 갯수 업데이트
            {
                int ItemCount = userItem.ItemCount + count;
                result = await queryFactory.Query("itemdata").Where("ItemCode", userItem.ItemCode).UpdateAsync(new { ItemCount = ItemCount });
                    
                
            }
        }
        else                            //겹치기가 안되면 걍 넣음
        {
             result = await queryFactory.Query("itemdata").InsertAsync(new
            {
                email,
                userItem.ItemCode,
                userItem.EnhanceCount,
                userItem.ItemCount
            });
        }
        if (result!=1)  //실패    
        {
            return ErrorCode.InsertItemDataFail;
        }
        else return ErrorCode.None;
    }
    public async Task<ErrorCode> UpdateItem(string email, UserItem userItem)        //게임 아이템 업데이트
    { 
        var   result = await queryFactory.Query("itemdata").Where("ItemCode", userItem.ItemCode).Where("Id",userItem.Id).UpdateAsync(new {

            Eamil = userItem.Eamil,
            ItemCode = userItem.ItemCode,
            Id = userItem.Id,
            EnhanceCount = userItem.EnhanceCount + 1,
            ItemCount = userItem.ItemCount,
            Attack = userItem.Attack,
            Defence = userItem.Defence,
            Magic = userItem.Magic


        }); //(성공 1, 실패 0)
        if (result!=1)
            return ErrorCode.UpdateItemDataFail;

        return ErrorCode.None;

    }
    public async Task<ErrorCode> DeleteItem(string email, int id)        //게임 아이템 지우기
    {
        var result = await queryFactory.Query("itemdata").Where("Id", id).DeleteAsync(); //(성공 1, 실패 0)
        if (result != 1)
            return ErrorCode.DeleteItemDataFail;

        return ErrorCode.None;

    }
    public  async Task<(ErrorCode,UserInfo)> GetGameData(string email)       //유저 게임 데이터 불러오기
    {
        UserInfo userInfo = await queryFactory.Query("gamedata").Where("Email", email).FirstOrDefaultAsync<UserInfo>();
        if (userInfo==null)             //유저의 게임정보가 없다면
            return ( ErrorCode.WrongGameData,null);
        else 
            return (ErrorCode.None, userInfo);
    }


   public async Task<(ErrorCode,List<UserItem>)> GetAllItems(string email)         //유저 아이템 모두 불러오기
    {
        var items =await queryFactory.Query("itemdata").Where("Email", email).GetAsync<UserItem>();

        if (items.Count()==0)       //가지고 있는 아이템이 하나도 없다면
            return (ErrorCode.InvalidItemData, null);

        return (ErrorCode.None, items.ToList());
    }
    public async Task<(ErrorCode, UserItem)> GetItem(string email, int id) //유저 아이템  불러오기
    {
        var items = await queryFactory.Query("itemdata").Where("Email", email).Where("Id",id).FirstOrDefaultAsync<UserItem>();

        if (items is null)       //해당 아이템이 없다면
            return (ErrorCode.InvalidItemData, null);

        return (ErrorCode.None, items);

    }

    
    public async Task<(ErrorCode,List<Mail>)> GetMails(string email,int page)           //유저 메일 불러오기
    {

     var mailInfos=await queryFactory.Query("mail").WhereRaw("Time +  ExpiryTime> NOW()").Where("isRead", false).Select("Id","Title").PaginateAsync<Mail>(page, 20);
        //유효기간 지난 메일과 읽은 메일은 불러오지 않고 메일ID와 제목만 불러오기
        if (mailInfos.List.Count()==0)
        {
            return (ErrorCode.EmptyMail, null);
        }
        foreach (var mail in mailInfos.List)
        {
            var mailitems = await queryFactory.Query("mailitem").Where("Email", email).Where("Id", mail.Id).GetAsync<MailItem>();

            if (mailitems.Count()==0)      //메일의 아이템이 없으면
                continue;

            mail.Items=mailitems.ToList();
        }
        return (ErrorCode.None, mailInfos.List.ToList());
    }

    public async Task<(ErrorCode, string )> ReadMail(string email, int id)      //메일 읽기
    {
        var content = await queryFactory.Query("mail").Where("Email", email).Where("Id", id).Select("Content").FirstOrDefaultAsync<string>();

        if (content is null)
        {
            return (ErrorCode.EmptyMailContent, null);
        }
        
        await queryFactory.Query("mail").Where("Email", email).Where("Id", id).UpdateAsync(new { isRead=true});     //읽음 처리함
        return (ErrorCode.None, content);

    }

    public async Task<(ErrorCode,List<MailItem>)> GetMailItem(string email,int id)      //유저 메일 아이템 정보 받기
    {
        var items = await queryFactory.Query("mailitem").Where("Email", email).Where("Id", id).GetAsync<MailItem>();
        if (items is null)
            return (ErrorCode.EmptyMailItem,null);

        return (ErrorCode.None, items.ToList());

    }
    public async Task<ErrorCode> UpdateMailItem(string email,int id)                //유저 메일 아이템 받기 처리
    {
        var result = await queryFactory.Query("mail").Where("Email", email).Where("Id", id).Select("isGet").FirstOrDefaultAsync<string>();

        if (result is null)
            return ErrorCode.GetMailItemFail;

        await queryFactory.Query("mail").Where("Email", email).Where("Id", id).UpdateAsync(new { isGet = true });     //받음 처리함

        return ErrorCode.None;

    }
    public async Task<ErrorCode> InsertMail(string email, List<UserItem> items, MailType type)        //유저에게 메일 전송
    {
        string title, content;
        switch (type)
        {
            case MailType.AttendanceReward:
                title = "출석 보상";
                content = "출석 보상입니다리미";
                break;
            case MailType.InAppPurchase:
                title = "인앱 결제 상품";
                content = "인앱 결제 상품입니다리미";
                break;
            default:
                title = "";
                content = "";
                break;
        }


        var result = await queryFactory.Query("mail").InsertGetIdAsync<int>(new
        {               //메일 내용 넣고 메일Id 불러오기
            Email = email,
            Title = title,
            Content = content,
            Time = DateTime.Today,
            ExpiryTime = 7,
            isRead = false,
            isGet = false,
        });
        var item = await InsertMailItems(email, items, result); //메일 아이템 테이블에 아이템 넣기

        if (item != ErrorCode.None)
            return item;

        return ErrorCode.None;

    }
    public async Task<ErrorCode> InsertMailItems(string email, List<UserItem> items, int id) //메일 아이템 테이블에 아이템 넣기
    {
        if (items.Count == 0) return ErrorCode.EmptyMailItemInfo;

        foreach (var item in items)
        {
            var insertMail = await queryFactory.Query("mailitem").InsertAsync(new  //메일 아이템 테이블에 아이템 넣기(성공 1, 실패 0)
            {
                Email = email,
                Id = id,
                Code = item.ItemCode,
                Count = item.ItemCount
            });

            if (insertMail != 1)
                return ErrorCode.ErrorInsertMail;
        }
        return ErrorCode.None;

    }
    public async Task<(ErrorCode, int)> AttendanceCheck(string email)          //출석 확인
    {
        var result = await queryFactory.Query("gamedata").Where("Email", email).Select("Attendance", "AttendanceCount").FirstOrDefaultAsync<AttendanceInfo>();     //사용자 게임정보 중 날짜를 불러옴

        if (result is null)
            return (ErrorCode.InvalidAttendance, 0);

        int day = (int)(DateTime.Today - result.Attendance).TotalDays;//(오늘 날짜 - 마지막 출석 날짜)

        if (day != 1)     //연속으로 출석하지 않으면
        {
           await queryFactory.Query("gamedata").Where("Email", email).UpdateAsync(new { AttendanceCount = 1, Attendance = DateTime.Today });
            //마지막으로 출석한 날짜 초기화, 1일부터 다시시작 
         
            return (ErrorCode.None, 1);
        }
        else
        {
            await queryFactory.Query("gamedata").Where("Email", email).UpdateAsync(new { AttendanceCount = result.AttendanceCount+1, Attendance = DateTime.Today });
            //마지막으로 출석한 날짜 초기화, 출석일수 업데이트
            return (ErrorCode.None, result.AttendanceCount); //(오늘 날짜 - DB 출석 날짜)+1 한 결과를 컨트롤러에 전달
        } 
    }

    public async Task<ErrorCode> CheckDuplicateReceipt(string id)           //영수증 중복 검사
    {
        var result = await queryFactory.Query("inapppurchasereceipt").Where("Id", id).Limit(1).FirstOrDefault();
        if (result is null)     //중복 되지 않음
            return ErrorCode.None;
        else                // 중복됨
            return ErrorCode.InAppPurchaseFailDup;
    }  public async Task<ErrorCode> InsertitemInfo(string email,UserItem itemInfo)           //강화 단계 이력 정보 추가
    {
        var result = await queryFactory.Query("enhance").InsertAsync(new  //강화 단계 이력 테이블에 강화 정보 넣기(성공 1, 실패 0)
        {
            Email = email,
            Id = itemInfo.Id,
            ItemCode = itemInfo.ItemCode,
            EnhanceCount = itemInfo.EnhanceCount,
            Attack = itemInfo.Attack,
            Defence = itemInfo.Defence,
            Magic = itemInfo.Magic
        }) ;
        if (result !=1)     //실패하면
            return ErrorCode.InsertEnhanceInfoFail;
        else                // 중복됨
            return ErrorCode.None;
    }
}

