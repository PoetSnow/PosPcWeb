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
    /// <summary>
    /// 模板消息辅助类
    /// </summary>
    public class TemplateMessageHelper
    {
        private const string remark = "此消息来自捷云PMS";

        #region 授权提醒
        /// <summary>
        /// 发送授权提醒
        /// </summary>
        /// <returns></returns>
        public static JsonResultData SendAuthTemplateMessage(TemplateMessageInfo.SendAuthTemplateMessageModel sendAuthTemplateMessageModel)
        {
            try
            {
                string templateId = "ZRjc5XColreLqhQAtRy-1T3NKAU-s-9e5ml9YX-8tbY";//授权提醒 模板ID
                string url = string.Format("{0}?id={1}-{2}-{3}", sendAuthTemplateMessageModel.Url, sendAuthTemplateMessageModel.Id, sendAuthTemplateMessageModel.Hid, sendAuthTemplateMessageModel.Type);
                var templateContent = new AuthTemplateData()//详细内容
                {
                    first = new TemplateDataItem(sendAuthTemplateMessageModel.Title),//标题：房价修改授权申请
                    keyword1 = new TemplateDataItem(sendAuthTemplateMessageModel.HotelName),//酒店名称：住哲酒店
                    keyword2 = new TemplateDataItem(sendAuthTemplateMessageModel.AuthApplicant),//授权申请人：manager
                    keyword3 = new TemplateDataItem(sendAuthTemplateMessageModel.AuthContent),//授权内容：修改888号房间房价，由888改为666
                    keyword4 = new TemplateDataItem(sendAuthTemplateMessageModel.AuthApplicantDateTime.ToString("yyyy-MM-dd HH:mm")),//授权申请时间：2015-07-10 11:13
                    remark = new TemplateDataItem(remark),//如有疑问，请咨询客服。
                };
                var result = TemplateApi.SendTemplateMessage(MvcApplication.WeixinAppId, sendAuthTemplateMessageModel.Openid, templateId, url, templateContent);
                //var result = new Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage.SendTemplateMessageResult
                //{
                //    errcode = ReturnCode.请求成功,
                //    errmsg = "ok",
                //    msgid = new Random().Next(111111111, 999999999),
                //};
                //var result = new Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage.SendTemplateMessageResult
                //{
                //    errcode = ReturnCode.发送消息失败_该用户已被加入黑名单_无法向此发送消息,
                //    errmsg = "failed:user Blacklist",
                //    msgid = new Random().Next(111111111, 999999999),
                //};
                Guid id = SaveTemplateMessage(sendAuthTemplateMessageModel, result);
                if (result != null && result.errcode == ReturnCode.请求成功)
                {
                    return JsonResultData.Successed(id);
                }
                return JsonResultData.Failure((result == null) ? "发送微信失败" : result.errcode.ToString());
            }
            catch(Exception ex)
            {
                return JsonResultData.Failure("发送微信失败，原因：" + ex.Message);
            }
        }
        /// <summary>  
        /// 授权提醒模板 
        /// </summary>  
        private class AuthTemplateData
        {
            public TemplateDataItem first { get; set; }
            public TemplateDataItem keyword1 { get; set; }
            public TemplateDataItem keyword2 { get; set; }
            public TemplateDataItem keyword3 { get; set; }
            public TemplateDataItem keyword4 { get; set; }
            public TemplateDataItem remark { get; set; }
        }
        #endregion

        #region 报表提醒
        /// <summary>
        /// 发送报表提醒
        /// </summary>
        /// <returns></returns>
        public static JsonResultData SendReportFormTemplateMessage(TemplateMessageInfo.SendReportFormTemplateMessageModel sendReportFormTemplateMessageModel)
        {
            try
            {
                string templateId = "hI9x7S8pwkofKnOKbqoQRtiKT8koTTrrmiBg8s_zd6U";//报表提醒 模板ID
                string url = sendReportFormTemplateMessageModel.Url;
                var templateContent = new ReportFormTemplateData()//详细内容
                {
                    first = new TemplateDataItem(sendReportFormTemplateMessageModel.Title),//标题：酒店经营日报表为您送达
                    keyword1 = new TemplateDataItem(sendReportFormTemplateMessageModel.HotelName),//酒店名称：住哲酒店
                    keyword2 = new TemplateDataItem(sendReportFormTemplateMessageModel.BusinessDate.ToString("yyyy-MM-dd")),//营业日期：2015-07-10
                    keyword3 = new TemplateDataItem(sendReportFormTemplateMessageModel.ReportFormName),//报表名称：经营日报表
                    remark = new TemplateDataItem(remark),//点击查看报表详情，如有疑问，请联系客服。
                };
                var result = TemplateApi.SendTemplateMessage(MvcApplication.WeixinAppId, sendReportFormTemplateMessageModel.Openid, templateId, url, templateContent);
                Guid id = SaveTemplateMessage(sendReportFormTemplateMessageModel, result);
                if (result != null && result.errcode == ReturnCode.请求成功)
                {
                    return JsonResultData.Successed(id);
                }
                return JsonResultData.Failure((result == null) ? "发送微信失败" : result.errcode.ToString());
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure("发送微信失败，原因：" + ex.Message);
            }
        }
        /// <summary>
        /// 报表提醒模板
        /// </summary>
        private class ReportFormTemplateData
        {
            public TemplateDataItem first { get; set; }
            public TemplateDataItem keyword1 { get; set; }
            public TemplateDataItem keyword2 { get; set; }
            public TemplateDataItem keyword3 { get; set; }
            public TemplateDataItem remark { get; set; }
        }
        #endregion

        #region 保存消息
        /// <summary>
        /// 保存模板消息
        /// </summary>
        /// <returns></returns>
        private static Guid SaveTemplateMessage(TemplateMessageInfo.TemplateMessageModel templateMessageModel, Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage.SendTemplateMessageResult sendResult)
        {
            string msgid = "-2";
            string sendStatus = "-2";
            string sendMsg = "无返回内容";
            if(sendResult != null)
            {
                msgid = sendResult.msgid.ToString();
                sendStatus = ((Int32)sendResult.errcode).ToString();
                sendMsg = sendResult.errcode.ToString();
            }
            var centerDb = DependencyResolver.Current.GetService<DbCommonContext>();
            var hotelDb = new DbHotelPmsContext(MvcApplication.GetHotelDbConnStr(centerDb, templateMessageModel.Hid), templateMessageModel.Hid, "weixinEvent", null);
            centerDb.WeixinTemplateMessages.Add(new Gemstar.BSPMS.Common.Services.Entities.WeixinTemplateMessage
            {
                Id = templateMessageModel.Id,
                Hid = templateMessageModel.Hid,
                Type = templateMessageModel.Type,
                Openid = templateMessageModel.Openid,
                Msgid = msgid,
            });
            hotelDb.WeixinTemplateMessages.Add(new Gemstar.BSPMS.Hotel.Services.Entities.WeixinTemplateMessage
            {
                Id = templateMessageModel.Id,
                Hid = templateMessageModel.Hid,
                Keyid = templateMessageModel.Keyid,
                SendStatus = sendStatus,
                SendMsg = sendMsg,
                SendDate = DateTime.Now,
            });
            centerDb.SaveChanges();
            hotelDb.SaveChanges();
            return templateMessageModel.Id;
        }
        #endregion
    }
}