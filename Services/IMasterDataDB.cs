using Com2usServerCampus.Model;

namespace Com2usServerCampus.Services
{
    public interface IMasterDataDB
    {
        public Task<ErrorCode> Init();


        public List<ItemData> ItemDataList { get; set; }
        public List<ItemAttribute> ItemAttributeDataList { get; set; }
        public List<AttendanceReward> AttendanceRewardDataList { get; set; }

        public List<InAppProduct> InAppProductDataList { get; set; }
        public (ErrorCode, ItemData) GetItemData(int code);

        public (ErrorCode, AttendanceReward) GetAttendanceRewardData(int code);

        public (ErrorCode, List<InAppProduct>) GetInAppProduct(int code);
        public (ErrorCode, ItemAttribute) GetItemAttributeData(int code);
        public (ErrorCode, List<StageItem>) GetStageItem(int stageCode);
        public (ErrorCode, List<StageNPC>) GetStageNPC(int stageCode);
        public ErrorCode IsExistStageItem(int stageCode, int itemCode);
        public ErrorCode IsExistStageNPC(int stageCode, int npcCode);


    }

}
