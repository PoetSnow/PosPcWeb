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

namespace Gemstar.BSPMS.Hotel.Web.Areas.RoomState.Controllers
{
    [AuthPage("10005")]
    public class FutureController : BaseController
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(FutureQueryModel queryModel)
        {
            if (!queryModel.BeginDate.HasValue)
            {
                queryModel.BeginDate = DateTime.Today;
            }
            if (!queryModel.Days.HasValue)
            {
                queryModel.Days = 15;
            }
            ViewBag.Hotelid = CurrentInfo.HotelId;
            ViewBag.HotelName = CurrentInfo.HotelName;
            return View(queryModel);
        }
        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult AjaxQuery([DataSourceRequest]DataSourceRequest request, FutureQueryModel queryModel)
        {
            if (!queryModel.BeginDate.HasValue)
            {
                queryModel.BeginDate = DateTime.Today;
            }
            if (!queryModel.Days.HasValue)
            {
                queryModel.Days = 15;
            }

            var services = GetService<IRoomStatusService>();
            var roomStatus = services.GetCurrentRoomStatusByDate(CurrentInfo.HotelId,queryModel.BeginDate.Value ,queryModel.Days.Value);
            return Json(roomStatus.ToDataSourceResult(request));
        }
        #endregion
    }
}