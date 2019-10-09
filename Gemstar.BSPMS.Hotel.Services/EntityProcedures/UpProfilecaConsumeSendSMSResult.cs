using System;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 存储过程up_profileca_consumeSendSMS的结果集
    /// 用于发送同时扣会员储值和增值的消费短信
    /// </summary>
    public class UpProfilecaConsumeSendSMSResult
    {
        /// <summary>
        /// 会员卡类型名称
        /// </summary>
        public string MbrCardTypeName { get; set; }
        /// <summary>
        /// 会员姓名
        /// </summary>
        public string GuestName { get; set; }
        /// <summary>
        /// 会员卡号
        /// </summary>
        public string MbrCardNo { get; set; }
        /// <summary>
        /// 酒店名称
        /// </summary>
        public string HotelName { get; set; }
        /// <summary>
        /// 消费日期
        /// </summary>
        public DateTime? TransDate { get; set; }
        /// <summary>
        /// 消费扣除的储值金额
        /// </summary>
        public decimal? ConsumeChargeAmount { get; set; }
        /// <summary>
        /// 消费扣除的增值金额
        /// </summary>
        public decimal? ConsumeLargessAmount { get; set; }
        /// <summary>
        /// 消费完后的储值余额
        /// </summary>
        public decimal? BalanceCurrentCharge { get; set; }
        /// <summary>
        /// 消费完后的增值余额
        /// </summary>
        public decimal? BalanceCurrentLargess { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string InputUser { get; set; }
        /// <summary>
        /// 营业点名称
        /// </summary>
        public string OutletName { get; set; }
    }
}
