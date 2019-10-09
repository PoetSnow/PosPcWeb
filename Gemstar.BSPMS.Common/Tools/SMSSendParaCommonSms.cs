using System;
using System.Text;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 通用短信模板
    /// </summary>
    public class SMSSendParaCommonSms : SMSSendParaHotel
    {
        /// <summary>
        /// 短信内容
        /// </summary>
        public string Content { get; set; }
        /// </summary>
        protected override string SendContent
        {
            get
            {
                var content = new StringBuilder();
                content.Append(Content)
                    .Append("【").Append(HotelName).Append("】");
                return content.ToString();
            }
        }
    }
}
