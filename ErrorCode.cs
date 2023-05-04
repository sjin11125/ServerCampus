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
        LoginFailNoData=2003,
        LoginFailException=2004,

        //CheckUserAuth 관련 에러코드
        WrongdRequestHttpBody = 3001,
        WrongAuthTokenOrEmail = 3001,
        InvalidAuthToken= 3002,
        AuthTokenFailSetNx=3003,

        //GameData 관련 에러코드
        InsertGameDataDup=4001,
        WrongGameData=4002,


        //ItemData 관련 에러코드
        InsertItemDataFail = 5001,
        EmptyItemData=5002,

        //Token 관련 에러코드
        SetUserTokenFail=6001
        //InvalidAppversion = 3002,
        // InvalidDataversion = 3003,
    }
}
