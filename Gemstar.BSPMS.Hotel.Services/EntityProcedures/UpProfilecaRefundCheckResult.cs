using System;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 检测充值记录是否可以及需要退款的结果集
    /// 存储过程up_profileca_refundCheck的结果集
    /// </summary>
    public class UpProfilecaRefundCheckResult
    {
        /// <summary>
        /// 是否已经退款
        /// </summary>
        public bool IsRefunded { get; set; }
        /// <summary>
        /// 充值记录id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 会员id
        /// </summary>
        public Guid Profileid { get; set; }
        /// <summary>
        /// 会员卡类型id
        /// </summary>
        public string MbrCardTypeid { get; set; }
        /// <summary>
        /// 余额类型
        /// </summary>
        public string BalanceType { get; set; }
        /// <summary>
        /// 交易说明
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal? Amount { get; set; }
        /// <summary>
        /// 支付相关单号
        /// </summary>
        public string Refno { get; set; }
        /// <summary>
        /// 支付方式id
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// 支付方式处理运作
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public string transDate { get; set; }
        /// <summary>
        /// 原币金额
        /// </summary>
        public decimal? originAmount { get; set; }
    }
}
