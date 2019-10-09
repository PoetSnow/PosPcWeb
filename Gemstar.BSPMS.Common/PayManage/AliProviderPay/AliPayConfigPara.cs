namespace Gemstar.BSPMS.Common.PayManage.AliProviderPay
{
    /// <summary>
    /// 支付宝服务商支付配置参数值
    /// </summary>
    public class AliPayConfigPara
    {
        /// <summary>
        /// 支付宝账户签约的应用ID
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 支付宝账号上传公钥的对应秘钥
        /// </summary>
        public string PrivateKey { get; set; }
        /// <summary>
        /// 支付宝合作伙伴ID
        /// </summary>
        public string PID { get; set; }
        /// <summary>
        /// 支付宝接口网关
        /// </summary>
        public string ServerUrl { get; set; }
        /// <summary>
        /// 支付宝版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 支付宝签名类型
        /// </summary>
        public string SignType { get; set; }
        /// <summary>
        /// 支付宝公钥
        /// </summary>
        public string AlipayPublicKey { get; set; }
        /// <summary>
        /// 支付宝系统接入商合作伙伴ID
        /// </summary>
        public string SysServiceProviderId { get; set; }
        /// <summary>
        /// 支付宝编码方式
        /// </summary>
        public string Charset { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        public string AliPayCallbackUrl { get; set; }
        /// <summary>
        /// 通知验证地址
        /// </summary>
        public string MapiUrl { get; set; }
    }
}
