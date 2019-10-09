namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 增加账务明细借方消费的参数
    /// </summary>
    public class ResFolioDebitPara
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 订单明细id
        /// </summary>
        public string RegId { get; set; }
        /// <summary>
        /// 消费项目id
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 单据号码
        /// </summary>
        public string InvNo { get; set; }
        /// <summary>
        /// 操作员名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 说明备注
        /// </summary>
        public string Remark { get; set; }
        public string TransShift { get; set; }
    }
}
