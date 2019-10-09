namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 客账界面中左侧显示的预订明细信息模型
    /// </summary>
    public class ResFolioDetailInfo
    {
        public string RegId { get; set; }
        public string RoomNo { get; set; }
        public string GuestName { get; set; }
        public string CheckInDate { get; set; }
        public decimal? DebitAmount { get; set; }
        public decimal ?CreditAmount { get; set; }
        public string StatuName { get; set; }
        public int? Days { get; set; }
        public int? AllRoomQty { get; set; }
        public int? IRoomQty { get; set; }
        public int? ORoomQty { get; set; }
        public int? IsCardAuth { get; set; }
        public decimal? CardAuthAmount { get; set; }
    }
}
