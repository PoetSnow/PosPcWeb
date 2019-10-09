using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.ResManage;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Controllers
{
    /// <summary>
    /// 预订管理
    /// </summary>
    [AuthPage("20010")]
    public class ResOrdersController : BaseController
    {
        #region 预订列表
        /// <summary>
        /// 预订列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            var hotelStatusService = GetService<IHotelStatusService>();
            var businessDate = hotelStatusService.GetBusinessDate(CurrentInfo.HotelId);

            var today = DateTime.Today.ToDateString();
            var aMonthAgo = DateTime.Today.AddMonths(-1).ToDateString();

            ViewBag.businessToday = businessDate.ToDateString();
            ViewBag.today = today;
            ViewBag.aMonthAgoToday = aMonthAgo;
            return View();
        }

        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Query)]
        public ActionResult IndexAjax([DataSourceRequest]DataSourceRequest request, ResDetailQueryPara queryPara)
        {
            queryPara.HotelId = CurrentInfo.HotelId;
            if (queryPara.RoomType != null)
            {
                queryPara.RoomType = queryPara.RoomType.Where((item) => { if (string.IsNullOrWhiteSpace(item)) { return false; } else { return true; } }).Distinct().ToList();
                if (queryPara.RoomType.Count <= 0)
                {
                    queryPara.RoomType = null;
                }
            }
            var services = GetService<IResService>();
            var orderDetails = services.QueryResDetails(queryPara);
            return Json(orderDetails.ToDataSourceResult(request));
        }
        #endregion

        #region 预订维护
        [AuthButton(AuthFlag.Query)]
        public ActionResult Edit()
        {

            return View();
        }
        #endregion
    }
}