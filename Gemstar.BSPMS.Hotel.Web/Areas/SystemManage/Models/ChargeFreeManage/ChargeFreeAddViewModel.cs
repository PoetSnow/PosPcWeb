using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.ChargeFreeManage
{
    public class ChargeFreeAddViewModel
    {
        [Display(Name = "会员卡类型")]
        [Required(ErrorMessage = "请选择会员卡类型")]
        public string MbrCardTypeid { get; set; }

        [Display(Name = "充值金额起始值")]
        [Required(ErrorMessage = "请输入充值金额起始值")]
        public decimal BeginAmount { get; set; }

        [Display(Name = "充值金额结束值")]
        [Required(ErrorMessage = "请输入充值金额结束值")]
        public decimal EndAmount { get; set; }

        [Display(Name = "赠送金额")]
        public decimal? Amount { get; set; }

        [Display(Name = "赠送比例")]
        public decimal? Rate { get; set; }
    }
}
