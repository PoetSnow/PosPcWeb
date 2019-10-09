using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoomManage
{
    public class RoomAddViewModel
    {
        [Display(Name = "房号")]
        [Required(ErrorMessage = "请输入房间号")]
        public string RoomNo { get; set; }

        [Display(Name = "房间描述")]
        public string Description { get; set; }

        [Display(Name = "房间特色")]
        public string Feature { get; set; }

        [Display(Name = "房间类型")]
        [Required(ErrorMessage = "请选择房间类型")]
        public string RoomTypeid { get; set; }

        [Display(Name = "分机号")]
        public string Tel { get; set; }

        [Display(Name = "门锁号")]
        public string Lockid { get; set; }

        [Display(Name = "门锁接口信息")]
        public string LockInfo { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "楼层ID")]
        [Required(ErrorMessage = "请选择楼层")]
        public string Floorid { get; set; }

    }
}
