using CloudStructures;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;
using  Com2usServerCampus.ModelReqRes;
using  Com2usServerCampus.Model;
using Com2usServerCampus.Services;

namespace Com2usServerCampus.Controllers;
[ApiController]
[Route("[controller]")]
public class AttendanceController : ControllerBase
{
    ILogger<AttendanceController> _logger;
    IGameDB _gameDB;
    IMasterDataDB _masterDataDB;

    public AttendanceController(ILogger<AttendanceController> logger, IGameDB gameDB, IMasterDataDB masterDataDB)
    {
        _logger = logger;
        _gameDB = gameDB;
        _masterDataDB = masterDataDB;
    }

    [HttpPost]
    public async Task<AttendanceResponse> AttendancePost(AttendanceRequest Attendance)
    {
        var userInfo = (AuthUser)HttpContext.Items[nameof(AuthUser)]!;

        AttendanceResponse AttendancedResponse = new AttendanceResponse();


        (var attendanceError,var content) = await _gameDB.Attendance(userInfo.Email);          //출석체크 하기
        if (attendanceError != ErrorCode.None)
        {
            AttendancedResponse.Error = attendanceError;
            return AttendancedResponse;
        }


        (var error, var reward, var itemInfo) =await GetData(content);
        if (error != ErrorCode.None)
        {
            AttendancedResponse.Error = error;
            return AttendancedResponse;
        }

        List<UserItem> UserItems = new List<UserItem>() { new UserItem {             //받아온 출석보상을 사용자 메일 테이블에 추가
            Eamil=Attendance.Email,
            ItemCount=reward.Count,
            ItemCode=reward.ItemCode,
            EnhanceCount= 0,

        } };

        var result = await _gameDB.InsertMail(Attendance.Email,UserItems,MailType.AttendanceReward);
        if (result != ErrorCode.None)
        {
            AttendancedResponse.Error = result;
            return AttendancedResponse;
        }

        return AttendancedResponse;
    }
    public async Task<(ErrorCode,AttendanceReward,ItemData)> GetData(int itemCode)
    {
        (var rewardError, var reward) = _masterDataDB.GetAttendanceRewardData(itemCode); //마스터 데이터에서 출석 보상 받아옴
        if (rewardError != ErrorCode.None)
        {
            return (rewardError,null,null);
        }


        (var itemInfoError, var itemInfo) = _masterDataDB.GetItemData(itemCode);          //마스터 데이터에서 해당 보상 아이템의 정보를 받아옴
        if (itemInfoError != ErrorCode.None)
        {
            return (itemInfoError, null, null);
        }
        return (ErrorCode.None, reward, itemInfo);

    }
}

