using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.SystemManage
{
    /// <summary>
    /// 酒店的微信相关参数值
    /// </summary>
    public class HotelWxParas
    {
        /// <summary>
        /// 微信appid
        /// </summary>
        public string WxAppId { get; set; }
        /// <summary>
        /// 微信密钥
        /// </summary>
        public string WxAppSecret { get; set; }
        /// <summary>
        /// 传输加密密钥，如果是明文模式则可以不需要
        /// </summary>
        public string WxEncodingAesKey { get; set; }
        /// <summary>
        /// 微信绑定时的token值
        /// </summary>
        public string WxToken { get; set; }
        /// <summary>
        /// 长租催租，业主费用催缴模板id
        /// </summary>
        public string LongTemplateId { get; set; }
        /// <summary>
        /// 是否启用gs微信中转接口，用于公司微信营销平台接管后台，业务系统通过其中转后请求微信服务
        /// </summary>
        public string IsGsWxInterface { get; set; }
        /// <summary>
        /// 酒店在微信营销平台的公司id
        /// </summary>
        public string GsWxComid { get; set; }
        /// <summary>
        /// 酒店在微信营销平台的获取openid的接口地址
        /// </summary>
        public string GsWxOpenidUrl { get; set; }
        /// <summary>
        /// 酒店在微信营销平台的推送模板消息的接口地址
        /// </summary>
        public string GsWxTemplateMessageUrl { get; set; }
        /// <summary>
        /// 酒店在微信营销平台的预下单接口地址
        /// </summary>
        public string GsWxCreatePayOrderUrl { get; set; }
        /// <summary>
        /// 酒店在微信营销平台的微信支付接口地址
        /// </summary>
        public string GsWxPayOrderUrl { get; set; }

    }
}
