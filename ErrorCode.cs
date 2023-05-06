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
        WrongAuthTokenOrEmail = 3002,
        InvalidAuthToken= 3003,
        AuthTokenFailSetNx=3004,

        //GameData 관련 에러코드
        InsertGameDataDup=4001,
        WrongGameData=4002,


        //ItemData 관련 에러코드
        InsertItemDataFail = 5001,

        //Token 관련 에러코드
        SetUserTokenFail=6001,

        //Mail 관련 에러코드
        EmptyMail=7001,
        EmptyMailContent=7002,
        EmptyMailItem=7003,
        GetMailItemFail=7004,
        ErrorInsertMail=7005,
        InvalidMailType=7006,
        EmptyMailItemInfo=7007,

        //Attendance 관련 에러코드
        InvalidAttendance=8001,
        ErrorAttendanceInit=8002,

        //마스터 데이터 관련 에러코드
        InvalidItemData=9001,

        //인앱 결제 영수증 관련 에러코드
        InAppPurchaseFailDup=10001,


        //InvalidAppversion = 3002,
        // InvalidDataversion = 3003,
    }
}
