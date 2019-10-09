using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    public class MbrCardChangeScoreViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "错误信息，请关闭后再重试")]
        public Guid Id { get; set; }

        [Display(Name = "会员卡号")]
        public string MbrCardNo { get; set; }

        [Display(Name = "会员姓名")]
        public string GuestName { get; set; }

        [Display(Name = "当前积分")]
        public int? CurrentScore { get; set; }

        [Display(Name = "当前业主分")]
        public int? CurrentOwnerScore { get; set; }

        [Display(Name = "账户类型")]
        [Required(ErrorMessage = "请选择账户类型")]
        public string AccountType { get; set; }

        [Display(Name = "调整分值")]
        [Required(ErrorMessage = "请输入调整数额，正数调增，负数调减")]
        public decimal Score { get; set; }

        [Display(Name = "单号")]
        public string InvNo { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}