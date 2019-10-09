using System;

namespace Gemstar.BSPMS.Common.PayManage.AliProviderPay
{
    /// <summary>
    /// AlipayTradeRefundContentBuilder 的摘要说明
    /// </summary>
    public class AlipayTradeRefundContentBuilder : JsonBuilder
    {


        public string trade_no { get; set; }

        public string out_trade_no { get; set; }

        public string refund_amount { get; set; }

        public string out_request_no { get; set; }

        public string refund_reason { get; set; }


        public AlipayTradeRefundContentBuilder()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(refund_amount))
            {
                throw new ArgumentException("缺少需要退款的金额，该金额不能大于订单金额,单位为元，支持两位小数", nameof(refund_amount));
            }
            return true;
        }


    }
}