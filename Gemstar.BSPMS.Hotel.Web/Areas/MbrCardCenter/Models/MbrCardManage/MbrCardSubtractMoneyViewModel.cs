using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    public class MbrCardSubtractMoneyViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "错误信息，请关闭后再重试")]
        public Guid Id { get; set; }

        [Display(Name = "会员卡号")]
        public string MbrCardNo { get; set; }

        [Display(Name = "会员姓名")]
        public string GuestName { get; set; }

        [Display(Name = "当前余额")]
        public decimal? Balance { get; set; }

        [Display(Name = "当前赠送余额")]
        public decimal? SendBalance { get; set; }

        [Display(Name = "扣款类型")]
        [Required(ErrorMessage = "请选择扣款类型")]
        public string AccountType { get; set; }

        [Display(Name = "扣款金额")]
        [Required(ErrorMessage = "请输入扣款金额")]
        public decimal Money { get; set; }

        [Display(Name = "单号")]
        public string InvNo { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}