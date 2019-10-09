using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.CouponManage
{
    public class CouponEditViewModel: BaseEditViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "请输入项目代码")]
        [Display(Name = "项目代码")]
        [Column(TypeName = "varchar")]
        public string Code { get; set; }

        [Display(Name = "项目名称")]
        [Required(ErrorMessage = "请输入项目名称")]
        public string Name { get; set; }

        [Display(Name = "优惠券类型ID")]
        public string ItemTypeid { get; set; }

        [Display(Name = "优惠券类型名")]
        public string ItemTypeName { get; set; }

        [Display(Name = "优惠券金额")]
        public decimal? CouponMoney { get; set; }

        [Display(Name = "有效时长")]
        public int? ValidDate { get; set; }

        [Display(Name = "适用房型")]
        public string RoomTypeids { get; set; }

        [Display(Name = "优惠券说明")]
        public string Remark { get; set; }

        [Display(Name = "排序号")]
        public int? Seqid { get; set; }

    }
}