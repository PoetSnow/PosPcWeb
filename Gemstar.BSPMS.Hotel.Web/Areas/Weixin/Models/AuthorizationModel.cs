using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models
{
    /// <summary>
    /// 授权类
    /// </summary>
    public class AuthorizationModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 授权记录
        /// </summary>
        public AuthorizationRecord AuthorizationRecord { get; set; }

        /// <summary>
        /// 微信模板消息表主键ID
        /// </summary>
        public Guid WeixinTemplateMessageId { get; set; }

        /// <summary>
        /// 授权内容
        /// </summary>
        public object AuthContent { get; set; }
    }

    /// <summary>
    /// 授权记录表
    /// </summary>
    public class AuthorizationRecord : Gemstar.BSPMS.Hotel.Services.Entities.AuthorizationRecord
    {
        /// <summary>
        /// 酒店名称
        /// </summary>
        public string HotelName { get; set; }

        /// <summary>
        /// 授权申请人
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 授权人
        /// </summary>
        public string AuthUserName { get; set; }
    }

}