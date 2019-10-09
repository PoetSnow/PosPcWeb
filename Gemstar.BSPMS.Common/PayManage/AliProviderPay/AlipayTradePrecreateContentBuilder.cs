using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Common.PayManage.AliProviderPay
{
    /// <summary>
    /// AlipayTradePrecreateContentBuilder 的摘要说明
    /// </summary>
    public class AlipayTradePrecreateContentBuilder : JsonBuilder
    {

        public string out_trade_no { get; set; }
        public string seller_id { get; set; }
        public string total_amount { get; set; }
        public string discountable_amount { get; set; }
        public string undiscountable_amount { get; set; }
        public string subject { get; set; }
        public string body { get; set; }

        public List<GoodsInfo> goods_detail { get; set; }
        public string operator_id { get; set; }

        public string store_id { get; set; }

        public string terminal_id { get; set; }

        public ExtendParams extend_params { get; set; }
        public string time_expire { get; set; }
        public string timeout_express { get; set; }

        public AlipayTradePrecreateContentBuilder()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }


        public override bool Validate()
        {
            if (string.IsNullOrWhiteSpace(out_trade_no))
            {
                throw new ArgumentNullException(nameof(out_trade_no), "缺少商户订单号,64个字符以内、只能包含字母、数字、下划线；需保证在商户端不重复");
            }
            if (string.IsNullOrWhiteSpace(total_amount))
            {
                throw new ArgumentNullException(nameof(total_amount), "缺少订单总金额，单位为元");
            }
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentNullException(nameof(subject), "缺少订单标题");
            }
            return true;
        }
    }
}