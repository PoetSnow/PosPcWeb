using System;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Models.ResOrderBatch
{
    /// <summary>
    /// 房间查询参数
    /// </summary>
    public class RoomQueryPara
    {
        public int isCheckIn { get; set; }
        public string roomTypeId { get; set; }
        public DateTime? arrDate { get; set; }
        public DateTime? depDate { get; set; }
    }
}