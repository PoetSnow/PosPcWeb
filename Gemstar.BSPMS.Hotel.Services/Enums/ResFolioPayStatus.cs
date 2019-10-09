namespace Gemstar.BSPMS.Hotel.Services.Enums
{
    public enum ResFolioPayStatus:byte
    {
        /// <summary>
        /// 等待支付
        /// </summary>
        WaitPay = 0,
        /// <summary>
        /// 支付成功
        /// </summary>
        PaidSuccess = 1,
        /// <summary>
        /// 支付失败
        /// </summary>
        PaidFailure = 2
    }
}
