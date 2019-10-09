namespace Gemstar.BSPMS.Hotel.Services.PayManage
{
    /// <summary>
    /// 支付后的结果
    /// </summary>
    public class PayAfterResult
    {
        /// <summary>
        /// 支付状态
        /// </summary>
        public PayStatu Statu { get; set; }
        /// <summary>
        /// 回调的方法名，为空表示后续不需要处理，否则填写需要继续处理的js函数名称
        /// </summary>
        public string Callback { get; set; }
        /// <summary>
        /// 查询支付状态用的交易号
        /// </summary>
        public string QueryTransId { get; set; }
        /// <summary>
        /// 二维码地址，如果是扫码支付，则需要显示此链接地址对应的二维码图片
        /// </summary>
        public string QrCodeUrl { get; set; }
    }
    public enum PayStatu
    {
        /// <summary>
        /// 待支付，需要稍后查询实际支付结果
        /// </summary>
        WaitPay = 1,
        /// <summary>
        /// 支付成功
        /// </summary>
        Successed = 2
        //不存在支付失败的状态，支付失败的时候直接抛出异常即可
    }
}
