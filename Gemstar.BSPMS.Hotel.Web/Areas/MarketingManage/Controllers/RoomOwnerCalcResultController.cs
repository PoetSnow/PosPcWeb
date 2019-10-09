using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Web.Models;
using Gemstar.BSPMS.Common.Tools;
using Kendo.Mvc.UI;
using Gemstar.BSPMS.Hotel.Services.Common;
using Kendo.Mvc.Extensions;
using System.Data;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Controllers
{
    /// <summary>
    /// 业主报表数据
    /// </summary>
    [AuthPage("61091005")]
    [BusinessType("业主报表数据")]
    public class RoomOwnerCalcResultController : BaseEditInWindowController<RoomOwnerCalcResult, IRoomOwnerCalcResultService>
    {
        // GET: MarketingManage/RoomOwnerFee
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(string dtime = "")
        {
            ViewData["dtime"] = dtime; //dtime.ToString().ToString("yyyy-mm"); 
                                       //  ViewBag.pubstatus = GetService<IRoomOwnerCalcResultService>().getStatusPublishCalcResult(CurrentInfo.HotelId, dtime);

            return View();
        }
        /// <summary>
        /// 重新生成业主报表数据
        /// </summary> 
        /// <returns></returns>
        [AuthButton(AuthFlag.Add)]
        public ActionResult Regenerate(DateTime dt)
        {
            var result = GetService<IRoomOwnerCalcResultService>().regenerateRoomOwnerCalcResult(CurrentInfo.HotelId, dt);
            AddOperationLog(OpLogType.业主分成报告生成, "重新生成" + dt.GetDateTimeFormats('y')[0].ToString() + "的业主分成报告");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region 执行ajax数据查询
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Query)]
        public ActionResult AjaxQuery(string calcDate, [DataSourceRequest]DataSourceRequest request)
        {
            var queryService = GetService<ICommonQueryService>();
            var procedure = "up_list_RoomOwnerCalcResult";
            var queryParameters = new CommonQueryHelper(string.Format("@h99hid={0}^{0}&@datetime={1}^{1}", CurrentInfo.HotelId, calcDate));
            var paraValues = queryParameters.GetParameters();
            var dt = queryService.ExecuteQuery(procedure, paraValues);
            var data = Json(dt.ToDataSourceResult(request));
            data.MaxJsonLength = int.MaxValue;
            return data;
        }
        #endregion

        [HttpPost]
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.None)]
        public ActionResult getPubStatus(DateTime dt)
        {
            string result = GetService<IRoomOwnerCalcResultService>().getStatusPublishCalcResult(CurrentInfo.HotelId, dt);
            return Json(JsonResultData.Successed(result));
        }
        [HttpPost]
        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.None)]
        public ActionResult setPubStatus(bool ispublish, DateTime dt)
        {
            JsonResultData result = GetService<IRoomOwnerCalcResultService>().setStatusPublishCalcResult(CurrentInfo.HotelId, dt, ispublish);
            return Json(result);
        }
    }
}