using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;

using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.ResManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Controllers
{
    /// <summary>
    /// 账单设置表（设置主单内所有子单账务的账单，例如：A账单、B账单等）
    /// </summary>
    [AuthPage("20020")]
    public class ResBillSettingController : BaseEditIncellController<ResBillSetting, IResBillSettingService>
    {
        #region 查询
        [AuthButton(AuthFlag.AdjustFolio)]
        public ActionResult Index(string resid)
        {
            string hid = CurrentInfo.HotelId;

            if (string.IsNullOrWhiteSpace(resid) || !resid.StartsWith(hid))
            {
                return Content("订单ID错误");
            }
            if(!GetService<IResBillSettingService>().ExistsResId(hid, resid))
            {
                return Content("订单ID错误");
            }

            ViewBag.Resid = resid;
            SetCommonQueryValues("up_list_ResBillSetting", "@t00resid=" + resid);

            
            ViewData["BillCode_Data"] = new SelectList(new SelectListItem[] {
                new SelectListItem { Text = "B", Value = "B" },
                new SelectListItem { Text = "C", Value = "C" },
                new SelectListItem { Text = "D", Value = "D" },
                new SelectListItem { Text = "E", Value = "E" },
                new SelectListItem { Text = "F", Value = "F" },
                new SelectListItem { Text = "G", Value = "G" },
                new SelectListItem { Text = "H", Value = "H" },
                new SelectListItem { Text = "I", Value = "I" },
                new SelectListItem { Text = "G", Value = "G" },
                new SelectListItem { Text = "K", Value = "K" },
                new SelectListItem { Text = "L", Value = "L" },
                new SelectListItem { Text = "M", Value = "M" },
                new SelectListItem { Text = "N", Value = "N" },
                new SelectListItem { Text = "O", Value = "O" },
                new SelectListItem { Text = "P", Value = "P" },
                new SelectListItem { Text = "Q", Value = "Q" },
                new SelectListItem { Text = "R", Value = "R" },
                new SelectListItem { Text = "S", Value = "S" },
                new SelectListItem { Text = "T", Value = "T" },
                new SelectListItem { Text = "U", Value = "U" },
                new SelectListItem { Text = "V", Value = "V" },
                new SelectListItem { Text = "W", Value = "W" },
                new SelectListItem { Text = "X", Value = "X" },
                new SelectListItem { Text = "Y", Value = "Y" },
                new SelectListItem { Text = "Z", Value = "Z" }
            }, "Value", "Text");

            var items = GetService<IItemService>().Query(hid, "D", "");
            var datas = items.Select(c => new { Id = c.Id, Name = c.Code + "-" + c.Name }).ToList();
            ViewData["DebitTypeId_Data"] = new SelectList(datas, "Id", "Name");

            ViewData["BillCode_Height"] = 150;
            ViewData["DebitTypeId_Height"] = 150;

            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.AdjustFolio)]
        [KendoGridDatasourceException]
        public ActionResult Add([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ResBillSetting> addVersions, bool isTemplete = false)
        {
            string hid = CurrentInfo.HotelId;

            if (addVersions.Select(c => c.Resid).Distinct().Count() != 1)
            {
                ModelState.AddModelError("Resid", "订单ID错误，订单ID必须相同");
                return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }

            string resid = addVersions.Select(c => c.Resid).FirstOrDefault();
            if (!GetService<IResBillSettingService>().ExistsResId(hid, resid))
            {
                ModelState.AddModelError("Resid", "订单ID错误，找不到此订单ID");
                return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }

            var resBillSettingService  = GetService< IResBillSettingService>();
            List<ResBillSetting> originList = resBillSettingService.ToListAll(hid, resid);
            if (isTemplete && originList != null && originList.Count > 0)
            {
                foreach(var item in originList)
                {
                    resBillSettingService.Delete(item);
                }
                originList.Clear();
            }

            foreach (var model in addVersions)
            {
                if (string.IsNullOrWhiteSpace(model.Resid))
                {
                    ModelState.AddModelError("Resid", "订单ID不能为空");
                    return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(model.BillCode))
                {
                    ModelState.AddModelError("BillCode", "账单代码不能为空");
                    return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }
                if(originList.Select(c => c.BillCode).Contains(model.BillCode))
                {
                    ModelState.AddModelError("BillCode", "账单代码" + model.BillCode + "已重复，请修改！");
                    return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrWhiteSpace(model.BillName))
                {
                    ModelState.AddModelError("BillName", "账单名称不能为空");
                    return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }
                if (originList.Select(c => c.BillName).Contains(model.BillName))
                {
                    ModelState.AddModelError("BillCode", "账单名称" + model.BillName + "已重复，请修改！");
                    return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrWhiteSpace(model.DebitTypeId))
                {
                    ModelState.AddModelError("DebitTypeId", "消费类型不能为空");
                    return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }
            }
            return _Add(request, addVersions, w => { w.Hid = hid; }, OpLogType.账单设置添加);
        }
        #endregion

        #region 修改      
        [AuthButton(AuthFlag.AdjustFolio)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ResBillSetting> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<ResBillSetting> originVersions)
        {
            List<ResBillSettingInfo> list = new List<ResBillSettingInfo>();

            foreach (var model in originVersions)
            {
                list.Add(new ResBillSettingInfo { Id = model.Id,  BillCode = model.BillCode,  BillName = model.BillName });
            }

            foreach (var model in updatedVersions)
            {
                if (string.IsNullOrWhiteSpace(model.Resid))
                {
                    ModelState.AddModelError("Resid", "订单ID不能为空");
                    return Json(updatedVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(model.BillCode))
                {
                    ModelState.AddModelError("BillCode", "账单代码不能为空");
                    return Json(updatedVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(model.BillName))
                {
                    ModelState.AddModelError("BillName", "账单名称不能为空");
                    return Json(updatedVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(model.DebitTypeId))
                {
                    ModelState.AddModelError("DebitTypeId", "消费类型不能为空");
                    return Json(updatedVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }

                var entity = list.Where(c => c.Id == model.Id).FirstOrDefault();
                if(entity != null)
                {
                    entity.BillCode = model.BillCode;
                    entity.BillName = model.BillName;
                }
                else
                {
                    list.Add(new ResBillSettingInfo { Id = model.Id, BillCode = model.BillCode, BillName = model.BillName });
                }
            }

            if(list.Select(c => c.BillCode).Distinct().Count() != list.Select(c => c.BillCode).Count())
            {
                ModelState.AddModelError("BillCode", "账单代码重复，请修改！");
                return Json(updatedVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }

            if (list.Select(c => c.BillName).Distinct().Count() != list.Select(c => c.BillName).Count())
            {
                ModelState.AddModelError("BillName", "账单名称重复，请修改！");
                return Json(updatedVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }

            return _Update(request, updatedVersions, originVersions, (originModels, update) => {
                var origin = originModels.SingleOrDefault(w => w.Id == update.Id);
                if(origin.Hid != update.Hid && update.Hid != CurrentInfo.HotelId)
                {
                    throw new Exception("酒店ID错误！");
                }
                if (origin.Resid != update.Resid)
                {
                    throw new Exception("订单ID错误！");
                }
                return origin;
                }, OpLogType.账单设置修改);
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.AdjustFolio)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IResBillSettingService>(), OpLogType.账单设置删除);
        }
        #endregion

        #region 新的设置应用到历史账务中
        [AuthButton(AuthFlag.AdjustFolio)]
        public ActionResult ResBillSettingToFolio(string resid)
        {
            if (string.IsNullOrWhiteSpace(resid))
            {
                return Json(JsonResultData.Failure("参数错误！"), JsonRequestBehavior.DenyGet);
            }
            var result = GetService<IResBillSettingService>().ResBillSettingToFolio(CurrentInfo.HotelId, resid, CurrentInfo.UserName);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 模板
        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.AdjustFolio)]
        public ActionResult GetTempleteList()
        {
            var list = GetService<IResBillSettingService>().ToTempleteList(CurrentInfo.HotelId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取模板详细列表
        /// </summary>
        /// <param name="id">模板ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.AdjustFolio)]
        public ActionResult ToTempleteDetailList(Guid id)
        {
            var list = GetService<IResBillSettingService>().ToTempleteDetailList(CurrentInfo.HotelId, id);
            return Json(JsonResultData.Successed(list), JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 添加模板
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.AdjustFolio)]
        public ActionResult AddTemplete(string name, List<ResBillSetting> addVersions)
        {
            if(string.IsNullOrWhiteSpace(name) || addVersions == null || addVersions.Count <= 0)
            {
                return Json(JsonResultData.Failure("参数不正确！"), JsonRequestBehavior.DenyGet);
            }
            foreach (var model in addVersions)
            {
                if (string.IsNullOrWhiteSpace(model.BillCode))
                {
                    return Json(JsonResultData.Failure("账单代码不能为空"), JsonRequestBehavior.DenyGet);
                }
                if (addVersions.Where(c => c.BillCode == model.BillCode).Count() > 1)
                {
                    return Json(JsonResultData.Failure("账单代码" + model.BillCode + "已重复，请修改！"), JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrWhiteSpace(model.BillName))
                {
                    return Json(JsonResultData.Failure("账单名称不能为空"), JsonRequestBehavior.DenyGet);
                }
                if (addVersions.Where(c => c.BillName == model.BillName).Count() > 1)
                {
                    return Json(JsonResultData.Failure("账单名称" + model.BillName + "已重复，请修改！"), JsonRequestBehavior.DenyGet);
                }
                if (string.IsNullOrWhiteSpace(model.DebitTypeId))
                {
                    return Json(JsonResultData.Failure("消费类型不能为空"), JsonRequestBehavior.DenyGet);
                }
            }
            var result = GetService<IResBillSettingService>().AddTemplete(CurrentInfo.HotelId, name, addVersions);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 删除模板
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.AdjustFolio)]
        public ActionResult DelTemplete(Guid id)
        {
            GetService<IResBillSettingService>().DelTemplete(CurrentInfo.HotelId, id);
            return Json(JsonResultData.Successed(), JsonRequestBehavior.DenyGet);
        }
        #endregion
    }
}