using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.RoomOwnerRoomInfos
{
    public class RoomOwnerRoomInfosAddViewModel
    {
        [Key] 
        [Display(Name = "业主房间信息id")]
        public Guid RoomInfoId { get; set; }
         
        [Display(Name = "酒店id")]
        public string Hid { get; set; }
         
        [Display(Name = "业主的id值")]
        public Guid ProfileId { get; set; } 

        [Display(Name = "房间id")]
        public string RoomId { get; set; }
         
        [Display(Name = "房号")]
        public string RoomNo { get; set; }

        [Display(Name = "生效日期")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "失效日期")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "分成类型")]
        public Guid CalcTypeId { get; set; }

        [Display(Name = "")]
        public string r1 { get; set; }

        [Display(Name = "")]
        public string r2 { get; set; }

        [Display(Name = "")]
        public string r3 { get; set; }

        [Display(Name = "")]
        public string r4 { get; set; }

        [Display(Name = "")]
        public string r5 { get; set; }

        [Display(Name = "")]
        public string r6 { get; set; }

        [Display(Name = "")]
        public string r7 { get; set; }

        [Display(Name = "")]
        public string r8 { get; set; }

        [Display(Name = "")]
        public string Remark { get; set; }
    }
}