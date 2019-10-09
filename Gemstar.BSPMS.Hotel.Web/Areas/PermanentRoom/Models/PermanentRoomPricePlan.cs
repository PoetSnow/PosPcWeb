using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PermanentRoom.Models
{
    public class PermanentRoomPricePlan
    {
        public string Hid { get; set; }

        public string Roomid { get; set; }

        [Display(Name = "月租")]
        public decimal? RoomPriceByMonth { get; set; }

        [Display(Name = "日租")]
        public decimal? RoomPriceByDay { get; set; }


        public string FloorName { get; set; }
        public string RoomNo { get; set; }
        public string Feature { get; set; }
        public string Description { get; set; }
    }
}