using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    /// <summary>
    /// 业务员提成任务定义表
    /// </summary>
    [Table("PercentagesPlanSalesman")]
    public class PercentagesPlanSalesman
    {
        /// <summary>
        /// 提成任务计划id
        /// </summary>
        [Key]
        public Guid PlanId { get; set; }

        /// <summary>
        /// 酒店id
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        /// <summary>
        /// 业务员id
        /// </summary>
        public Guid SalesmanId { get; set; }

        /// <summary>
        /// 提成业务金额来源，业务员提成取值：（合约单位签约数，会员发卡数，会员充值金额，合约单位消费金额，会员消费金额）
        /// </summary>
        [Column(TypeName = "varchar")]
        public string PlanSource { get; set; }

        /// <summary>
        /// 计划任务年月，只取年月信息
        /// </summary>
        public DateTime PlanDate { get; set; }

        /// <summary>
        /// 计划任务金额或数量
        /// </summary>
        public decimal PlanAmount { get; set; }

    }
}