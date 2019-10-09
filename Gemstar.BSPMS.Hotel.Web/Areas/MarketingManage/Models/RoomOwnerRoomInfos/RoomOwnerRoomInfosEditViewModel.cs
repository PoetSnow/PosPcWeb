using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.RoomOwnerRoomInfos
{
    public class RoomOwnerRoomInfosEditViewModel : BaseEditViewModel
    {
        [Key]
        [Required(ErrorMessage = "请输入业主房间信息")]
        [Display(Name = "业主房间信息id")]
        public Guid RoomInfoId { get; set; }

        [Required(ErrorMessage = "请输入酒店id")]
        [Display(Name = "酒店id")]
        public string Hid { get; set; }

        [Required(ErrorMessage = "请输入业主的id值")]
        [Display(Name = "业主的id值")]
        public Guid ProfileId { get; set; }

        [Required(ErrorMessage = "请输入房间id")]
        [Display(Name = "房间id")]
        public string RoomId { get; set; }

        [Required(ErrorMessage = "请输入房号")]
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