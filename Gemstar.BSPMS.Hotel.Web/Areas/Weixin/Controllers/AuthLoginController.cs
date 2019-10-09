﻿using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Controllers
{
    public class AuthLoginController : BaseWeixinController {
        /// <summary>
        /// 确认登录
        /// </summary>
        /// <param name="code">微信code作为换取access_token的票据</param>
        /// <param name="state">主键ID</param>
        /// <param name="sign">外键ID</param>
        /// <param name="errcode">错误代码（invalid请求已过期，use二维码已使用）</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ConfirmQrcode(string code, string state, string sign, string errcode) {
            try {
                LoginModel result = new LoginModel { Status = new LoginModelStatus { Icon = Icons.warn, Title = "参数错误", Descript = "请重试" } };
                #region errcode
                if (!string.IsNullOrWhiteSpace(errcode)) {
                    switch (errcode) {
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
                if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state) || string.IsNullOrWhiteSpace(sign)) {
                    return View(result);
                }

                string openId = "";
#if envTest
                openId = code;
#else
                openId = Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models.OAuthHelper.GetOpenId(code);
#endif

                if (string.IsNullOrWhiteSpace(openId)) {
                    return View(result);
                }

                Common.Services.Entities.WeixinQrcodeLogin entity = null; 
                var jsonResult = GetService<Gemstar.BSPMS.Hotel.Services.WeixinManage.IQrcodeLoginService>().GetConfirmAuthQrcode(state, sign, openId, out entity);
                if (jsonResult != null) {
                    //如果获取成功
                    if (jsonResult.Success) {
                        return View(new LoginModel { Status = null, Id = entity.Id, KeyId = entity.Keyid, OpenId = openId });
                    }
                    //如果是由于指定的openid没有绑定，则转到绑定界面中
                    if (jsonResult.ErrorCode == 99) {
                        return View("Bind", new AuthLoginModel { OpenId = openId, QrcodeId = entity.Id.ToString() ,QrcodeKey = entity.Keyid.ToString()});
                    }
                    //其他情况下，则显示返回的错误内容
                    if (jsonResult.Data != null && !string.IsNullOrWhiteSpace(jsonResult.Data.ToString())) {
                        result.Status.Title = jsonResult.Data.ToString();
                    }
                }
                return View(result);
            } catch { }
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
        public ActionResult ConfirmQrcode(string para_id, string para_keyid, string para_openid, bool para_loginSuccess) {
            try {
                LoginModel result = new LoginModel { Status = new LoginModelStatus { Icon = Icons.warn, Title = "参数错误", Descript = "请重试" } };

                if (string.IsNullOrWhiteSpace(para_id) || string.IsNullOrWhiteSpace(para_keyid) || string.IsNullOrWhiteSpace(para_openid)) {
                    return View(result);
                }

                var authorizeService = GetService<IServiceAuthorizeService>();
                var accountInfo = authorizeService.AuthWxLogin(para_id, para_keyid, para_openid);
                if (accountInfo.LoginSuccess) {
                    if (accountInfo.UserId == "currentHotelAdminUserId") {
                        var centerDb = GetService<DbCommonContext>();
                        var hotelDbConnStr = MvcApplication.GetHotelDbConnStr(centerDb, accountInfo.Hid);
                        var hotelDb = new DbHotelPmsContext(hotelDbConnStr, accountInfo.Hid, accountInfo.UserName, Request);
                        var userId = hotelDb.Database.SqlQuery<string>("SELECT CONVERT(VARCHAR(60),id) AS id FROM pmsUser WHERE grpid = {0} AND (isReg = 1 OR Code = 'admin')", accountInfo.Grpid).First();
                        accountInfo.UserId = userId;
                    }
                    var jsonResult = GetService<Gemstar.BSPMS.Hotel.Services.WeixinManage.IQrcodeLoginService>().SubmitConfirmAuthQrcode(para_id,para_openid,para_loginSuccess,accountInfo);
                    if (jsonResult != null && jsonResult.Success) {
                        result.Status.Icon = Icons.success;
                        result.Status.Title = para_loginSuccess ? "登录成功" : "已拒绝登录";
                        result.Status.Descript = "";
                        return View(result);
                    }

                    if (jsonResult != null && jsonResult.Data != null && !string.IsNullOrWhiteSpace(jsonResult.Data.ToString())) {
                        result.Status.Title = jsonResult.Data.ToString();
                    }
                    return View(result);
                } else {
                    return View(new LoginModel { Status = new LoginModelStatus { Icon = Icons.warn, Title = accountInfo.ErrorMessage, Descript = "请重试" } });
                }
            } catch { }
            return View(new LoginModel { Status = new LoginModelStatus { Icon = Icons.warn, Title = "参数错误", Descript = "请重试" } });
        }
        #region 微信工程师绑定
        /// <summary>
        /// 发送绑定验证码
        /// </summary>
        /// <param name="mobile">售后工程师手机号</param>
        /// <returns></returns>
        public JsonResult SendCheckCodeByMobile(string mobile) {
            try {
                if (string.IsNullOrWhiteSpace(mobile)) {
                    return Json(JsonResultData.Failure("请输入手机号码"));
                }
                //验证输入的手机号是否是售后工程师的
                var centerDb = GetService<DbCommonContext>();
                var mobileCount = centerDb.Database.SqlQuery<int>("SELECT COUNT(*) FROM ServicesOperator WHERE MobilePhone = {0}", mobile ?? "").First();
                if(mobileCount != 1) {
                    return Json(JsonResultData.Failure("输入的手机号码不正确"));
                }
                var username = "";
                var password = "";
                var sendPara = new SMSSendParaCheckCode {
                    Mobile = mobile,
                    Func = Common.Enumerator.CheckFunc.ServiceOperatorBind,
                    UserName = username,
                    Password = password
                };
                return Json(SMSSendHelper.SendCheckCode(sendPara), JsonRequestBehavior.AllowGet);
            } catch (Exception ex) {
                return Json(JsonResultData.Failure(ex), JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Bind(AuthLoginModel model) {
            if (ModelState.IsValid) {
                //验证码验证
                var result = SMSSendHelper.MatchCheckCode(model.Mobile, model.CheckCode, Common.Enumerator.CheckFunc.ServiceOperatorBind);
                if (result.Success) {
                    //更新售后工程师的微信openid
                    var centerDb = GetService<DbCommonContext>();
                    centerDb.Database.ExecuteSqlCommand("UPDATE ServicesOperator SET LoginOpenid = {0} WHERE MobilePhone = {1}", model.OpenId ?? "", model.Mobile ?? "");
                    return Json(JsonResultData.Successed(""));
                }
                return Json(result);
            }
            return Json(JsonResultData.Failure(ModelState.Values));
        } 
        #endregion


    }
}