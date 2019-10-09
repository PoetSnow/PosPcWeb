using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    public class MbrCardChangeSalesViewModel
    {
        public Guid ID { get; set; }

        [Display(Name = "会员卡号")]
        public string CardNo { get; set; }

        [Display(Name = "原业务员")]
        public string OldSales { get; set; }

        [Display(Name = "新业务员")]
        [Required(ErrorMessage = "请输入新业务员")]
        public string NewSales { get; set; }

        [Display(Name = "备注")]
        [Required(ErrorMessage = "请输入换业务员备注")]
        public string Remark { get; set; }
    }
}