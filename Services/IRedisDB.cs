using Com2usServerCampus.Model;
using System;
using System.Threading.Tasks;


namespace Com2usServerCampus.Services;
    public interface IRedisDB
    {
    public void Init(string connectString);
    public Task<(bool, AuthUser)> GetUserAsync(string email);
    public  Task<bool> SetUserReqLockAsync(string key);
    public Task<bool> DelUserReqLockAsync(string key);

    public Task<List<Notice>> LoadNotice();
    public Task<(ErrorCode, int, int)> GetUserStageItem(string userId, int itemCode, int stageCode);

    public Task<ErrorCode> SetUserStageItem(string userId, int itemCode, int stageNum,int itemCount, int index);

    public Task<(ErrorCode, int, int)> GetUserStageNPC(string userId, int npcCode, int stageCode);
    public  Task<ErrorCode> SetUserStageNPC(string userId, int npcCode, int stageCode, int npcCount, int index);

    public Task<ErrorCode> SetUserToken(string email, string token, int accountId);

    public  Task<ErrorCode> DeleteUserStageItemData(string userId, int stageCode);
    public Task<ErrorCode> DeleteUserStageNPCData(string userId, int stageCode);

    public  Task<(ErrorCode, List<AcquireStageItem>)> GetAllUserStageItem(string userId, int stageCode);
    public  Task<(ErrorCode, List<KillStageNPC>)> GetAllUserStageNPC(string userId, int stageCode);
    



}

