using System;

namespace Gemstar.BSPMS.Common.PayManage.AliProviderPay
{
    /// <summary>
    /// AlipayTradeQueryCententBuilder 的摘要说明
    /// </summary>
    public class AlipayTradeQueryContentBuilder : JsonBuilder
    {


        public string trade_no { get; set; }
        public string out_trade_no { get; set; }


        public AlipayTradeQueryContentBuilder()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(trade_no) && string.IsNullOrWhiteSpace(out_trade_no))
            {
                throw new ArgumentNullException(nameof(trade_no), "支付宝交易号，和商户订单号不能同时为空");
            }
            return true;
        }

    }
}