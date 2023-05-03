using System;
using System.ComponentModel.DataAnnotations;

namespace Com2usServerCampus.ModelReqRes;
public class CreateAccountRequest //유저가 서버에게 주는 아이디, 비번 데이터 클래스
{
    [Required]
    [MinLength(1, ErrorMessage = "Email cannot be empty")]
    [MaxLength(50, ErrorMessage = "Email is so long")]
    [DataType(DataType.EmailAddress)]

    //[RegularExpression("^[a-zA-Z0-9")]
    //https://learn.microsoft.com/ko-kr/dotnet/api/system.componentmodel.dataannotations.regularexpressionattribute?view=net-7.0
    public string Email { get; set; }
    [Required]
    [MinLength(1, ErrorMessage = "Password cannot be empty")]
    [MaxLength(50, ErrorMessage = "Password is so long")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
public class CreateAccountResponse //서버가 유저에게 주는 응답 클래스
{
    public ErrorCode Error { get; set; } = ErrorCode.None;

}
