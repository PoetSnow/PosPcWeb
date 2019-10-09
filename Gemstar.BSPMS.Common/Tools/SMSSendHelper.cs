using Gemstar.BSPMS.Common.Enumerator;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Common.Tools
{
    public static class SMSSendHelper
    {
        public static string GetResult(int code)
        {
            var msg = "";
            switch (code)
            {
                case 1: msg = "发送成功"; break;
                case 0: msg = "短信发送失败"; break;

                case -1: msg = "用户名或者密码不正确"; break;
                case -2: msg = "必填选项为空"; break;
                case -3: msg = "短信内容0个字节"; break;
                case -4: msg = "请输入正确的手机号码"; break;
                case -5: msg = "余额不够"; break;
                case -10: msg = "用户被禁用"; break;
                case -11: msg = "短信内容超过500字"; break;
                case -12: msg = "无扩展权限（ext字段需填空）"; break;
                case -13: msg = "IP校验错误"; break;
                case -14: msg = "内容解析异常"; break;
                case -15: msg = "短信发送参数异常"; break;
                case -990: msg = "未知错误"; break;
                default: msg = "发送失败，非预期结果"; break;
            }
            return msg;
        }
        
        private static int SendMsg(string content,SMSSendPara sendPara,Dictionary<string,string> smsCenterParas, ISmsLogService smsLogService)
        {
            try
            {
                var para = smsCenterParas;
                var sendServer = string.Format("{0}{1}",para["smssendurl"],GetUrl("短信发送"));
                var userName = sendPara.UserName;
                var password = sendPara.Password;
                var mobile = sendPara.Mobile;

                //如果传递的用户名为空，则使用平台参数中的短信用户名
                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = para["smssendusername"];
                }
                //如果传递的密码为空，则使用平台参数中的短信密码
                if (string.IsNullOrWhiteSpace(password))
                {
                    password = para["smssendpassword"];
                }
                //检测参数值是否都有效
                if (string.IsNullOrWhiteSpace(sendServer) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                {
                    return -15;
                }

                var md5Password1 = CryptHelper.EncryptMd5(password).ToLower();
                var md5Password2 = CryptHelper.EncryptMd5(userName + md5Password1).ToLower();

                var postString = new StringBuilder();
                postString.Append("username=").Append(HttpUtility.UrlEncode(userName));
                postString.Append("&password=").Append(HttpUtility.UrlEncode(md5Password2));
                postString.Append("&mobile=").Append(HttpUtility.UrlEncode(mobile));
                postString.Append("&content=").Append(HttpUtility.UrlEncode(content));
                var contentBytes = Encoding.UTF8.GetBytes(postString.ToString());

                var request = WebRequest.CreateHttp(sendServer);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = contentBytes.Length;

                using (var input = request.GetRequestStream())
                {
                    input.Write(contentBytes, 0, contentBytes.Length);
                }
                using (var response = request.GetResponse())
                {
                    var streamReader = new StreamReader(response.GetResponseStream());
                    var result = streamReader.ReadToEnd();
                    if (result.Length > 8)
                    {
                        var mobiles = mobile.Split(',');
                        foreach(var mob in mobiles)
                        {
                            smsLogService.AddLog(mob, content, GetResult(1));
                        }
                        
                        return 1;
                    }
                    else
                    {
                        smsLogService.AddLog(mobile, content, GetResult(int.Parse(result)));
                        return int.Parse(result);
                    }
                }
            }
            catch
            {
                return -99;
            }
        }
        /// <summary>
        /// get或post发送数据，接收返回值
        /// </summary>
        /// <param name="type">提交类型:get,post</param>
        /// <param name="url">请求网址</param>
        /// <param name="data">请求数据</param>
        /// <returns></returns>
        public static string PostGetData(string type,string url,byte[] data)
        {
            try
            {
                var request = WebRequest.CreateHttp(url);
                request.Method = type;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var input = request.GetRequestStream())
                {
                    input.Write(data, 0, data.Length);
                }
                using (var response = request.GetResponse())
                {
                    var streamReader = new StreamReader(response.GetResponseStream());
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 查询余额
        /// </summary>
        /// <param name="sendPara"></param>
        /// <param name="smsCenterParas"></param>
        /// <param name="smsLogService"></param>
        /// <returns></returns>
        public static int QueryBalance(Dictionary<string, string> sendPara, Dictionary<string, string> smsCenterParas)
        {
            try
            {
                var para = smsCenterParas;
                var sendServer = string.Format("{0}{1}", para["smssendurl"], GetUrl("余额查询"));
                var userName = sendPara["username"];
                var password = sendPara["password"];

                //如果传递的用户名为空，则使用平台参数中的短信用户名
                if (string.IsNullOrWhiteSpace(userName))
                {
                    userName = para["smssendusername"];
                }
                //如果传递的密码为空，则使用平台参数中的短信密码
                if (string.IsNullOrWhiteSpace(password))
                {
                    password = para["smssendpassword"];
                }
                //检测参数值是否都有效
                if (string.IsNullOrWhiteSpace(sendServer) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                {
                    return -1;
                }

                var md5Password1 = CryptHelper.EncryptMd5(password).ToLower();
                var md5Password2 = CryptHelper.EncryptMd5(userName + md5Password1).ToLower();

                var postString = new StringBuilder();
                postString.Append("username=").Append(HttpUtility.UrlEncode(userName));
                postString.Append("&password=").Append(HttpUtility.UrlEncode(md5Password2));
                var contentBytes = Encoding.UTF8.GetBytes(postString.ToString());

                var request = WebRequest.CreateHttp(sendServer);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = contentBytes.Length;

                using (var input = request.GetRequestStream())
                {
                    input.Write(contentBytes, 0, contentBytes.Length);
                }
                using (var response = request.GetResponse())
                {
                    var streamReader = new StreamReader(response.GetResponseStream());
                    var result = streamReader.ReadToEnd();
                    if (result.Length > 8)
                    {
                        return -1;
                    }
                    else
                    {
                        return int.Parse(result);
                    }
                }
            }
            catch
            {
                return -1;
            }
        }
        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="mobile">要验证的手机号</param>
        /// <param name="func">需要验证的功能名称</param>
        /// <param name="userName">发送短信用户名</param>
        /// <param name="password">发送短信密码</param>
        /// <returns>发送验证码结果</returns>
        public static JsonResultData SendCheckCode(SMSSendParaCheckCode para)
        {
            try
            {
                var smsContent = para.GetSendContent();
                var mobile = para.Mobile;
                var func = para.Func.ToString();

                var _sysCheckCodeService = DependencyResolver.Current.GetService<ISysCheckCodeService>();
                var _sysParaService = DependencyResolver.Current.GetService<ISysParaService>();
                var smsCenterParas = _sysParaService.GetSMSSendPara();
                var smsLogService = DependencyResolver.Current.GetService<ISmsLogService>();
                //返回多少秒后可重新发送
                int seconds = 0;
                var checkResult = _sysCheckCodeService.GetCheckCode(CheckMethod.Mobile.ToString(), mobile, func, out seconds);
                //如果验证码已经发送，则根据返回的多少秒后可重新发送重置按钮状态
                if (checkResult.Success)
                {
                    //尝试重新发送当前验证码
                    SendMsg(string.Format(smsContent, checkResult.Data.ToString()), para, smsCenterParas, smsLogService);
                    return JsonResultData.Successed(seconds);
                }
                //创建验证码
                var checkCode = CommonHelper.GetCheckCode();
                //创建短信内容
                var result = SendMsg(string.Format(smsContent, checkCode), para, smsCenterParas, smsLogService);
                //发送成功，将验证码记录插入数据库
                if (result > 0)
                {
                    var sysCheckCode = new SysCheckCode();
                    sysCheckCode.Hid = "";
                    sysCheckCode.UserCode = "";
                    sysCheckCode.GetMethod = CheckMethod.Mobile.ToString();
                    sysCheckCode.GetMethodValue = mobile;
                    sysCheckCode.CheckCode = checkCode;
                    sysCheckCode.EndDate = DateTime.Now.AddSeconds(90);
                    sysCheckCode.Func = func;

                    var addResult = _sysCheckCodeService.AddCheckCode(sysCheckCode);
                    return addResult;
                }
                else
                {
                    return JsonResultData.Failure(GetResult(result));
                }
            }catch(Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }

        /// <summary>
        /// 发送手机验证码返回验证码
        /// </summary>
        /// <param name="mobile">要验证的手机号</param>
        /// <param name="func">需要验证的功能名称</param>
        /// <param name="userName">发送短信用户名</param>
        /// <param name="password">发送短信密码</param>
        /// <returns>发送验证码结果</returns>
        public static string SendCheckCodeandRetn(SMSSendParaCheckCode para)
        {
            try
            {
                var smsContent = para.GetSendContent();
                var mobile = para.Mobile;
                var func = para.Func.ToString();

                var _sysCheckCodeService = DependencyResolver.Current.GetService<ISysCheckCodeService>();
                var _sysParaService = DependencyResolver.Current.GetService<ISysParaService>();
                var smsCenterParas = _sysParaService.GetSMSSendPara();
                var smsLogService = DependencyResolver.Current.GetService<ISmsLogService>();
                //返回多少秒后可重新发送
                int seconds = 0;
                var checkResult = _sysCheckCodeService.GetCheckCode(CheckMethod.Mobile.ToString(), mobile, func, out seconds);
                //如果验证码已经发送，则根据返回的多少秒后可重新发送重置按钮状态
                if (checkResult.Success)
                {
                    //尝试重新发送当前验证码
                    SendMsg(string.Format(smsContent, checkResult.Data.ToString()), para, smsCenterParas, smsLogService);
                    return seconds+"";
                }
                //创建验证码
                var checkCode = CommonHelper.GetCheckCode();
                //创建短信内容
                var result = SendMsg(string.Format(smsContent, checkCode), para, smsCenterParas, smsLogService);
                //发送成功，将验证码记录插入数据库
                if (result > 0)
                {
                    return checkCode;
                }
                else
                {
                    return "发送失败！";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
        public static JsonResultData MatchCheckCode(string mobile, string checkCode, CheckFunc func)
        {
            //验证码验证
            var _sysCheckCodeService = DependencyResolver.Current.GetService<ISysCheckCodeService>();
            //验证码剩余禁用时间
            int seconds = 0;
            //获取验证码
            var result = _sysCheckCodeService.GetCheckCode(CheckMethod.Mobile.ToString(), mobile, func.ToString(), out seconds);
            //如果验证码不存在，返回错误信息
            if (!result.Success) return result;
            //如果验证码存在，匹配是否相等
            if (result.Data.ToString() != checkCode) return JsonResultData.Failure("验证码不正确");
            //如果匹配成功，则删除已匹配验证码
            var deleteCheck_result = _sysCheckCodeService.DeleteCheckCode(CheckMethod.Mobile.ToString(), mobile, func.ToString());
            return deleteCheck_result;
        }
        /// <summary>
        /// 使用酒店账号发送酒店业务短信
        /// 由参数类型来决定具体的业务短信内容
        /// </summary>
        /// <param name="para">发送短信参数</param>
        /// <param name="smsCenterParas">中央库中的短信配置参数信息</param>
        /// <param name="smsLogService">用于记录短信发送记录</param>
        /// <returns>发送结果</returns>
        public static JsonResultData SendHotelMessage(SMSSendParaHotel para,Dictionary<string,string> smsCenterParas,ISmsLogService smsLogService)
        {
            try
            {
                var content = para.GetSendContent();
                var sendResult = SendMsg(content,para,smsCenterParas,smsLogService);
                if (sendResult > 0)
                {
                    return JsonResultData.Successed("短信发送成功");
                }
                return JsonResultData.Failure(GetResult(sendResult));
            }
            catch(Exception ex)
            {
                return JsonResultData.Failure(ex);
            }           
        }
        private static string GetUrl(string opt)
        {
            switch(opt)
            {
                case "短信发送": return "smsSend.do";
                case "个性短信": return "sendData.do";
                case "余额查询": return "balanceQuery.do";
                case "修改密码": return "passwordUpdate.do";
            }
            return "";
        }
    }
}