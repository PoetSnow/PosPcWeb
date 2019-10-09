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
    /// 业务员资料 
    /// </summary>
    [AuthPage("99035")]
    [AuthPage(ProductType.Member, "m61060")]
    public class SalesManageController : BaseEditIncellController<Sales, ISalesService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_sales", "");
            if (CurrentInfo.IsGroupInGroup)
            {
                var hotellist = GetService<IPmsHotelService>().GetHotelsInGroup(CurrentInfo.HotelId).Where(w => w.Hid != CurrentInfo.HotelId);
                var list = hotellist.Select(s => new SelectListItem { Value = s.Hid, Text = string.IsNullOrEmpty(s.Hotelshortname) ? s.Name : s.Hotelshortname }).ToList();
                ViewData["Belonghotel_Data"] = new SelectList(list, "Value", "Text");
            }
            ViewBag.isgroupingroup = CurrentInfo.IsGroupInGroup;
            ViewBag.isHotelInGroup = CurrentInfo.IsHotelInGroup;
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        [KendoGridDatasourceException]
        public ActionResult Add([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<Sales> addVersions)
        {
            string hid = CurrentInfo.HotelId;
            var salesService = GetService<ISalesService>();
            var salesNames = new List<string>();
            foreach (var model in addVersions)
            {
                ModelState.Clear();
                model.Id = Guid.NewGuid();
                if (model.Name == null)
                {
                    ModelState.AddModelError("Name", "业务员姓名不能为空");
                    return Json(addVersions.ToDataSourceResult(request, ModelState));
                }
                if (model.Mobile != null && model.Mobile.Length != 11)
                {
                    ModelState.AddModelError("Mobile", "手机号格式错误");
                    return Json(addVersions.ToDataSourceResult(request, ModelState));
                }
                if (model.Email != null && model.Email.IndexOf("@") <= -1)
                {
                    ModelState.AddModelError("Email", "邮箱格式错误");
                    return Json(addVersions.ToDataSourceResult(request, ModelState));
                }
                //验证姓名
                if (salesNames.Contains(model.Name))
                {
                    ModelState.AddModelError("Name", string.Format("业务员姓名[{0}]不能重复", model.Name));
                    return Json(addVersions.ToDataSourceResult(request, ModelState));
                }
                salesNames.Add(model.Name);
                if (salesService.Exists(hid, model.Name))
                {
                    ModelState.AddModelError("Name", string.Format("业务员姓名[{0}]已存在", model.Name));
                    return Json(addVersions.ToDataSourceResult(request, ModelState));
                }
            }
            if (CurrentInfo.IsGroupInGroup)
            {
                foreach (var model in addVersions)
                {

                    salesService.GroupControlAdd(model, CurrentInfo.HotelId);
                }
                return Json("");
            }
            else
            {
                return _Add(request, addVersions, w => { w.Id = Guid.NewGuid(); w.Hid = CurrentInfo.HotelId; w.Status = EntityStatus.启用; w.Grpid = CurrentInfo.GroupId; }, OpLogType.业务员增加);
            }

        }
        #endregion

        #region 修改      
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<Sales> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<Sales> originVersions)
        {
            string hid = CurrentInfo.HotelId;
            var salesService = GetService<ISalesService>();
            var salesNames = new List<string>();
            foreach (var model in updatedVersions)
            {
                ModelState.Clear();
                if (model.Name == null)
                {
                    ModelState.AddModelError("Name", "业务员姓名不能为空");
                    return Json(updatedVersions.ToDataSourceResult(request, ModelState));
                }
                if (model.Mobile != null && model.Mobile.Length != 11)
                {
                    ModelState.AddModelError("Mobile", "手机号格式错误");
                    return Json(updatedVersions.ToDataSourceResult(request, ModelState));
                }
                if (model.Email != null && model.Email.IndexOf("@") <= -1)
                {
                    ModelState.AddModelError("Email", "邮箱格式错误");
                    return Json(updatedVersions.ToDataSourceResult(request, ModelState));
                }
                //验证姓名
                if (salesNames.Contains(model.Name))
                {
                    ModelState.AddModelError("Name", string.Format("业务员姓名[{0}]不能重复", model.Name));
                    return Json(updatedVersions.ToDataSourceResult(request, ModelState));
                }
                salesNames.Add(model.Name);
            }


            if (CurrentInfo.IsGroupInGroup)
            {
                foreach (var model in updatedVersions)
                {
                    foreach (var orimodel in originVersions)
                    {
                        if (model.Id == orimodel.Id)
                        {
                            salesService.GroupControlEdit(model, orimodel, CurrentInfo.HotelId);
                        }
                    }
                }
                return Json("");
            }
            else
            {

                return _Update(request, updatedVersions, originVersions, (list, u) =>
                {
                    var entity = list.SingleOrDefault(w => w.Id == u.Id);
                    if (entity == null || entity.Hid != hid || entity.Hid != u.Hid)
                    {
                        throw new Exception("错误信息，请关闭后重试");
                    }
                    if (salesService.Exists(hid, u.Name, entity.Id))
                    {
                        throw new Exception(string.Format("业务员姓名[{0}]已存在", u.Name));
                    }
                    return entity;
                }, OpLogType.业务员修改);
            }
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {

            if (CurrentInfo.IsGroupInGroup)
            {
                id = GetService<ISalesService>().getGrouphotelid(id);
            }
            return _BatchDelete(id, GetService<ISalesService>(), OpLogType.业务员删除);
        }
        #endregion
        #region 启用禁用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            if (CurrentInfo.IsGroupInGroup)
            {
                id = GetService<ISalesService>().getGrouphotelid(id);
            }
            var _salesService = GetService<ISalesService>();
            return Json(_salesService.BatchUpdateStatus(id, EntityStatus.启用));
        }
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            if (CurrentInfo.IsGroupInGroup)
            {
                id = GetService<ISalesService>().getGrouphotelid(id);
            }
            var _salesService = GetService<ISalesService>();
            return Json(_salesService.BatchUpdateStatus(id, EntityStatus.禁用));
        }

        #endregion

        [HttpGet]
        [AuthButton(AuthFlag.None)]
        public ActionResult getBelonghotel(string values)
        {
            string[] val = values.Split(',');
            string retval = "";
            for (int i = 0; i < val.Length; i++)
            {
                PmsHotel hotel = GetService<IPmsHotelService>().Get(val[i]);
                retval += string.IsNullOrEmpty(hotel.Hotelshortname) ? hotel.Name : hotel.Hotelshortname + ",";
            }
            return Json(JsonResultData.Successed(retval.Trim(',')), JsonRequestBehavior.AllowGet);
        }
        #region 下拉绑定
        [AuthButton(AuthFlag.None)]
        public JsonResult GetSalesSelectList(string nameOrMobile)
        {
            if (string.IsNullOrWhiteSpace(nameOrMobile))
            {
                return Json(new System.Collections.Generic.List<Sales>(), JsonRequestBehavior.AllowGet);
            }
            var _salesService = GetService<ISalesService>();
            return Json(_salesService.List((CurrentInfo.IsGroup ? CurrentInfo.GroupId : CurrentInfo.HotelId), nameOrMobile), JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}