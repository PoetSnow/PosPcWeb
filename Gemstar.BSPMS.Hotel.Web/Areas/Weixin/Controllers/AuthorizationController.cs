using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Senparc.Weixin.MP.MvcExtension;
using Senparc.Weixin.MP.Entities.Request;
using Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models;
using Senparc.Weixin.MP;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Common.Services.EF;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Controllers
{
    /// <summary>
    /// 授权
    /// </summary>
    [NotAuth]
    public class AuthorizationController : Controller
    {
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="id">[WeixinTemplateMessageId + HotelId + Type]</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(string id)
        {
            return View(GetEntity(id));
        }

        /// <summary>
        /// 授权提交
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">主键ID</param>
        /// <param name="type">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权）</param>
        /// <param name="weixinTemplateMessageId">微信模板主键ID</param>
        /// <param name="authSuccess">true同意，false不同意</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string hid, Guid id, byte type, Guid weixinTemplateMessageId, bool authSuccess)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hid) || id == null || id == Guid.Empty || !AuthorizationInfo.CheckAuthType(type) || weixinTemplateMessageId == null || weixinTemplateMessageId == Guid.Empty)
                {
                    return View();
                }

                //获取数据库
                var centerDb = DependencyResolver.Current.GetService<DbCommonContext>();
                if (!centerDb.WeixinTemplateMessages.AsNoTracking().Where(c => c.Hid == hid && c.Id == weixinTemplateMessageId && c.Type == type).Any())
                {
                    return null;
                }
                var hotelDb = new DbHotelPmsContext(MvcApplication.GetHotelDbConnStr(centerDb, hid), hid, "weixinEvent", null);

                #region
                //var weixinTemplateMessageEntity = hotelDb.WeixinTemplateMessages.AsNoTracking().Where(c => c.Hid == hid && c.Id == weixinTemplateMessageId).FirstOrDefault();
                //if (weixinTemplateMessageEntity == null)
                //{
                //    return null;
                //}
                //if (!(weixinTemplateMessageEntity.SendStatus == "0" && weixinTemplateMessageEntity.SendFinishStatus == "success"))
                //{
                //    return null;
                //}
                //Guid authId = weixinTemplateMessageEntity.Keyid;
                //if (id != authId)
                //{
                //    return null;
                //}

                //var entity = hotelDb.AuthorizationRecords.Where(c => c.Hid == hid && c.Id == id && c.Type == type && c.Mode == 2 && c.AuthStatus == 0).FirstOrDefault();
                //if (entity != null)
                //{
                //    if (entity.RevokeDate != null && entity.RevokeDate.HasValue == true)
                //    {
                //        return null;
                //    }
                //    if (entity.CreateDate.AddMinutes(10) < DateTime.Now)
                //    {
                //        return null;
                //    }

                //    entity.AuthStatus = (authSuccess == true ? (byte)1 : (byte)2);
                //    entity.AuthDate = DateTime.Now;
                //    hotelDb.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                //    hotelDb.SaveChanges();
                //}
                #endregion

                hotelDb.Database.ExecuteSqlCommand("exec up_weixinAuthorization_update @h99hid=@h99hid,@authorizationId=@authorizationId,@weixinTemplateMessageId=@weixinTemplateMessageId,@type=@type,@authSuccess=@authSuccess"
                    , new System.Data.SqlClient.SqlParameter("@h99hid", hid)
                    , new System.Data.SqlClient.SqlParameter("@authorizationId", id)
                    , new System.Data.SqlClient.SqlParameter("@weixinTemplateMessageId", weixinTemplateMessageId)
                    , new System.Data.SqlClient.SqlParameter("@type", type)
                    , new System.Data.SqlClient.SqlParameter("@authSuccess", authSuccess)
                    );

                return View(GetEntity(string.Format("{0}-{1}-{2}", weixinTemplateMessageId, hid, type)));
            }
            catch (Exception ex) { ViewBag.Msg = ex.Message; }
            return View();
        }

        /// <summary>
        /// 获取授权信息
        /// </summary>
        /// <param name="id">[WeixinTemplateMessageId + HotelId + Type]</param>
        /// <returns></returns>
        public AuthorizationModel GetEntity(string id)
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
                    return null;
                }

                //获取数据库
                var centerDb = DependencyResolver.Current.GetService<DbCommonContext>();
                if(!centerDb.WeixinTemplateMessages.AsNoTracking().Where(c => c.Hid == hid && c.Id == weixinTemplateMessageId && c.Type == type).Any())
                {
                    return null;
                }
                var hotelDb = new DbHotelPmsContext(MvcApplication.GetHotelDbConnStr(centerDb, hid), hid, "weixinEvent", null);

                #region
                //var weixinTemplateMessageEntity = hotelDb.WeixinTemplateMessages.AsNoTracking().Where(c => c.Hid == hid && c.Id == weixinTemplateMessageId).FirstOrDefault();
                //if (weixinTemplateMessageEntity == null)
                //{
                //    return null;
                //}
                //if (!(weixinTemplateMessageEntity.SendStatus == "0" && weixinTemplateMessageEntity.SendFinishStatus == "success"))
                //{
                //    return null;
                //}
                //Guid authId = weixinTemplateMessageEntity.Keyid;

                ////获取数据
                //string hotelName = hotelDb.PmsHotels.AsNoTracking().Where(c => c.Hid == hid).Select(c => c.Name).FirstOrDefault();
                //if (string.IsNullOrWhiteSpace(hotelName))
                //{
                //    return null;
                //}

                //var entity = hotelDb.AuthorizationRecords.AsNoTracking().Where(c => c.Hid == hid && c.Id == authId && c.Type == type && c.Mode == 2).FirstOrDefault();
                //if (entity == null)
                //{
                //    return null;
                //}
                //if(entity.RevokeDate != null && entity.RevokeDate.HasValue == true)
                //{
                //    return null;
                //}
                //if (entity.CreateDate.AddMinutes(10) < DateTime.Now)
                //{
                //    return null;
                //}

                //List<Guid> userids = new List<Guid> { entity.CreateUserId, entity.AuthUserId };
                //var userList = hotelDb.PmsUsers.AsNoTracking().Where(w => w.Grpid == hid && userids.Contains(w.Id)).Select(c => new { c.Id, c.Name }).ToList();
                //if (userList == null || userList.Count != 2)
                //{
                //    return null;
                //}
                //string createUserName = userList.Where(c => c.Id == entity.CreateUserId).Select(c => c.Name).FirstOrDefault();
                //string authUserName = userList.Where(c => c.Id == entity.AuthUserId).Select(c => c.Name).FirstOrDefault();
                //if (string.IsNullOrWhiteSpace(createUserName) || string.IsNullOrWhiteSpace(authUserName))
                //{
                //    return null;
                //}
                #endregion

                var entity = hotelDb.Database.SqlQuery<Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Models.AuthorizationRecord>("exec up_weixinAuthorization_get @h99hid=@h99hid,@weixinTemplateMessageId=@weixinTemplateMessageId,@type=@type"
                    , new System.Data.SqlClient.SqlParameter("@h99hid", hid)
                    , new System.Data.SqlClient.SqlParameter("@weixinTemplateMessageId", weixinTemplateMessageId)
                    , new System.Data.SqlClient.SqlParameter("@type", type)
                    ).FirstOrDefault();

                string title = AuthorizationInfo.GetAuthTypeName(entity.Type);
                object authContent = AuthorizationInfo.GetAuthContent(entity.Type, entity.Content);
                if (string.IsNullOrWhiteSpace(title) || authContent == null)
                {
                    return null;
                }
                
                return new AuthorizationModel
                {
                    Title = title,
                    AuthContent = authContent,
                    AuthorizationRecord = entity,
                    WeixinTemplateMessageId = weixinTemplateMessageId,
                };

            }
            catch (Exception ex){ ViewBag.Msg = ex.Message; }
            return null;
        }

        /// <summary>
        /// 获取微信模板消息主键ID和酒店ID
        /// </summary>
        /// <param name="id">参数ID</param>
        /// <param name="weixinTemplateMessageId">微信模板消息ID</param>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权）</param>
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
                    if(hid_type_list != null && hid_type_list.Length == 2)
                    {
                        hid = hid_type_list[0];
                        byte.TryParse(hid_type_list[1], out type);
                    }
                }
                if (weixinTemplateMessageId != null && weixinTemplateMessageId != Guid.Empty && !string.IsNullOrWhiteSpace(hid) && AuthorizationInfo.CheckAuthType(type))
                {
                    return true;
                }
            }
            return false;
        }
    }
}