using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.RoomStatusManage;
using Gemstar.BSPMS.Hotel.Web.Areas.RoomState.Models.Future;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Gemstar.BSPMS.Hotel.Web.Areas.RoomState.Models.Notify;

namespace Gemstar.BSPMS.Hotel.Web.Areas.RoomState.Controllers
{
    [AuthPage("10001")]
    public class NotifyController : BaseController
    {
        #region 查询
        [AuthButton(AuthFlag.Open)]
        public ActionResult Index(string beginDate, string endDate, string roomId = "", int status = 0,string wakeCallTypeName="")
        {
            if(endDate == "结束时间")
            {
                var service = GetService<IWakeCallService>();
                double time = service.NotifyTimeBef(CurrentInfo.HotelId);
                endDate = DateTime.Now.AddMinutes(time).ToString("yyyy-MM-dd HH:mm");
            }
            ViewBag.endDate = endDate;
            SetCommonQueryValues("up_queryNotify", "@hid=" + CurrentInfo.HotelId + "&@roomId=" + roomId + "&@userId=" + CurrentInfo.UserId + "&@notifyDateBegin=" + beginDate + "&@notifyDateEnd=" + endDate + "&@status=" + status + "&@wakeCallTypeName=" + wakeCallTypeName);
            return View();
        }
        #endregion

        #region 处理
        //设为已读
        [AuthButton(AuthFlag.None)]
        public JsonResult BatchRead(string id)
        {
            var service = GetService<IWakeCallService>();
            return Json(service.BatchRead(id, CurrentInfo.UserId, 1, ""), JsonRequestBehavior.AllowGet);
        }
        //设为未读
        //[AuthButton(AuthFlag.None)]
        //public JsonResult BatchNoRead(string id)
        //{
        //    var service = GetService<IWakeCallService>();
        //    return Json(service.BatchRead(id, CurrentInfo.UserId, 0, ""), JsonRequestBehavior.AllowGet);
        //}
        //作废
        [AuthButton(AuthFlag.None)]
        public ActionResult BatchCancel(string id)
        {
            ViewBag.ids = id;
            return PartialView("_BatchCancel");
        }
        //作废
        [AuthButton(AuthFlag.None)]
        public ActionResult BatchCancels(string id, string remarks)
        {
            var service = GetService<IWakeCallService>();
            return Json(service.BatchRead(id, CurrentInfo.UserId, 3, remarks), JsonRequestBehavior.AllowGet);
        }

        //处理
        [AuthButton(AuthFlag.None)]
        public ActionResult DealNotify(string id)
        {
            ViewBag.ids = id;
            return PartialView("_DealNotify");
        }
        //处理
        [AuthButton(AuthFlag.None)]
        public ActionResult DealNotifys(string id, string remarks)
        {
            var service = GetService<IWakeCallService>();
            return Json(service.BatchRead(id, CurrentInfo.UserId, 2, remarks), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 详情
        [AuthButton(AuthFlag.None)]
        public ActionResult Detail(string id)
        {
            NotifyDeatilViewModel resultEntity = new NotifyDeatilViewModel();
            resultEntity.Id = id;
            var service = GetService<IWakeCallService>();
            var entity = service.Get(Guid.Parse(id));
            if(entity != null)
            {
                resultEntity.RoomNo = entity.RoomNo;
                resultEntity.Cteater = entity.Creater;
                resultEntity.Content = entity.Content;
                resultEntity.Remarks = entity.Remark;
                resultEntity.NotifyTime = entity.CallTime == null ? "" : Convert.ToDateTime(entity.CallTime).ToString("yyyy-MM-dd HH:mm");
                resultEntity.CteateTime = entity.CreateTime == null ? "" : Convert.ToDateTime(entity.CreateTime).ToString("yyyy-MM-dd HH:mm");
                resultEntity.InvalidMan = entity.InvalidMan?? "";
                resultEntity.InvalidReason = entity.InvalidReason ?? "";
                resultEntity.InvalidTime = entity.InvalidTime == null ? "" : Convert.ToDateTime(entity.InvalidTime).ToString("yyyy-MM-dd HH:mm");
                resultEntity.Refno = entity.Refno ?? "";
                resultEntity.WakeCallTypeName = entity.WakeCallTypeName;
            }
            var detil = service.GetDetil(1, Guid.Parse(id));
            if(detil == null)
            {
                if (entity.Status == 1)
                {
                    resultEntity.Status = "未读";
                }
            }
            else
            {
                if(entity.Status == 1) { 
                    resultEntity.Status = "已接单";
                }
                resultEntity.Reader = detil.Reader ?? "";
                resultEntity.ReadTime = Convert.ToDateTime(detil.ReadTime).ToString("yyyy-MM-dd HH:mm");
            }
            if(entity.Status != 1)
            {
                var udetil = service.GetDetil(2, Guid.Parse(id));
                if (udetil != null)
                {
                    resultEntity.DealContent = udetil.DealContent;
                    resultEntity.DealMan = udetil.DealMan;
                    resultEntity.DealTime = udetil.DealTime == null ? "" : Convert.ToDateTime(udetil.DealTime).ToString("yyyy-MM-dd HH:mm");
                }
                resultEntity.Status = entity.Status == 2 ? "已处理" : "已作废";
            }
            return PartialView("_Detail", resultEntity);
        }
        #endregion
    }
}