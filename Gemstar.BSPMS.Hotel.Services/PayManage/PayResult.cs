using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PayManage
{
    /// <summary>
    /// 支付结果
    /// </summary>
    public class PayResult
    {
        /// <summary>
        /// 参考号
        /// </summary>
        public string RefNo { get; set; }
        /// <summary>
        /// 是否等待支付
        /// </summary>
        public bool IsWaitPay { get; set; }
        /// <summary>
        /// 是否需要拆分成多条付款记录
        /// </summary>
        public bool IsMultiple { get; set; }
        /// <summary>
        /// 拆分后的多条付款记录信息
        /// </summary>
        public List<PayItemInfo> MultipleItemInfos { get; set; }
        /// <summary>
        /// 需要返写回账务明细表中的备注信息
        /// </summary>
        public string Remark { get; set; }
        public class PayItemInfo
        {
            public string RefNo { get; set; }
            public string ItemId { get; set; }
            public decimal Amount { get; set; }
            public string Remark { get; set; }
        }
    }
}
