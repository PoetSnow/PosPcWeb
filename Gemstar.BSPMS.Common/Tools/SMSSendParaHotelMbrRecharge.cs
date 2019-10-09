using System;
using System.Text;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 发送酒店会员充值短信参数
    /// </summary>
    public class SMSSendParaHotelMbrRecharge : SMSSendParaHotel
    {
        /// <summary>
        /// 客人名
        /// </summary>
        public string GuestName { get; set; }
        /// <summary>
        /// 充值时间
        /// </summary>
        public DateTime RechargeTime { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal RechargeAmount { get; set; }
        /// <summary>
        /// 当前增值余额
        /// </summary>
        public decimal CurrentLargess { get; set; }
        /// <summary>
        /// 赠送金额
        /// </summary>
        public decimal LargessAmount { get; set; }
        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; }
        /// <summary>
        /// 重写参数检测
        /// 增加了业务参数检测
        /// </summary>
        /// <param name="invalidMsg">无效原因</param>
        /// <returns>是否有效</returns>
        protected override bool IsValid(out string invalidMsg)
        {
            if (string.IsNullOrWhiteSpace(GuestName))
            {
                invalidMsg = "客人名不能为空";
                return false;
            }
            return base.IsValid(out invalidMsg);
        }
        protected override string SendContent
        {
            get
            {
                //尊敬的<客人>，您于<时间>充值<充值金额>元成功，本次增值金额<增值金额>元，现卡上金额为<卡上金额>元，非常感谢！【捷信达】
                var content = new StringBuilder();
                content.Append("尊敬的").Append(GuestName)
                    .Append("，您于").Append(RechargeTime.ToString("MM-dd HH:mm"))
                    .Append("充值").Append(RechargeAmount).Append("元成功")
                    .Append("，本次增值金额").Append(LargessAmount).Append("元")
                    .Append("，现充值余额为").Append(Balance).Append("元")
                    .Append("，增值余额为").Append(CurrentLargess).Append("元")
                    .Append("，非常感谢！【").Append(HotelName).Append("】");
                return content.ToString();
            }
        }
    }
}
