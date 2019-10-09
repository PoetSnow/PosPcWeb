namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 客账界面中的账务明细
    /// 存储过程up_queryResFolioForFolio的查询结果对象
    /// </summary>
    public class ResFolioFolioInfo
    {
        /// <summary>
        /// 账务id
        /// </summary>
        public string Transid { get; set; }
        /// <summary>
        /// 房间号
        /// </summary>
        public string RoomNo { get; set; }
        /// <summary>
        /// 消费项目名称
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public decimal? Quantity { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal? CreditAmount { get; set; }
        /// <summary>
        /// 消费金额
        /// </summary>
        public decimal? DebitAmount { get; set; }
        /// <summary>
        /// 账务发生时间
        /// </summary>
        public string TransDate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 付款类型
        /// </summary>
        public string Paymentdesc { get; set; }

        /// <summary>
        /// 原始登记单ID
        /// </summary>
        public string RegidFrom { get; set; }

        /// <summary>
        /// 录入操作员
        /// </summary>
        public string InputUser { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string RegId { get; set; }
    }
}
