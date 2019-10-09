using System;
using System.Text;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 渠道订单发送短信到客服的短信参数类
    /// </summary>
    public class SMSSendParaHotelMsgToCutom : SMSSendParaHotel
    {
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
                //尊敬的<客人>，您的<房间号>房间门锁密码为<房间密码>；有效期至<EndDate>,感谢您的光临！【捷信达】
                var content = new StringBuilder();
                content.Append(Remark).Append("【").Append(HotelName).Append("】");
                return content.ToString();
            }
        }
    }
}
