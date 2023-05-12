using Com2usServerCampus.Model;
using Com2usServerCampus.ModelReqRes;

namespace Com2usServerCampus.Services
{
    public interface IGameDB
    {
        public Task<(ErrorCode, UserInfo)> GetGameData(string userId);
        public Task<(ErrorCode, List<UserItem>)> GetAllItems(string userId);
        public Task<(ErrorCode, UserItem)> GetItem( int itemId);
        public Task<ErrorCode> InsertItem(bool isCount, UserItem useritem);
        public Task<ErrorCode> UpdateItem(UserItem userItem);
        public Task<ErrorCode> DeleteItem( int itemId);

        public Task<ErrorCode> InsertGameData(UserInfo userInfo);
        public Task<(ErrorCode, List<Mail>)> GetMails(string userId,int page);
        public Task<(ErrorCode, string)> ReadMail(int mailId);
        public Task<(ErrorCode, List<MailItem>)> GetMailItem( int mailId);
        public Task<ErrorCode> ReceiveMailItem(int mailId);
        public Task<(ErrorCode, int)> Attendance(string userId);
        public Task<ErrorCode> InsertMail(string userId,List<UserItem> items, MailType type);
        public  Task<ErrorCode> CheckDuplicateReceipt(InAppPurchaseRequest info);
        public Task<ErrorCode> InsertEnhanceInfo(EnhanceItemInfo enhanceInfo);

        public  Task<(ErrorCode, int)> GetUserStageInfo(string userId);



    }

}
