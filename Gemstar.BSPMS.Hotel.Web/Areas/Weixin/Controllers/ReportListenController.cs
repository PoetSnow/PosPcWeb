using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP.Entities.Request;
using Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models;
using Senparc.Weixin.MP;
using Gemstar.BSPMS.Common.Services.EF;
using System.Data.SqlClient;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Controllers
{
    /// <summary>
    /// 负责接收报表推送的任何事件和交互
    /// </summary>
    [NotAuth]
    public class ReportListenController : BaseWeixinController
    {
        // GET: Weixin/ReportListen
        public ActionResult Index(string id)
        {
            try
            {
                //获取参数
                Guid weixinTemplateMessageId = Guid.Empty;
                string hid = null;
                byte type = 0;
                bool isTrue = GetWeixinTemplateMessageIdAndHotelId(id, out weixinTemplateMessageId, out hid, out type);
                if (isTrue == false)
                {
                    return Content("0§参数不正确");
                }

                var db = GetHotelDb(hid);
                if(db == null)
                {
                    return Content("0§参数不正确");
                }
                var entity = db.Database.SqlQuery<Services.WeixinManage.TemplateMessageInfo.SendReportFormTemplateMessageModel>("exec up_DayBsnsReprotWxGet @hid=@hid,@id=@id", new SqlParameter("@hid", hid), new SqlParameter("@id", weixinTemplateMessageId)).FirstOrDefault();
                if(entity == null)
                {
                    return Content("0§参数不正确");
                }

                #region url参数编码，[§要编码的内容§]
                string url = entity.Url;
                if (!string.IsNullOrWhiteSpace(url))
                {
                    bool isEncodeComplete = false;
                    while (!isEncodeComplete)
                    {
                        int start = url.IndexOf("[§", 0);
                        int end = url.IndexOf("§]", 0);
                        if(start > -1 && end > -1 && start < end)
                        {
                            var para = url.Substring(start, end - start + 2);
                            var para_encode = System.Web.HttpUtility.UrlEncode(para.Replace("[§", "").Replace("§]", ""));

                            var oldValue = url.Substring(0, end + 2);
                            var newValue = url.Substring(0, start) + para_encode;

                            url = url.Replace(oldValue, newValue);
                        }
                        else
                        {
                            isEncodeComplete = true;
                        }
                    }
                }
                entity.Url = url;
                #endregion

                var result = TemplateMessageHelper.SendReportFormTemplateMessage(entity);
                if (result != null && result.Success)
                {
                    return Content("1§");
                }
                return Content("0§"+ result.Data);
            }
            catch (Exception ex)
            {
                return Content("0§" + ex.Message);
            }
            return Content("0§");
        }

        /// <summary>
        /// 获取微信模板消息主键ID和酒店ID
        /// </summary>
        /// <param name="id">参数ID</param>
        /// <param name="weixinTemplateMessageId">微信模板消息ID</param>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">授权类型（4营业简报推送）</param>
        /// <returns></returns>
        private bool GetWeixinTemplateMessageIdAndHotelId(string id, out Guid weixinTemplateMessageId, out string hid, out byte type)
        {
            weixinTemplateMessageId = Guid.Empty;
            hid = null;
            type = 0;
            if (!string.IsNullOrWhiteSpace(id))
            {
                if (id.Length > 37)
                {
                    Guid.TryParse(id.Substring(0, 36), out weixinTemplateMessageId);
                    string hid_type = id.Substring(37);
                    var hid_type_list = hid_type.Split('-');
                    if (hid_type_list != null && hid_type_list.Length == 2)
                    {
                        hid = hid_type_list[0];
                        byte.TryParse(hid_type_list[1], out type);
                    }
                }
                if (weixinTemplateMessageId != null && weixinTemplateMessageId != Guid.Empty && !string.IsNullOrWhiteSpace(hid) && type == 4)
                {
                    return true;
                }
            }
            return false;
        }
    }
}