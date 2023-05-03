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
    }

