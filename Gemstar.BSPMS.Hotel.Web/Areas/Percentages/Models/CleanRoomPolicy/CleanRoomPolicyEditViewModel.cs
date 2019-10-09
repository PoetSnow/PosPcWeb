using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Percentages.Models.CleanRoomPolicy
{
    public class CleanRoomPolicyEditViewModel
    {
        
        public string RoomTypeId { get; set; }

        [Display(Name = "续住打扫价格")]
        public decimal? ContinuedToLivePrice { get; set; }

        [Display(Name = "离店打扫价格")]
        public decimal? CheckOutPrice { get; set; }

        [Display(Name = "政策说明")]
        public string PolicyDesciption { get; set; }
    }

    public class CleanRoomPolicyListViewModel : CleanRoomPolicyEditViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public decimal? Price { get; set; }
        public byte? Count { get; set; }
    }

}