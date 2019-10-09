using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    /// <summary>
    /// 业务员提成政策定义表
    /// </summary>
    [Table("PercentagesPolicySalesman")]
    public class PercentagesPolicySalesman
    {
        /// <summary>
        /// 政策id
        /// </summary>
        [Key]
        public Guid PolicyId { get; set; }

        /// <summary>
        /// 酒店id
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        /// <summary>
        /// 提成政策说明
        /// </summary>
        [Column(TypeName = "varchar")]
        public string PolicyDesciption { get; set; }

        /// <summary>
        /// 提成业务金额来源，业务员提成取值：合约单位签约数，会员发卡数，充值金额，合约单位消费金额，会员消费金额 
        /// </summary>
        [Column(TypeName = "varchar")]
        public string AmountSource { get; set; }

        /// <summary>
        /// 是否计划内,1:计划内，0：计划外（使用下拉或者单选让操作员选择）null单次，不分计划内外
        /// </summary>
        public bool? IsInPlan { get; set; }

        /// <summary>
        /// 开始金额
        /// </summary>
        public decimal AmountBegin { get; set; }

        /// <summary>
        /// 结束金额
        /// </summary>
        public decimal AmountEnd { get; set; }

        /// <summary>
        /// 是否全额，1：全额，0：阶梯（使用下拉或者单选让操作员选择）
        /// </summary>
        public bool IsAllAmount { get; set; }

        /// <summary>
        /// 金额计算类型：single:单次，month:按月累计
        /// </summary>
        [Column(TypeName = "varchar")]
        public string AmountSumType { get; set; }

        /// <summary>
        /// 提成计算类型：percent:比例，price:单价，amount:固定金额
        /// </summary>
        [Column(TypeName = "varchar")]
        public string CalcType { get; set; }

        /// <summary>
        /// 提成计算类型对应的值
        /// </summary>
        public decimal? CalcValue { get; set; }

    }
}
