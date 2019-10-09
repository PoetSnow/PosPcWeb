using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoomManage
{
    public class RoomBatchAddViewModel
    {

        [Display(Name = "房号")]
        [Required(ErrorMessage = "请输入房间号")]
        public string RoomNo { get; set; }

        [Display(Name = "房间类型")]
        [Required(ErrorMessage = "请选择房间类型")]
        public string RoomTypeid { get; set; }

        [Display(Name = "分机号")]
        public string Tel { get; set; }

        [Display(Name = "门锁号")]
        public string Lockid { get; set; }

        [Display(Name = "楼层ID")]
        [Required(ErrorMessage = "请选择楼层")]
        public string Floorid { get; set; }

        [Display(Name = "楼层名")] 
        public string FloorName { get; set; }

        [Display(Name = "房型名称")]
        public string RoomTypeName { get; set; }
    }
}