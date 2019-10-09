namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 入账增加结果
    /// </summary>
    public class ResFolioAddResult
    {
        /// <summary>
        /// 账务明细id
        /// </summary>
        public string FolioTransId { get; set; }
        /// <summary>
        /// 支付状态
        /// </summary>
        public string Statu { get; set; }
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
        /// <summary>
        /// DcFlag
        /// </summary>
        public string DCFlag { get; set; }
    }
}