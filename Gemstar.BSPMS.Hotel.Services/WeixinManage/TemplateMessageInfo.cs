using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.WeixinManage
{
    /// <summary>
    /// 模板消息类
    /// </summary>
    public class TemplateMessageInfo
    {
        /// <summary>
        /// 授权提醒类
        /// </summary>
        public class SendAuthTemplateMessageModel : TemplateMessageModel
        {
            /// <summary>
            /// 标题（房价修改授权申请，客账冲销授权申请，客账减免授权申请等等）
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 酒店名称（住哲酒店）
            /// </summary>
            public string HotelName { get; set; }
            /// <summary>
            /// 授权申请人（manager）
            /// </summary>
            public string AuthApplicant { get; set; }
            /// <summary>
            /// 授权内容（修改888号房间房价，由888改为666）
            /// </summary>
            public string AuthContent { get; set; }
            /// <summary>
            /// 授权申请时间（2015-07-10 11:13）
            /// </summary>
            public DateTime AuthApplicantDateTime { get; set; }
            /// <summary>
            /// 消息链接URL
            /// </summary>
            public string Url { get; set; }
        }
        /// <summary>
        /// 报表提醒类
        /// </summary>
        public class SendReportFormTemplateMessageModel : TemplateMessageModel
        {
            /// <summary>
            /// 标题（酒店经营日报表为您送达，某某某酒店昨日的营业报表已生成 等等）
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 酒店名称（住哲酒店）
            /// </summary>
            public string HotelName { get; set; }
            /// <summary>
            /// 营业日期
            /// </summary>
            public DateTime BusinessDate { get; set; }
            /// <summary>
            /// 报表名称
            /// </summary>
            public string ReportFormName { get; set; }
            /// <summary>
            /// 消息链接URL
            /// </summary>
            public string Url { get; set; }
        }
        /// <summary>
        /// 模板消息基类
        /// </summary>
        public class TemplateMessageModel
        {
            private Guid _id = Guid.Empty;
            public TemplateMessageModel()
            {
                _id = Guid.NewGuid();
            }
            /// <summary>
            /// 主键ID
            /// </summary>
            public Guid Id { get { return _id; } }
            /// <summary>
            /// 酒店ID
            /// </summary>
            public string Hid { get; set; }
            /// <summary>
            /// 模板消息类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权；4：营业简报推送）
            /// </summary>
            public byte Type { get; set; }
            /// <summary>
            /// 微信用户标识Openid 消息接收人唯一标识
            /// </summary>
            public string Openid { get; set; }
            /// <summary>
            /// 外键ID（等于具体业务表主键ID，例如：authorizationRecord.id）
            /// </summary>
            public Guid Keyid { get; set; }
        }
    }
}
