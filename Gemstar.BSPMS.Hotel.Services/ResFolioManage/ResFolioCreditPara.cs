using System;

namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 增加账务明细贷方付款参数
    /// </summary>
    public class ResFolioCreditPara
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
        /// 原币含税金额
        /// </summary>
        public decimal OriAmount { get; set; }
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
        /// <summary>
        /// 外部单号
        /// </summary>
        public string RefNo { get; set; }
        /// <summary>
        /// 是否等待支付
        /// </summary>
        public bool IsWaitPay { get; set; }
        /// <summary>
        /// 班次
        /// </summary>
        public string TransShift { get; set; }

        /// <summary>
        /// 付款类型
        /// </summary>
        public string Paymentdesc { get; set; }

        /// <summary>
        /// 是否作废，true作废，false不作废，IsWaitPay是否等待支付 优先
        /// </summary>
        public bool Invalid { get; set; }
        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime? InputDate { get; set; }

        /// <summary>
        /// 长租房押金类型
        /// </summary>
        public string FolioDepositType { get; set; }

    }
}
