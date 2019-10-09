using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MbrCardCenter.Models.MbrCardManage
{
    public class MbrCardChangeLevelViewModel
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

        [Display(Name = "会员卡类型")]
        [Required(ErrorMessage = "请选择新会员卡类型")]
        public string MbrCardTypeid { get; set; }

        [Display(Name = "扣除积分")]
        [Required(ErrorMessage = "请输入扣除积分数")]
        public int Score { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}