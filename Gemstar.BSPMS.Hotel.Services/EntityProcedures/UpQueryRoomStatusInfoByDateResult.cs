namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 远期房态存储过程结果
    /// 按日期查看房态信息
    /// </summary>
    public class UpQueryRoomStatusInfoByDateResult
    {
        public string QtyDate { get; set; }
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public int TotalRooms { get; set; }
        public int BookingQty { get; set; }
        public int ServiceQty { get; set; }
        public int StopQty { get; set; }
        public int QuotaAvailableQty { get; set; }
        public int OtherQty { get; set; }
        public int AvailableQty { get; set; }
        public int LivedQty { get; set; }
    }
    public class UpQueryRoomStatusInfoByDateResultForshow
    {
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public int TotalRooms { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day01 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day02 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day03 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day04 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day05 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day06 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day07 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day08 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day09 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day10 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day11 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day12 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day13 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day14 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day15 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day16 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day17 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day18 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day19 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day20 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day21 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day22 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day23 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day24 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day25 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day26 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day27 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day28 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day29 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day30 { get; set; }
        public UpQueryRoomStatusInfoByDateResultQtyForshow Day31 { get; set; }
    }
    public class UpQueryRoomStatusInfoByDateResultQtyForshow
    {
        public int BookingQty { get; set; }
        public int ServiceQty { get; set; }
        public int StopQty { get; set; }
        public int QuotaAvailableQty { get; set; }
        public int OtherQty { get; set; }
        public int AvailableQty { get; set; }
        /// <summary>
        /// 可超预定数
        /// </summary>
        public int? OverQauntity { get; set; }
        public int LivedQty { get; set; }
        public int DepQty { get; set; }
    }
}
