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

namespace Gemstar.BSPMS.Hotel.Web.Areas.PermanentRoom.Controllers
{
    /// <summary>
    /// 新预订
    /// </summary>
    [AuthPage("21020")]
    public class ResOrderAddController : BaseController
    {
        /// <summary>
        /// 预订或入住
        /// </summary>
        /// <param name="type">类型（R预订，I入住）</param>
        /// <param name="id">子单ID（为空，新订单。否则，维护订单）</param>
        /// <param name="parameters">参数</param>
        /// <param name="IsRoomStatus">IsRoomStatus用来判断哪些页面是否需要关闭子窗口刷新父窗口 
        /// 0:房态右键结账 1：默认 2：双击客单列表进入客情  3：房态图-新入住，客单操作 4：双击房态进入客账</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(string type, string id, string parameters, string IsRoomStatus = "1")
        {
            if (!IsPermanentRoom())//是否启用长租管理功能
            {
                return Content("没有启用长租管理功能");
            }
            if (string.IsNullOrWhiteSpace(type))
            {
                return Content("参数不正确");
            }
            type = type.ToUpper().Trim();
            if (type != "R" && type != "I")
            {
                return Content("参数不正确");
            }
            string title = type == "R" ? "预订" : "入住";
            title = (string.IsNullOrWhiteSpace(id) || IsPageAuth(this, id)) ? ("新" + title) : (title + "维护");
            ViewBag.Title = title;
            ViewBag.Type = type;
            ViewBag.RegId = id;
            ViewBag.Parameters = parameters;
            ViewBag.IsRoomStatus = IsRoomStatus;

            ViewBag.IsLog = GetService<IAuthCheck>().HasAuth(CurrentInfo.UserId, "20020", (Int64)AuthFlag.OrderLog, CurrentInfo.HotelId);

            return View("Index");
        }

        /// <summary>
        /// 预订
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult RIndex(string id, string parameters)
        {
            return Index("R", id, parameters);
        }

        /// <summary>
        /// 入住
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult IIndex(string id, string parameters)
        {
            return Index("I", id, parameters);
        }


        [AuthButton(AuthFlag.OrderLog)]
        public ActionResult Log(string resid)
        {
            ViewBag.ResId = resid;
            return View();
        }

        [AuthButton(AuthFlag.OrderLog)]
        [KendoGridDatasourceException]
        public ActionResult LogAjax([DataSourceRequest]DataSourceRequest request, int type, string id, string keywords)
        {
            string hid = CurrentInfo.HotelId;
            if (string.IsNullOrWhiteSpace(hid))
            {
                throw new Exception("酒店代码错误，请重新登录！");
            }
            if (type != 1 && type != 2)
            {
                throw new Exception("参数类型错误！");
            }
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new Exception("ID错误，不能为空！");
            }

            var orderLogs = GetService<IoperationLog>().GetCustomerOrderLog(hid, type, id, keywords);
            var result = Json(orderLogs.ToDataSourceResult(request));
            return result;
        }
    }
}