namespace Com2usServerCampus
{
    public enum ErrorCode : int
    {
        None = 0,

        //CreateAccount 관련 에러코드
        CreateAccount_Fail_Dup = 1001, 
        CreateAccount_Fail_Exception = 1002,

        //Login 관련 에러코드
        Login_Fail_Email = 2001,
        Login_Fail_Password = 2002,

        //CheckUserAuth 관련 에러코드
        InvalidRequestHttpBody = 3001,
        InvalidAppversion = 3002,
        InvalidDataversion = 3003,
    }
}
