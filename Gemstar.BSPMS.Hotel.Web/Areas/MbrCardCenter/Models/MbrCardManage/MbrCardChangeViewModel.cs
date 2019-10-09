using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    public class MbrCardChangeViewModel
    {
        public Guid ID { get; set; }

        [Display(Name = "原会员卡号")]
        public string OldCardNo { get; set; }

        [Display(Name = "新会员卡号")]
        [Required(ErrorMessage = "请输入新会员卡号")]
        public string NewCardNo { get; set; }

        [Display(Name = "备注")]
        [Required(ErrorMessage = "请输入换卡备注")]
        public string Remark { get; set; }
    }
}