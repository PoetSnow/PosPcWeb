namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 房态中心房态表存储过程结果
    /// </summary>
    public class UpQueryRoomStatusInfosByRoomTypeResult
    {
        public string RoomTypeId { get; set; }
        public string RoomTypeCode { get; set; }
        public string RoomTypeName { get; set; }
        public int? TotalRoomsQty { get; set; }
        public int? InRoomsQty { get; set; }
        public int? ArrRoomsQty { get; set; }
        public int? DepRoomsQty { get; set; }
        public int? ServiceRoomsQty { get; set; }
        public int? StopRoomsQty { get; set; }
        public int? HoldRoomsQty { get; set; }
        public int? AvailableToSellQty { get; set; }
        public int? AvailableToUseQty { get; set; }
        public int? AvailableToUseIncludeDepQty { get; set; }
        public string RentalRate { get; set; }
        public string RentalRates { get; set; }
        public decimal? RoomRate { get; set; }
    }
}
