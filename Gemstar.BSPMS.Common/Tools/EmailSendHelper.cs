
using Gemstar.BSPMS.Common.Enumerator;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Common.Tools
{
    public static class EmailSendHelper
    {

        public static JsonResultData MatchCheckCode(string email, string checkCode, CheckFunc func)
        {
            //验证码验证
            var _sysCheckCodeService = DependencyResolver.Current.GetService<ISysCheckCodeService>();
            //验证码剩余禁用时间
            int seconds = 0;
            //获取验证码
            var result = _sysCheckCodeService.GetCheckCode(CheckMethod.Email.ToString(), email, func.ToString(), out seconds);
            //如果验证码不存在，返回错误信息
            if (!result.Success) return result;
            //如果验证码存在，匹配是否相等
            if (result.Data.ToString() != checkCode) return JsonResultData.Failure("验证码不正确");
            //如果匹配成功，则删除已匹配验证码
            var deleteCheck_result = _sysCheckCodeService.DeleteCheckCode(CheckMethod.Email.ToString(), email, func.ToString());
            return deleteCheck_result;
        }

        public static JsonResultData SendCheckCode(EmailModel model, string func, EmailTemplate temp)
        {
            if (!RegexHelper.IsRightEmail(model.ToAddress)) return JsonResultData.Failure("邮箱格式不正确");
            var _sysCheckCodeService = DependencyResolver.Current.GetService<ISysCheckCodeService>();
            int seconds = 0;
            var checkResult = _sysCheckCodeService.GetCheckCode(CheckMethod.Email.ToString(), model.ToAddress, func, out seconds);
            if (checkResult.Success)
            {
                if (model.BodyPara.ContainsKey("CheckCode"))
                    model.BodyPara["CheckCode"] = checkResult.Data.ToString();
                else
                    model.BodyPara.Add("CheckCode", checkResult.Data.ToString());
                SendEmail(model, temp.ToString());
                JsonResultData.Successed(seconds);
            }

            var checkCode = CommonHelper.GetCheckCode();

            model.BodyPara.Add("CheckCode", checkCode);
            var result = SendEmail(model, temp.ToString());
            if (result.Success)
            {
                var sysCheckCode = new SysCheckCode();
                sysCheckCode.Hid = "";
                sysCheckCode.UserCode = "";
                sysCheckCode.GetMethod = CheckMethod.Email.ToString();
                sysCheckCode.GetMethodValue = model.ToAddress;
                sysCheckCode.CheckCode = checkCode;
                sysCheckCode.EndDate = DateTime.Now.AddSeconds(90);
                sysCheckCode.Func = func;
                var addResult = _sysCheckCodeService.AddCheckCode(sysCheckCode);
                return addResult;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="template"></param>
        public static JsonResultData SendEmail(EmailModel model, string template = "Default")
        {
            var _smsLogService = DependencyResolver.Current.GetService<ISmsLogService>();
            string strbody = "";
            try
            {
                var _sysParaService = DependencyResolver.Current.GetService<ISysParaService>();
                var para = _sysParaService.GetEmailSendPara();
                var email_server = para["emailsendserver"];
                var email_port = para["emailsendport"];
                var email_from = para["emailsendusername"];
                var email_pass = para["emailsendpassword"];
                if (email_server.IsNullOrEmpty() || email_port.IsNullOrEmpty() || email_from.IsNullOrEmpty() || email_pass.IsNullOrEmpty())
                {
                    return JsonResultData.Failure("邮件发送配置参数不正确");
                }
                model.BodyPara.Add("Remark", model.Remark);
                template = string.Format("Gemstar.BSPMS.Hotel.Web\\Content\\Template\\{0}.htm", template);
                // 建立一个邮件实体     
                MailAddress from = new MailAddress(email_from, model.FromName, Encoding.UTF8);
                MailAddress to = new MailAddress(model.ToAddress);
                MailMessage message = new MailMessage(from, to);
                try
                {
                    strbody = ReplaceText(model.BodyPara, template);
                }
                catch
                {
                    template = string.Format("PMSBeta\\Content\\Template\\{0}.htm", "ResetPassword");
                    strbody = ReplaceText(model.BodyPara, template);
                }
                message.IsBodyHtml = true;
                message.BodyEncoding = Encoding.UTF8;
                message.Priority = MailPriority.High;
                message.Body = strbody;  //邮件BODY内容    
                message.Subject = model.Subject;

                SmtpClient smtp = new SmtpClient();
                //smtp.EnableSsl = true;
                smtp.Host = email_server;
                smtp.Port = int.Parse(email_port);
                smtp.Credentials = new NetworkCredential(email_from, email_pass);
                smtp.Send(message); //发送邮件 


                _smsLogService.AddLog(model.ToAddress, "【" + model.Subject + "】" + strbody, "发送成功");
                return JsonResultData.Successed();
            }
            catch (Exception ex)
            {
                _smsLogService.AddLog(model.ToAddress, "【" + model.Subject + "】" + strbody, ex.InnerException != null ? ex.InnerException.ToString() : ex.ToString());
                return JsonResultData.Failure(ex);
            }
        }

        private static string ReplaceText(Dictionary<string, string> para, string template)
        {
            string path = string.Empty;
            var baseDirectoryArr = AppDomain.CurrentDomain.BaseDirectory.SplitString("\\");
            var result = "";
            for (int i = 0; i < baseDirectoryArr.Length - 2; i++)
            {
                result += baseDirectoryArr[i] + "\\";
            }
            path = result + template;
            if (path == string.Empty)
            {
                return string.Empty;
            }
            StreamReader read = new StreamReader(path);
            string str = string.Empty;
            str = read.ReadToEnd();
            if (para != null)
            {
                foreach (var item in para)
                {
                    str = str.Replace("{~" + item.Key + "~}", item.Value);
                }
            }
            read.Dispose();
            return str;
        }
    }
}
