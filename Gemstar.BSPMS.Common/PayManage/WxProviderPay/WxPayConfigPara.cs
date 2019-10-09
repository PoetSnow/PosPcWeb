using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.PayManage.WxProviderPay
{
    /// <summary>
    /// 微信服务商支付配置参数值
    /// </summary>
    public class WxPayConfigPara : IWxHttpServicePara
    {
        /// <summary>
        /// 代理服务器设置,对于需要代理服务器才能上网时进行设置，格式如：http://10.152.18.220:8080
        /// </summary>
        public string ProxyUrl { get; set; }
        /// <summary>
        /// 终端ip
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 公众账号ID
        /// </summary>
        public string WxProviderAppId { get; set; }
        /// <summary>
        /// 公众账号Secret
        /// </summary>
        public string WxProviderSecret { get; set; }
        /// <summary>
        /// 微信服务商api key
        /// </summary>
        public string WxProviderKey { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string WxProviderMchId { get; set; }
        /// <summary>
        /// 分店名称，用于区分同一收款账号下不同分店的账务
        /// </summary>
        public string ResortName { get; set; }
        /// <summary>
        /// 子公众账号ID
        /// </summary>
        public string AppID { get; set; }
        /// <summary>
        /// 子商户号,实际收钱的商户号
        /// </summary>
        public string MchID { get; set; }
        /// <summary>
        /// 测速上报等级，0.关闭上报; 1.仅错误时上报; 2.全量上报
        /// </summary>
        public int ReportLevenl { get; set; }
        /// <summary>
        /// 异步通知地址
        /// </summary>
        public string WxProviderNotifyUrl { get; set; }
        /// <summary>
        /// 微信证书路径
        /// </summary>
        public string CertResourceName { get; set; }
        /// <summary>
        /// 微信服务商退款中转接口地址
        /// </summary>
        public string WxRefundTransferUrl { get; set; }

        public string CertKey
        {
            get
            {
                return WxProviderMchId;
            }
        }
    }
}
