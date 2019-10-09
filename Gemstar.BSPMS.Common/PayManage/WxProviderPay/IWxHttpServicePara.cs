namespace Gemstar.BSPMS.Common.PayManage.WxProviderPay
{
    /// <summary>
    /// 微信支付httpPost参数接口
    /// </summary>
    public interface IWxHttpServicePara
    {
        /// <summary>
        /// 网关对象
        /// </summary>
        string ProxyUrl { get; }
        /// <summary>
        /// 证书资源名称
        /// </summary>
        string CertResourceName { get; }
        /// <summary>
        /// 证书密钥
        /// </summary>
        string CertKey { get; }
    }
}
