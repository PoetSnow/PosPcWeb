using Senparc.Weixin;
using Senparc.Weixin.MP;
namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models
{
    public static class OAuthHelper
    {
        /// <summary>
        /// 获取验证地址（微信）
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="redirectUrl">授权后重定向的回调链接地址</param>
        /// <param name="state">重定向后会带上state参数，开发者可以填写a-zA-Z0-9的参数值，最多128字节</param>
        public static string GetWxAuthorizeUrl(string appId, string redirectUrl, string state, OAuthScope scope)
        {
            return Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAuthorizeUrl(appId, redirectUrl, state, scope);
        }

        /// <summary>
        /// 根据Code获取微信OpenId（微信）
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="secret"></param>
        /// <param name="code">code作为换取access_token的票据</param>
        /// <returns></returns>
        public static string GetWxOpenId(string appId, string secret, string code)
        {
            var result = Senparc.Weixin.MP.AdvancedAPIs.OAuthApi.GetAccessToken(appId, secret, code);
            if (result != null && result.errcode == ReturnCode.请求成功)
            {
                return result.openid;
            }
            return null;
        }
    }
}