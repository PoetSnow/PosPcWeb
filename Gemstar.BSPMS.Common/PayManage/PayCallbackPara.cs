namespace Gemstar.BSPMS.Common.PayManage
{
    public class PayCallbackPara
    {
        /// <summary>
        /// 是否支付成功
        /// </summary>
        public bool IsPaidSuccess { get; set; }
        /// <summary>
        /// 酒店id
        /// </summary>
        public string HotelId { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal? PaidAmount { get; set; }
        /// <summary>
        /// 交易流水号
        /// </summary>
        public string PaidTransId { get; set; }
        /// <summary>
        /// 支付的产品类型
        /// </summary>
        public PayProductType ProductType { get; set; }
        /// <summary>
        /// 业务系统订单号
        /// </summary>
        public string OutTradeNo { get; set; }
        /// <summary>
        /// 支付错误信息，当支付失败时填写
        /// </summary>
        public string PaidError { get; set; }
    }
}
