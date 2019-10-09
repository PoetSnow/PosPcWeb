using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    public class MbrCardGiveCouponViewModel
    {
        [Display(Name = "会员卡号")]
        public string MbrCardNo { get; set; }

        [Display(Name = "券类型")]
        [Required(ErrorMessage = "请选择优惠券类型")]
        public string TicketTypeid { get; set; }

        [Display(Name = "券数量")]
        [Required(ErrorMessage = "请输入数量")]
        public string Number { get; set; }

        [Display(Name = "备注")]
        public string Remarks { get; set; }
    }
}