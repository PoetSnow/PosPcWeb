using System.ComponentModel.DataAnnotations;
using System;
using Gemstar.BSPMS.Hotel.Web.Models;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Percentages.Models.OperatorPolicy
{
    public class OperatorPolicyEditViewModel : BaseEditViewModel
    {
        /// <summary>
        /// 政策id
        /// </summary>
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public Guid PolicyId { get; set; }

        /// <summary>
        /// 提成操作金额来源，操作员提成取值：合约单位签约数，会员发卡数，充值金额，合约单位消费金额，会员消费金额 
        /// </summary>
        [Display(Name = "提成内容")]
        [Required(ErrorMessage = "提成内容")]
        public string AmountSource { get; set; }

        /// <summary>
        /// 是否计划内,1:计划内，0：计划外（使用下拉或者单选让操作员选择）null单次，单次不分计划内外
        /// </summary>
        [Display(Name = "内容类型")]
        public bool? IsInPlan { get; set; }

        /// <summary>
        /// 开始金额
        /// </summary>
        [Display(Name = "开始值")]
        public decimal AmountBegin { get; set; }

        /// <summary>
        /// 结束金额
        /// </summary>
        [Display(Name = "结束值")]
        public decimal AmountEnd { get; set; }

        /// <summary>
        /// 是否全额，1：全额，0：阶梯（使用下拉或者单选让操作员选择）
        /// </summary>
        [Display(Name = "是否全额")]
        public bool IsAllAmount { get; set; }

        /// <summary>
        /// 金额计算类型：single:单次，month:按月累计
        /// </summary>
        [Display(Name = "计算类型")]
        public string AmountSumType { get; set; }

        /// <summary>
        /// 提成计算类型：percent:比例，price:单价，amount:固定金额
        /// </summary>
        [Display(Name = "提成类型")]
        public string CalcType { get; set; }

        /// <summary>
        /// 提成计算类型对应的值
        /// </summary>
        [Display(Name = "提成值")]
        public decimal? CalcValue { get; set; }

        /// <summary>
        /// 提成政策说明
        /// </summary>
        [Display(Name = "政策说明")]
        public string PolicyDesciption { get; set; }
    }
}