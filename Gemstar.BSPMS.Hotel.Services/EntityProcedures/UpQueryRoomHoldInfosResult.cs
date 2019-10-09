using System;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 查询渠道保留房设置信息结果集
    /// 存储过程up_queryRoomHoldInfos的结果集
    /// </summary>
    public class UpQueryRoomHoldInfosResult
    {
        public string RoomTypeid { get; set; }
        public string RoomTypeName { get; set; }
        public DateTime HoldDate { get; set; }
        public int TotalRooms { get; set; }
        public int SettingRoomQty { get; set; }
        public int AvailbleRoomQty { get; set; }
    }

    public class UpQueryRoomHoldInfosResultForshow
    {
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public int TotalRooms { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day01 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day02 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day03 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day04 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day05 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day06 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day07 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day08 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day09 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day10 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day11 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day12 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day13 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day14 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day15 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day16 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day17 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day18 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day19 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day20 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day21 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day22 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day23 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day24 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day25 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day26 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day27 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day28 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day29 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day30 { get; set; }
        public UpQueryRoomHoldInfosResultQtyForshow Day31 { get; set; }
    }
    public class UpQueryRoomHoldInfosResultQtyForshow
    {
        public int SettingRoomQty { get; set; }
        public int AvailbleRoomQty { get; set; }
    }
}
