using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.MarketingManage.Controllers
{
    [AuthPage("61091001")]
    public class RoomOwnerCalcParaDefineController : BaseEditIncellController<RoomOwnerCalcParaDefine, IRoomOwnerCalcParaDefineService>
    {
        // GET: MarketingManage/RoomOwnerCalcParaDefine
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_RoomOwnerCalcParaDefine", "");
            Collection<SelectListItem> list = new Collection<SelectListItem>() {
                   new SelectListItem() { Value = "true", Text = "是" },
                   new SelectListItem() { Value = "false", Text = "否"}
            };
            var datas = list.Select(c => new { Id = c.Value, Name = c.Text }).ToList();
            ViewData["isHidden_Data"] = new SelectList(datas, "Id", "Name");
            List<V_dataType> dt = GetService<IRoomOwnerCalcParaDefineService>().getDataType();
            datas = dt.Select(c => new { Id = c.Code, Name = c.Name }).ToList();
            ViewData["dataType_Data"] = new SelectList(datas, "Id", "Name");
            return View();
        }
        #endregion


        #region 修改      
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<RoomOwnerCalcParaDefine> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<RoomOwnerCalcParaDefine> originVersions)
        {
            List<RoomOwnerCalcParaDefine> r = new List<RoomOwnerCalcParaDefine>();
            foreach (var up in updatedVersions)
            {
                r.Add(up);
            }
            var rp = GetService<IRoomOwnerCalcTypeService>().List(CurrentInfo.HotelId);

            var roomOwner = GetService<IRoomOwnerCalcParaDefineService>();
            string confirm = roomOwner.updateRoomOwnerCalcParaDefine(r, CurrentInfo.HotelId);
            if (confirm != "")
            {
                ModelState.AddModelError("Name", confirm);
                return Json(updatedVersions.ToDataSourceResult(request, ModelState));
            }
            return Json(updatedVersions.ToDataSourceResult(request));
        }
        #endregion

        [HttpGet]
        [AuthButton(AuthFlag.None)]
        public ActionResult getDataTypeName(string values)
        {
            var m = GetService<IRoomOwnerCalcParaDefineService>();
            V_dataType c = m.getDataTypeName(values);
            if (c != null)
            {
                return Json(JsonResultData.Successed(c.Name), JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(JsonResultData.Failure(""), JsonRequestBehavior.AllowGet);
            }
        }
    }
}