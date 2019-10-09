using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Controllers
{
    public class LoginController : BaseWeixinController
    {
        /// <summary>
        /// 确认登录
        /// </summary>
        /// <param name="code">微信code作为换取access_token的票据</param>
        /// <param name="state">主键ID</param>
        /// <param name="sign">外键ID</param>
        /// <param name="errcode">错误代码（invalid请求已过期，use二维码已使用）</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ConfirmQrcode(string code, string state, string sign, string errcode)
        {
            try
            {
                LoginModel result = new LoginModel { Status = new LoginModelStatus { Icon = Icons.warn, Title = "参数错误", Descript = "请重试" } };
                #region errcode
                if (!string.IsNullOrWhiteSpace(errcode))
                {
                    switch (errcode)
                    {
                        case "invalid":
                            result.Status.Title = "请求已过期";
                            break;
                        case "use":
                            result.Status.Title = "二维码已使用";
                            break;
                        default:
                            result.Status.Title = "状态错误";
                            break;
                    }
                    return View(result);
                }
                #endregion

                //1.验证
                if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state) || string.IsNullOrWhiteSpace(sign))
                {
                    return View(result);
                }

                string openId = Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models.OAuthHelper.GetOpenId(code);
                if (string.IsNullOrWhiteSpace(openId))
                {
                    return View(result);
                }

                Common.Services.Entities.WeixinQrcodeLogin entity = null; List<Common.Tools.KeyValuePairModel<string, string>> hids = null;
                var jsonResult = GetService<Gemstar.BSPMS.Hotel.Services.WeixinManage.IQrcodeLoginService>().GetConfirmQrcode(state, sign, openId, out entity, out hids);

                if (jsonResult != null && jsonResult.Success)
                {
                    return View(new LoginModel { Status = null, Id = entity.Id, KeyId = entity.Keyid, OpenId = Common.Tools.CryptHelper.EncryptMd5(openId), HotelList = hids });
                }

                if (jsonResult != null && jsonResult.Data != null && !string.IsNullOrWhiteSpace(jsonResult.Data.ToString()))
                {
                    result.Status.Title = jsonResult.Data.ToString();
                }
                return View(result);
            }
            catch { }
            return View(new LoginModel { Status = new LoginModelStatus { Icon = Icons.warn, Title = "参数错误", Descript = "请重试" } });
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="para_id"></param>
        /// <param name="para_keyid"></param>
        /// <param name="para_openid"></param>
        /// <param name="para_hid"></param>
        /// <param name="para_loginSuccess"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmQrcode(string para_id, string para_keyid, string para_openid, string para_hid, bool para_loginSuccess)
        {
            try
            {
                LoginModel result = new LoginModel { Status = new LoginModelStatus { Icon = Icons.warn, Title = "参数错误", Descript = "请重试" } };

                if (string.IsNullOrWhiteSpace(para_id) || string.IsNullOrWhiteSpace(para_keyid) || string.IsNullOrWhiteSpace(para_openid) || string.IsNullOrWhiteSpace(para_hid))
                {
                    return View(result);
                }

                var jsonResult = GetService<Gemstar.BSPMS.Hotel.Services.WeixinManage.IQrcodeLoginService>().SubmitConfirmQrcode(para_id, para_keyid, para_openid, para_hid, para_loginSuccess);
                if (jsonResult != null && jsonResult.Success)
                {
                    result.Status.Icon = Icons.success;
                    result.Status.Title = para_loginSuccess ? "登录成功" : "已拒绝登录";
                    result.Status.Descript = "";
                    return View(result);
                }

                if (jsonResult != null && jsonResult.Data != null && !string.IsNullOrWhiteSpace(jsonResult.Data.ToString()))
                {
                    result.Status.Title = jsonResult.Data.ToString();
                }
                return View(result);
            }
            catch { }
            return View(new LoginModel { Status = new LoginModelStatus { Icon = Icons.warn, Title = "参数错误", Descript = "请重试" } });
        }

    }
}