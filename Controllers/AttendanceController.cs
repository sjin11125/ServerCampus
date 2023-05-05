using CloudStructures;
using CloudStructures.Structures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlKata.Execution;
using ZLogger;
using  Com2usServerCampus.ModelReqRes;
using Com2usServerCampus.Services;

namespace Com2usServerCampus.Controllers;

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
        AttendanceResponse AttendancedResponse = new AttendanceResponse();

        var content = await _gameDB.AttendanceCheck(Attendance.Email);          //출석일수 
        if (content.Item1!=ErrorCode.None)
        {
            AttendancedResponse.Error = content.Item1;
            return AttendancedResponse;
        }

        var reward = await _masterDataDB.GetAttendanceRewardData(content.Item2); //마스터 데이터에서 출석 보상 받아옴
        if (reward.Item1 != ErrorCode.None)
        {
            AttendancedResponse.Error = reward.Item1;
            return AttendancedResponse;
        }
        var itemInfo = await _masterDataDB.GetItemData(content.Item2);          //마스터 데이터에서 해당 보상 아이템의 정보를 받아옴
        if (itemInfo.Item1 != ErrorCode.None)
        {
            AttendancedResponse.Error = itemInfo.Item1;
            return AttendancedResponse;
        }

        var result = await _gameDB.InsertMail(Attendance.Email,new Model.UserItem {             //받아온 출석보상을 사용자 메일 테이블에 추가
            Eamil=Attendance.Email,
            Count=reward.Item2.Count,
            ItemCode=reward.Item2.ItemCode,
            EnhanceCount= 0,
            IsCount=itemInfo.Item2.isCount

        });
        if (result != ErrorCode.None)
        {
            AttendancedResponse.Error = result;
            return AttendancedResponse;
        }

        return AttendancedResponse;
    }
}

