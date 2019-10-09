using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Percentages.Models.SalesmanPlan
{
    public class SalesmanPlanAddViewModel
    {
        /// <summary>
        /// 业务员id
        /// </summary>
        [Display(Name = "业务员")]
        [Required(ErrorMessage = "请选择业务员")]
        public List<Guid> SalesmanIds { get; set; }

        /// <summary>
        /// 计划任务年月，只取年月信息
        /// </summary>
        [Display(Name = "计划年月")]
        [Required(ErrorMessage = "请选择计划年月")]
        public DateTime PlanDate { get; set; }

        /// <summary>
        /// 提成类型（合约单位签约数，会员发卡数，会员充值金额，合约单位消费金额，会员消费金额）
        /// </summary>
        [Display(Name = "计划内容")]
        [Required(ErrorMessage = "请选择计划内容")]
        public string PlanSource { get; set; }

        /// <summary>
        /// 计划任务金额或数量
        /// </summary>
        [Display(Name = "计划值")]
        [Required(ErrorMessage = "请输入计划值")]
        public decimal PlanAmount { get; set; }
    }
}