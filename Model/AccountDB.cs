namespace Com2usServerCampus.Model;
    public class DBUserInfo
{
    public int AccountId { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }

    public ErrorCode Error { get; set; }
}
