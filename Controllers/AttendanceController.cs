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


        var content = await _gameDB.Attendance(Attendance.Email);          //출석체크 하기
        if (content.Item1!=ErrorCode.None)
        {
            AttendancedResponse.Error = content.Item1;
            return AttendancedResponse;
        }


        (var error,var reward )= _masterDataDB.GetAttendanceRewardData(content.Item2); //마스터 데이터에서 출석 보상 받아옴
        if (error != ErrorCode.None)
        {
            AttendancedResponse.Error = error;
            return AttendancedResponse;
        }


        var itemInfo = _masterDataDB.GetItemData(content.Item2);          //마스터 데이터에서 해당 보상 아이템의 정보를 받아옴
        if (itemInfo.Item1 != ErrorCode.None)
        {
            AttendancedResponse.Error = itemInfo.Item1;
            return AttendancedResponse;
        }


        List<UserItem> UserItems = new List<UserItem>() { new UserItem {             //받아온 출석보상을 사용자 메일 테이블에 추가
            Eamil=Attendance.Email,
            ItemCount=reward.Count,
            ItemCode=reward.ItemCode,
            EnhanceCount= 0,
            IsCount=itemInfo.Item2.isCount

        } };

        var result = await _gameDB.InsertMail(Attendance.Email,UserItems,MailType.AttendanceReward);
        if (result != ErrorCode.None)
        {
            AttendancedResponse.Error = result;
            return AttendancedResponse;
        }

        return AttendancedResponse;
    }
}

