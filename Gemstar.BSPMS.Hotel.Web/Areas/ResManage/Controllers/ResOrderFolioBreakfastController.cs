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
using Gemstar.BSPMS.Hotel.Services.BreakfastManage;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Common.Services.Enums;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Gemstar.BSPMS.Hotel.Web.Models;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Controllers
{
    /// <summary>
    /// 电子早餐
    /// </summary>
    [AuthPage("10007")]
    public class ResOrderFolioBreakfastController : BaseController
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            var hotelInterfaces = GetService<IHotelInfoService>().GetHotelInterface(CurrentInfo.HotelId);
            var lockInterface = hotelInterfaces.SingleOrDefault(w => w.TypeCode == "01");//只取出设置的门锁读卡器接口信息
            ViewBag.lockType = lockInterface == null ? "" : lockInterface.TypeCode;
            ViewBag.lockCode = lockInterface == null ? "" : lockInterface.Code;
            ViewBag.lockEditionName = lockInterface == null ? "" : lockInterface.EditionName;
            return View();
        }

        [KendoGridDatasourceException]
        [AuthButton(AuthFlag.Query)]
        public ActionResult IndexAjax([DataSourceRequest]DataSourceRequest request)
        {
            var result = GetService<IBreakfastService>().Today(CurrentInfo.HotelId);
            return Json(result.ToDataSourceResult(request));
        }

        [AuthButton(AuthFlag.Add)]
        public ActionResult ToHaveBreakfastByCardNo(string cardNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cardNo))
                {
                    return Json(JsonResultData.Failure("卡号不能为空！"));
                }
                var result = GetService<IBreakfastService>().ToHaveBreakfastByCardNo(CurrentInfo.HotelId, cardNo);
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex), JsonRequestBehavior.DenyGet);
            }
        }

        [AuthButton(AuthFlag.Add)]
        public ActionResult ToHaveBreakfastByRoomNo(string roomNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(roomNo))
                {
                    return Json(JsonResultData.Failure("房号不能为空！"));
                }
                var result = GetService<IBreakfastService>().ToHaveBreakfastByRoomNo(CurrentInfo.HotelId, roomNo);
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex), JsonRequestBehavior.DenyGet);
            }
        }

    }
}