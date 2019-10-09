using System;

namespace Gemstar.BSPMS.Hotel.Services.MbrCardCenter
{
    /// <summary>
    /// 会员账务参数
    /// </summary>
    public class ProfileCaInputPara
    {
        /// <summary>
        /// 会员ID
        /// </summary>
        public Guid ProfileId { get; set; }
        /// <summary>
        /// 会员账户类型
        /// </summary>
        public ProfileAccountType AccountType { get; set; }
        /// <summary>
        /// 会员交易类型
        /// </summary>
        public ProfileCaType CaType { get; set; }
        /// <summary>
        /// 支付方式id
        /// </summary>
        public string PayWayId { get; set; }
        /// <summary>
        /// 支付（金额、积分、券）
        /// </summary>
        public decimal PayAmount { get; set; }
        /// <summary>
        /// 赠送（金额、积分、券）
        /// </summary>
        public decimal LargessAmount { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string InvNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 原币支付（金额，积分，券)
        /// </summary>
        public decimal? OriginPayAmount { get; set; }
        /// <summary>
        /// 外部单号
        /// </summary>
        public string RefNo { get; set; }
    }
}
