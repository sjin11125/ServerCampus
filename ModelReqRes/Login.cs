using System.ComponentModel.DataAnnotations;
using Com2usServerCampus.Model;

namespace Com2usServerCampus.ModelReqRes;
public class LoginAccountRequest //유저가 서버에게 주는 아이디, 비번 데이터 클래스
{
    [Required]
    [MinLength(1, ErrorMessage = "UserId cannot be empty")]
    [MaxLength(50, ErrorMessage = "UserId is so long")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required]
    [MinLength(1, ErrorMessage = "Password cannot be empty")]
    [MaxLength(50, ErrorMessage = "Password is so long")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}

public class LoginAccountResponse //서버가 유저에게 주는 응답 클래스
{
    public ErrorCode Error { get; set; }
    public UserInfo userInfo { get; set; }
    public List<UserItem> itemList { get; set; }
    public List<Notice> NoticeList { get; set; }
    public string Authtoken { get; set; }
}