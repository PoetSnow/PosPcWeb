using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Web.Models;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoomManage
{
    public class RoomEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public string Id { get; set; }

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
        public string RoomTypeCode { get; set; }
        public string RoomTypeName { get; set; }
        public string RoomTypeShortName { get; set; }

        [Display(Name = "分机号")]
        public string Tel { get; set; }

        [Display(Name = "门锁号")]
        public string Lockid { get; set; }

        [Display(Name = "门锁接口信息")]
        public string LockInfo { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; } 

        [Display(Name = "楼层id")]
        public string Floorid { get; set; } 

    }
}
