using System.ComponentModel.DataAnnotations;
using System;
using Gemstar.BSPMS.Hotel.Web.Models;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.ScoreUseRuleManage
{
    public class ScoreUseRuleEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public Guid Id { get; set; }

        [Display(Name = "兑换项目")]
        [Required(ErrorMessage = "请选择兑换项目")]
        public string ItemScoreid { get; set; }

        [Display(Name = "会员卡类型")]
        [Required(ErrorMessage = "请选择会员卡类型")]
        public string MbrCardTypeid { get; set; }

        [Display(Name = "纯积分兑换所需积分")]
        public int? OnlyScore { get; set; }

        [Display(Name = "积分加金额所需积分")]
        public int? ScoreAndAmount { get; set; }

        [Display(Name = "积分加金额所需金额")]
        public int? AmountAndScore { get; set; }

        [Display(Name = "排序号")]
        public int? Seqid { get; set; }
    }
}
