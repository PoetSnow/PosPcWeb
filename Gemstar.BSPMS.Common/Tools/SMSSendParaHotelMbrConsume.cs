using System;
using System.Text;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 发送会员消费短信参数类
    /// </summary>
    public class SMSSendParaHotelMbrConsume : SMSSendParaHotel
    {
        /// <summary>
        /// 客人名
        /// </summary>
        public string GuestName { get; set; }
        /// <summary>
        /// 消费时间
        /// </summary>
        public DateTime ConsumeTime { get; set; }
        /// <summary>
        /// 消费扣除的储值金额
        /// </summary>
        public decimal AmountCharge { get; set; }
        /// <summary>
        /// 消费扣除的增值赠送金额
        /// </summary>
        public decimal AmountLargess { get; set; }
        /// <summary>
        /// 扣除后的储值余额
        /// </summary>
        public decimal BalanceCharge { get; set; }
        /// <summary>
        /// 扣除后的增值余额
        /// </summary>
        public decimal BalanceLargess { get; set; }
        /// <summary>
        /// 重写参数是否有效
        /// 增加业务参数的有效性检测
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
        /// <summary>
        /// 发送内容
        /// </summary>
        protected override string SendContent
        {
            get
            {
                var totalAmount = AmountCharge + AmountLargess;
                var totalBalance = BalanceCharge + BalanceLargess;
                //尊敬的<客人>，您于<时间>在本店消费<金额>，已扣款成功；欢迎再次光临！【捷信达】
                var content = new StringBuilder();
                content.Append("尊敬的").Append(GuestName)
                    .Append("，您于").Append(ConsumeTime.ToString("MM-dd HH:mm")).Append("在本店消费").Append(totalAmount);
                if(AmountCharge > 0)
                {
                    content.Append("，其中储值消费").Append(AmountCharge);
                }
                if (AmountLargess > 0)
                {
                    content.Append("，增值消费").Append(AmountLargess);
                }
                content.Append("；余额共").Append(totalBalance)
                    .Append("，其中储值").Append(BalanceCharge)
                    .Append("，增值").Append(BalanceLargess)
                    .Append("；欢迎再次光临！【").Append(HotelName).Append("】");
                return content.ToString();
            }
        }
    }
}
