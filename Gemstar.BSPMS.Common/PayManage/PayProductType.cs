namespace Gemstar.BSPMS.Common.PayManage
{
    /// <summary>
    /// 支付产品类型
    /// </summary>
    public enum PayProductType
    {
        /// <summary>
        /// 客账付款
        /// </summary>
        ResFolio = 1,
        /// <summary>
        /// 会员充值
        /// </summary>
        MbrRecharge = 2,
        /// <summary>
        /// 合约单位收款
        /// </summary>
        CorpReceive = 3,
        /// <summary>
        /// 会员卡费收取
        /// </summary>
        MbrCardFee = 4,
        /// <summary>
        /// Pos付款
        /// </summary>
        PosPayment = 5,
    }
}
