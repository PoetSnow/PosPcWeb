using System;
using System.Linq;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.WeixinManage;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EF;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models
{
    public static class OAuthHelper
    {
        /// <summary>
        /// 获取验证地址
        /// </summary>
        /// <param name="redirectUrl">授权后重定向的回调链接地址</param>
        /// <param name="state">重定向后会带上state参数，开发者可以填写a-zA-Z0-9的参数值，最多128字节</param>
        public static string GetAuthorizeUrl(string redirectUrl, string state)
        {
            return Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAuthorizeUrl(MvcApplication.WeixinAppId, redirectUrl, state, OAuthScope.snsapi_base);
        }

        /// <summary>
        /// 根据Code获取OpenId
        /// </summary>
        /// <param name="code">code作为换取access_token的票据</param>
        /// <returns></returns>
        public static string GetOpenId(string code)
        {
            var result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(MvcApplication.WeixinAppId, MvcApplication.WeixinAppSecret, code);
            if(result != null && result.errcode == ReturnCode.请求成功)
            {
                return result.openid;
            }
            return null;
        }
    }
}