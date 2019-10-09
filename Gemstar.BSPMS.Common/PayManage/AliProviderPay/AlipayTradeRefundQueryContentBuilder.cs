using System;

namespace Gemstar.BSPMS.Common.PayManage.AliProviderPay
{
    public class AlipayTradeRefundQueryContentBuilder : JsonBuilder
    {


        public string trade_no { get; set; }
        public string out_trade_no { get; set; }
        public string out_request_no { get; set; }


        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(out_request_no))
            {
                throw new ArgumentNullException(nameof(out_request_no), "缺少请求退款接口时，传入的退款请求号");
            }
            if (string.IsNullOrWhiteSpace(trade_no) && string.IsNullOrWhiteSpace(out_trade_no))
            {
                throw new ArgumentNullException(nameof(trade_no), "支付宝交易号，和商户订单号不能同时为空");
            }
            return true;
        }

    }
}