using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosMScroll;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos扫码点餐滚动菜式
    /// </summary>
    [AuthPage(ProductType.Pos, "p99115003")]
    public class BasicDataPosMScrollController : BaseEditInWindowController<PosMScroll, IPosMScrollService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            return View();
        }

        #region 增加

        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            ViewBag.Domain = "http://res.gshis.com/";
            return _Add(new PosMScrollAddViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosMScrollAddViewModel addViewModel)
        {
            var modelService = GetService<IPosMScrollService>();

            var itemService = GetService<IPosItemService>();
            var item = itemService.GetEntity(CurrentInfo.HotelId, addViewModel.Itemid);
            addViewModel.ItemCode = item.Code;
            addViewModel.ItemName = item.Cname;

            var unitService = GetService<IPosUnitService>();
            var unit = unitService.GetEntity(CurrentInfo.HotelId, addViewModel.Unitid);
            addViewModel.UnitName = unit.Cname;

            ActionResult result = _Add(addViewModel, new PosMScroll { Id = Guid.NewGuid(), Hid = CurrentInfo.HotelId, Creator = CurrentInfo.UserName, Createdate = DateTime.Now }, OpLogType.Pos扫码点餐滚动菜式添加);
            return result;
        }

        #endregion 增加

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            ViewBag.Domain = "http://res.gshis.com/";
            return _Edit(id, new PosMScrollEditViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosMScrollEditViewModel model)
        {
            var modelService = GetService<IPosMScrollService>();

            var itemService = GetService<IPosItemService>();
            var item = itemService.GetEntity(CurrentInfo.HotelId, model.Itemid);
            model.ItemCode = item.Code;
            model.ItemName = item.Cname;

            var unitService = GetService<IPosUnitService>();
            var unit = unitService.GetEntity(CurrentInfo.HotelId, model.Unitid);
            model.UnitName = unit.Cname;

            ActionResult result = _Edit(model, new PosMScroll(), OpLogType.Pos扫码点餐滚动菜式修改);
            return result;
        }

        #endregion 修改

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosMScrollService>(), OpLogType.Pos扫码点餐滚动菜式删除);
        }

        #endregion 批量删除

        /// <summary>
        ///
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListMScrollByHid([DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosMScrollService>();
            var list = service.GetPosMScrollList(CurrentInfo.HotelId);
            return Json(list.ToDataSourceResult(request));
        }

        /// <summary>
        /// 获取指定显示方式下的消费项目
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosItemByShowSets()
        {
            var service = GetService<IPosItemService>();
            var datas = service.GetPosItemByShowSet(CurrentInfo.HotelId, CurrentInfo.ModuleCode, PosShowSet.Mobile.ToString(), PosItemDcFlag.D.ToString());
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
    }
}