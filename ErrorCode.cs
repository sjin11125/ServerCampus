﻿namespace Com2usServerCampus
{
    public enum ErrorCode:int
    {
        None=0,
        CreateAccount_Fail_Dup=11, CreateAccount_Fail_Exception=12,
        Login_Fail_Email=16, Login_Fail_Password=17,    
    }
    public enum SuccessCode : int
    {
        CreateAccount_Success=0,
        Login_Success=1,
    }
}