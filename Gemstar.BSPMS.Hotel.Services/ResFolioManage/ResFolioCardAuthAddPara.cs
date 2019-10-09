namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 信用卡授权增加参数
    /// </summary>
    public class ResFolioCardAuthAddPara
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 预订明细id
        /// </summary>
        public string RegId { get; set; }
        /// <summary>
        /// 付款方式id
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// 信用卡号
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 信用卡有效期
        /// </summary>
        public string ValidDate { get; set; }
        /// <summary>
        /// 授权号
        /// </summary>
        public string AuthNo { get; set; }
        /// <summary>
        /// 授权金额
        /// </summary>
        public decimal? AuthAmount { get; set; }
        /// <summary>
        /// 授权人
        /// </summary>
        public string CreateUser { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
