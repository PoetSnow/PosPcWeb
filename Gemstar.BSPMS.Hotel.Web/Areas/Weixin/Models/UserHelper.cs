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
    public class UserHelper
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="nickname"></param>
        /// <param name="headimgurl"></param>
        /// <returns></returns>
        public static JsonResultData GetUserInfo(string openid, out string nickname, out string headimgurl)
        {
            nickname = null;
            headimgurl = null;
            try
            {
                var userInfo = UserApi.Info(MvcApplication.WeixinAppId, openid);
                if (userInfo != null && userInfo.errcode == ReturnCode.请求成功)
                {
                    nickname = userInfo.nickname;
                    headimgurl = userInfo.headimgurl;
                    return JsonResultData.Successed();
                }
                return JsonResultData.Failure(userInfo != null ? userInfo.errcode.ToString() : "");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
    }
}