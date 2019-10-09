using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.RoleManage;
using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Kendo.Mvc.Extensions;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 酒店日历
    /// </summary>
    [AuthPage("99045")]
    public class HotelDayManageController : BaseEditIncellController<HotelDay, IHotelDayService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_hotelDay", "");
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        [KendoGridDatasourceException]
        public ActionResult Add([DataSourceRequest] DataSourceRequest request, HotelDay entity)
        {
            List<HotelDay> addVersions = new List<HotelDay>();
            addVersions.Add(entity);
            return _Add(request, addVersions, w => { w.Id = Guid.NewGuid(); w.Hid = CurrentInfo.HotelId; }, OpLogType.酒店日历增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, HotelDay updated, [Bind(Prefix = "originModels")]IEnumerable<HotelDay> originVersions)
        {
            List<HotelDay> updatedVersions = new List<HotelDay>();
            updatedVersions.Add(updated);
            return _Update(request, updatedVersions, originVersions, (list, u) => list.SingleOrDefault(w => w.Id == u.Id), OpLogType.酒店日历修改);
        }
        #endregion

        #region 删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult Destroy([DataSourceRequest] DataSourceRequest request, HotelDay entity)
        {
            return _BatchDelete(entity.Id.ToString(), GetService<IHotelDayService>(), OpLogType.酒店日历删除);
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IHotelDayService>(), OpLogType.酒店日历删除);
        }
        #endregion
        #region 下拉绑定
        [AuthButton(AuthFlag.Query)]
        public JsonResult GetHotelDaySelectList()
        {
            var _hotelDayService = GetService<IHotelDayService>();
            return Json(_hotelDayService.List(CurrentInfo.HotelId), JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}