using System;
using System.Text;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 渠道订单发送短信到客人的短信参数类
    /// </summary>
    public class SMSSendParaHotelMsgToGuest : SMSSendParaHotel
    {
        /// <summary>
        /// 客人名
        /// </summary>
        public string GuestName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 发送内容
        /// </summary>
        protected override string SendContent
        {
            get
            {
                var content = new StringBuilder();
                content.Append(Remark)
                    .Append("【").Append(HotelName).Append("】");
                return content.ToString();
            }
        }
    }
}
