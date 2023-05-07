namespace Com2usServerCampus.Model;
public class DBUserInfo             //AccountDB에 있는 유저 계정 정보
{
    public int AccountId { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }

    public ErrorCode Error { get; set; }
}
public class AuthUser           //레디스에 저장되어있는 유저 계정 정보
{
    public string Email { get; set; }
    public string AuthToken { get; set; }  
    public Int64 AccountId { get; set; }    
    public string State { get; set; }
}

public class RedisKeyExpireTime
{
    public const ushort NxKeyExpireSecond = 3;
    public const ushort RedisKeyExpireSecond = 6000;        //1일
}