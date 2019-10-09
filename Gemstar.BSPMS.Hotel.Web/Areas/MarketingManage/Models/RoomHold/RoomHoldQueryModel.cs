using System;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.RoomHold
{
    public class RoomHoldQueryModel
    {
        public string ChannelId { get; set; }
        public DateTime? BeginDate { get; set; }
        public int? Days { get; set; }
    }
}