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
using Gemstar.BSPMS.Hotel.Services.PermanentRoomManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PermanentRoom.Controllers
{
    /// <summary>
    /// 长包房物品设置表（设置长包房物品列表）
    /// </summary>
    [AuthPage("21020")]
    public class PermanentRoomGoodsController : BaseEditIncellController<PermanentRoomGoodsSet, IPermanentRoomGoodsService>
    {
        #region 查询
        [AuthButton(AuthFlag.Update)]
        public ActionResult Index(string regid)
        {
            string hid = CurrentInfo.HotelId;
            if (string.IsNullOrWhiteSpace(regid) || !regid.StartsWith(hid))
            {
                return Content("账号ID错误");
            }

            var permanentRoomGoodsService = GetService<IPermanentRoomGoodsService>();
            if (!permanentRoomGoodsService.ExistsRegId(hid, regid))
            {
                return Content("账号ID错误");
            }
            var permanentRoomSetId = permanentRoomGoodsService.GetPermanentRoomId(hid, regid);
            if (string.IsNullOrWhiteSpace(permanentRoomSetId))
            {
                return Content("账号ID错误");
            }

            ViewBag.PermanentRoomSetId = permanentRoomSetId;
            SetCommonQueryValues("up_list_PermanentRoomGoodsSetting", "@t00permanentRoomId=" + permanentRoomSetId);

            //物品列表
            var items = GetService<ICodeListService>().List(hid, "19");
            var datas = items.Select(c => new { Id = c.Id, Name = c.Name }).ToList();
            ViewData["Itemid_Data"] = new SelectList(datas, "Id", "Name");

            //借用状态
            ViewData["BorrowType_Data"] = new SelectList(new SelectListItem[] {
                new SelectListItem { Text = " ", Value = "" },
                new SelectListItem { Text = "借用", Value = "1" },
                new SelectListItem { Text = "归还", Value = "2" },
            }, "Value", "Text");

            ViewData["Itemid_Height"] = 150;
            ViewData["BorrowType_Height"] = 150;

            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Add([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PermanentRoomGoodsSet> addVersions, bool isTemplete = false)
        {
            string hid = CurrentInfo.HotelId;

            if (addVersions.Select(c => c.PermanentRoomSetId).Distinct().Count() != 1)
            {
                ModelState.AddModelError("PermanentRoomSetId", "订单ID错误，订单ID必须相同");
                return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }

            string permanentRoomSetId = addVersions.Select(c => c.PermanentRoomSetId).FirstOrDefault();
            if (!GetService<IPermanentRoomGoodsService>().ExistsPermanentRoomId(hid, permanentRoomSetId))
            {
                ModelState.AddModelError("PermanentRoomSetId", "订单ID错误，找不到此订单ID");
                return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }

            var settingService = GetService<IPermanentRoomGoodsService>();
            List<PermanentRoomGoodsSet> originList = settingService.ToList(hid, permanentRoomSetId);
            if (isTemplete && originList != null && originList.Count > 0)
            {
                foreach (var item in originList)
                {
                    settingService.Delete(item);
                }
                originList.Clear();
            }

            foreach (var model in addVersions)
            {
                if (string.IsNullOrWhiteSpace(model.PermanentRoomSetId))
                {
                    ModelState.AddModelError("PermanentRoomSetId", "订单ID不能为空");
                    return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(model.Itemid))
                {
                    ModelState.AddModelError("Itemid", "物品不能为空");
                    return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }
                if (originList.Select(c => c.Itemid).Contains(model.Itemid))
                {
                    ModelState.AddModelError("Itemid", "物品不能重复，请修改！");
                    return Json(addVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }
            }
            return _Add(request, addVersions, w => { w.Hid = hid; }, OpLogType.账单设置添加);
        }
        #endregion

        #region 修改      
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<PermanentRoomGoodsSet> updatedVersions, [Bind(Prefix = "originModels")]IEnumerable<PermanentRoomGoodsSet> originVersions)
        {
            List<PermanentRoomGoodsSet> list = new List<PermanentRoomGoodsSet>();

            foreach (var model in originVersions)
            {
                list.Add(new PermanentRoomGoodsSet { Id = model.Id, Itemid = model.Itemid });
            }

            foreach (var model in updatedVersions)
            {
                if (string.IsNullOrWhiteSpace(model.PermanentRoomSetId))
                {
                    ModelState.AddModelError("PermanentRoomSetId", "订单ID不能为空");
                    return Json(updatedVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(model.Itemid))
                {
                    ModelState.AddModelError("Itemid", "物品不能为空");
                    return Json(updatedVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
                }

                var entity = list.Where(c => c.Id == model.Id).FirstOrDefault();
                if (entity != null)
                {
                    entity.Itemid = model.Itemid;
                }
                else
                {
                    list.Add(new PermanentRoomGoodsSet { Id = model.Id,  Itemid = model.Itemid });
                }
            }

            if (list.Select(c => c.Itemid).Distinct().Count() != list.Select(c => c.Itemid).Count())
            {
                ModelState.AddModelError("Itemid", "物品不能重复，请修改！");
                return Json(updatedVersions.ToDataSourceResult(request, ModelState), JsonRequestBehavior.AllowGet);
            }

            return _Update(request, updatedVersions, originVersions, (originModels, update) => {
                var origin = originModels.SingleOrDefault(w => w.Id == update.Id);
                if (origin.Hid != update.Hid && update.Hid != CurrentInfo.HotelId)
                {
                    throw new Exception("酒店ID错误！");
                }
                if (origin.PermanentRoomSetId != update.PermanentRoomSetId)
                {
                    throw new Exception("订单ID错误！");
                }
                return origin;
            }, OpLogType.账单设置修改);
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Update)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPermanentRoomGoodsService>(), OpLogType.账单设置删除);
        }
        #endregion

        #region 模板
        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult GetTempleteList()
        {
            var list = GetService<IPermanentRoomGoodsService>().ToTempleteList(CurrentInfo.HotelId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取模板详细列表
        /// </summary>
        /// <param name="id">模板ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult ToTempleteDetailList(Guid id)
        {
            var list = GetService<IPermanentRoomGoodsService>().ToTempleteDetailList(CurrentInfo.HotelId, id);
            return Json(JsonResultData.Successed(list), JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 添加模板
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult AddTemplete(string name, List<PermanentRoomGoodsSet> addVersions)
        {
            if (string.IsNullOrWhiteSpace(name) || addVersions == null || addVersions.Count <= 0)
            {
                return Json(JsonResultData.Failure("参数不正确！"), JsonRequestBehavior.DenyGet);
            }
            foreach (var model in addVersions)
            {
                if (string.IsNullOrWhiteSpace(model.Itemid))
                {
                    return Json(JsonResultData.Failure("物品不能为空！"), JsonRequestBehavior.DenyGet);
                }
            }
            var result = GetService<IPermanentRoomGoodsService>().AddTemplete(CurrentInfo.HotelId, name, addVersions);
            return Json(result, JsonRequestBehavior.DenyGet);
        }
        /// <summary>
        /// 删除模板
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult DelTemplete(Guid id)
        {
            GetService<IPermanentRoomGoodsService>().DelTemplete(CurrentInfo.HotelId, id);
            return Json(JsonResultData.Successed(), JsonRequestBehavior.DenyGet);
        }
        #endregion
    }
}