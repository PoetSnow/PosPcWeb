using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.IO;
using Gemstar.BSPMS.Common.Tools;
using System.Data;
using NPOI.SS.UserModel;
using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PermanentRoom.Controllers
{
    /// <summary>
    /// 长包房
    /// </summary>
    [AuthPage("21020")]
    public class PermanentRoomController : BaseController
    {
        /// <summary>
        /// 长包房设置GET
        /// </summary>
        /// <param name="regid">账号</param>
        /// <returns></returns>
        [HttpGet]
        [AuthButton(AuthFlag.Update)]
        public ActionResult PermanentRoomSet(string regid)
        {
            string hid = CurrentInfo.HotelId;
            if (string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(regid))
            {
                return Content("参数错误！");
            }
            var permanentRoomService = GetService<IPermanentRoomService>();
            bool isTrue = permanentRoomService.IsPermanentRoom(hid, regid);
            if (!isTrue)
            {
                return Content("请先保存客情后再来设置长租房！");
            }
            ViewBag.Hid = hid;
            ViewBag.Regid = regid;
            var resDetailEntity = permanentRoomService.GetPermanentRoomOrder(hid, regid);
            if(resDetailEntity != null)
            {
                ViewBag.RoomNo = resDetailEntity.RoomNo;
                ViewBag.Guestname = resDetailEntity.Guestname;
                if (!string.IsNullOrWhiteSpace(resDetailEntity.RoomNo))
                {
                    var lastMeterEntity = permanentRoomService.UpGetPermanentRoomLastWaterAndElectricity(hid, resDetailEntity.RoomNo);
                    if(lastMeterEntity != null)
                    {
                        if(lastMeterEntity.Water > 0) { ViewBag.Water = lastMeterEntity.Water; }
                        if(lastMeterEntity.Watt > 0) { ViewBag.Watt = lastMeterEntity.Watt; }
                        if(lastMeterEntity.Gas > 0) { ViewBag.Gas = lastMeterEntity.Gas; }
                    }
                }
            }
            return View();
        }

        /// <summary>
        /// 长包房设置
        /// </summary>
        /// <param name="regid">账号</param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetPermanentRoomSet(string regid)
        {
            string hid = CurrentInfo.HotelId;
            if(string.IsNullOrWhiteSpace(hid) || string.IsNullOrWhiteSpace(regid))
            {
                return Json(JsonResultData.Failure("参数错误！"), JsonRequestBehavior.DenyGet);
            }
            var result = GetService<IPermanentRoomService>().Get(CurrentInfo.HotelId, regid);
            return Json(JsonResultData.Successed(result), JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 长包房保存
        /// </summary>
        /// <param name="model">数据</param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Update)]
        public ActionResult SavePermanentRoomSet(PermanentRoomInfo.PermanentRoomSetPara model)
        {
            var result = GetService<IPermanentRoomService>().Save(CurrentInfo, model);
            return Json(result, JsonRequestBehavior.DenyGet);
        }

        #region 其他
        /// <summary>
        /// 获取消费项目
        /// </summary>
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetItems()
        {
            var items = GetService<IItemService>().Query(CurrentInfo.HotelId, "D", "");
            var datas = items.Select(c => new { Id = c.Id, Name = c.Code + "-" + c.Name }).ToList();
            return Json(datas, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}