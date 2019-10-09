using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.UI;
using Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Models.RoomHold;
using Kendo.Mvc.Extensions;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.NotifyManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Controllers
{
    /// <summary>
    /// 保留房设置
    /// </summary>
    [AuthPage("61020")]
    [BusinessType("预订须知")]
    public class RoomHoldController : BaseEditInWindowController<RoomHold, IRoomHoldService>
    {

        #region 查询
        //// GET: MarketingManage/RoomHold
        //[AuthButton(AuthFlag.Query)]
        //public ActionResult Index()
        //{
        //    var currentInfo = GetService<ICurrentInfo>();
        //    var serv = GetService<IRoomHoldService>();
        //    ViewData["Channelid"] = serv.GetChannel(currentInfo.HotelId);
        //    ViewData["RoomTypeid"] = serv.GetRoomType(currentInfo.HotelId); 　
        //    ViewBag.HotelBusinessDate = GetService<Services.SystemManage.IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd");
        //    return View();
        //}
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(RoomHoldQueryModel queryModel)
        {
            if (!queryModel.BeginDate.HasValue)
            {
                queryModel.BeginDate = DateTime.Today;
            }
            if (!queryModel.Days.HasValue)
            {
                queryModel.Days = 15;
            }
            var serv = GetService<IRoomHoldService>();
            var currentInfo = GetService<ICurrentInfo>();
            ViewData["Channelid"] = serv.GetChannel(currentInfo.HotelId);
            return View(queryModel);
        }
        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult AjaxQuery([DataSourceRequest]DataSourceRequest request, RoomHoldQueryModel queryModel)
        {
            if (!queryModel.BeginDate.HasValue)
            {
                queryModel.BeginDate = DateTime.Today;
            }
            if (!queryModel.Days.HasValue)
            {
                queryModel.Days = 15;
            }

            var services = GetService<IRoomHoldService>();
            var roomStatus = services.QueryRoomHoldInfos(CurrentInfo.HotelId, queryModel.ChannelId, queryModel.BeginDate.Value, queryModel.Days.Value);
            return Json(roomStatus.ToDataSourceResult(request));
        }
        #endregion

        #region 查询所选条件的保留房信息
        // GET: MarketingManage/RoomHold
        /// <summary>
        /// 查询所选条件的保留房信息
        /// </summary>
        /// <param name="channelid">渠道编号</param>
        /// <param name="roomtype">房间类型</param>
        /// <param name="year">年份</param>
        /// <param name="month">月</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult getData(string channelid, string roomtype, int year, int month)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var serv = GetService<IRoomHoldService>();
            List<RoomHold> rh = serv.GetRoomHold(currentInfo.HotelId, channelid, roomtype, year, month);
            return Json(rh, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 修改保留房设置 
        // GET: MarketingManage/RoomHold
        [AuthButton(AuthFlag.Update)]
        public ActionResult changeRoomhold(string begintime, string endtime, string strarr, string channelid, string roomtype,string channelname,string roomtypename)
        {
            DateTime HotelBusinessDate = DateTime.Parse(GetService<Services.SystemManage.IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd"));
            if (DateTime.Parse(begintime).AddDays(1) < HotelBusinessDate || DateTime.Parse(endtime).AddDays(1) < HotelBusinessDate)
            {
                return Json("营业日之前的日期不能修改保留房信息！", JsonRequestBehavior.AllowGet);
            }
            var currentInfo = GetService<ICurrentInfo>();
            var serv = GetService<IRoomHoldService>();
            string rh = serv.UpdateRoomHold(begintime, endtime, strarr, channelid, roomtype, currentInfo.HotelId);
            //发送保留房变更通知
            var service = GetService<INotifyService>();
            service.NotifyOtaRoomQty(currentInfo.HotelId, MvcApplication.IsTestEnv, channelid, roomtype, begintime, endtime);

            AddOperationLog(OpLogType.保留房设置, string.Format("渠道：{0}，房型：{1}，开始日期：{2}，结束日期：{3}，房数周一到周日分别为：{4}", channelname, roomtypename, begintime, endtime, strarr.Trim(',').Replace(",", "、")), "");
            return Json(rh, JsonRequestBehavior.AllowGet);
        }
        #endregion

        /// <summary>
        /// 获取房间类型的下拉框值
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult listItemForRoomType()
        {
            var currentInfo = GetService<ICurrentInfo>();
            var serv = GetService<IChannelService>();
            List<RoomType> alist = serv.GetRoomType(currentInfo.HotelId,true).Where(w=>w.IsNotRoom!=true).OrderBy(w => w.Seqid).ToList();
            Collection<SelectListItem> list = new Collection<SelectListItem>();
            list.Add(new SelectListItem() { Value = "all", Text = "全部" });
            foreach (var item in alist)
            {
                list.Add(new SelectListItem() { Value = item.Id.ToString(), Text = item.Name.ToString() });
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}