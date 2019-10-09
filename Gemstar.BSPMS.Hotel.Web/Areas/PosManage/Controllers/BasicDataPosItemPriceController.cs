using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemPrice;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos单位价格
    /// </summary>
    [AuthPage(ProductType.Pos, "p99020007")]
    public class BasicDataPosItemPriceController : BaseEditInWindowController<PosItemPrice, IPosItemPriceService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_ItemPrice", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosItemPriceAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosItemPriceAddViewModel addViewModel)
        {
            var id = Guid.NewGuid();
            var modelService = GetService<IPosItemPriceService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Itemid, addViewModel.Unitid);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            var unitService = GetService<IPosUnitService>();
            var unit = unitService.Get(addViewModel.Unitid);
            //如果是缺省单位，把其余的单位缺省修改成否
            if (addViewModel.IsDefault == true)
            {
                var itemPriceList = modelService.GetPosItemPriceForCopy(CurrentInfo.HotelId, addViewModel.Itemid);
                foreach (var itemPrice in itemPriceList)
                {
                    var newPosItemPrice = new PosItemPrice();
                    AutoSetValueHelper.SetValues(itemPrice, newPosItemPrice);
                    newPosItemPrice.IsDefault = false;
                    newPosItemPrice.Modified = DateTime.Now;
                    modelService.Update(newPosItemPrice, itemPrice);
                    modelService.AddDataChangeLog(OpLogType.Pos消费项目对应价格修改);
                    modelService.Commit();
                }
            }
            ActionResult result = _Add(addViewModel, new PosItemPrice { Id = id, Hid = CurrentInfo.HotelId, UnitCode = unit.Code, Unit = unit.Cname, Modified = DateTime.Now }, OpLogType.Pos消费项目对应价格增加);


            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            return _Edit(id, new PosItemPriceEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosItemPriceEditViewModel model)
        {
            var modelService = GetService<IPosItemPriceService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Itemid, model.Unitid);
            if (isexsit)
            {
                var positemPrice = modelService.GetPosItemPriceByUnitid(CurrentInfo.HotelId, model.Itemid, model.Unitid);
                if (positemPrice.Id != model.Id)
                {
                    return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！"));
                }

            }
            //如果是缺省单位，把其余的单位缺省修改成否
            if (model.IsDefault == true)
            {
                var itemPriceList = modelService.GetPosItemPriceForCopy(CurrentInfo.HotelId, model.Itemid).Where(m => m.Id != model.Id).ToList();
                foreach (var itemPrice in itemPriceList)
                {
                    var newPosItemPrice = new PosItemPrice();
                    AutoSetValueHelper.SetValues(itemPrice, newPosItemPrice);
                    newPosItemPrice.IsDefault = false;
                    newPosItemPrice.Modified = DateTime.Now;
                    modelService.Update(newPosItemPrice, itemPrice);
                    modelService.AddDataChangeLog(OpLogType.Pos消费项目对应价格修改);
                    modelService.Commit();
                }
            }

            var oldEntity = modelService.Get(model.Id);
            PosItemPrice newEntity = new PosItemPrice();
            AutoSetValueHelper.SetValues(oldEntity, newEntity);

            var unitService = GetService<IPosUnitService>();
            var unit = unitService.Get(model.Unitid);
            model.Unit = unit.Cname;
            model.UnitCode = unit.Code;
            model.Modified = DateTime.Now;

            ActionResult result = _Edit(model, oldEntity, OpLogType.Pos消费项目对应价格修改);
            return result;
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosItemPriceService>(), OpLogType.Pos消费项目对应价格删除);
        }
        #endregion

        #region 下拉数据绑定
        /// <summary>
        /// 获取指定项目下的单位定义
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForItemPriceByItemId(string itemid)
        {
            var service = GetService<IPosItemPriceService>();
            var datas = service.GetPosItemPriceByItemId(CurrentInfo.HotelId, itemid);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Unitid, Text = w.Unit }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取指定项目和单位下的单位价格
        /// </summary>
        /// <param name="itemid">项目id</param>
        /// <param name="unitid">单位价格id</param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.None)]
        public ActionResult GetItemPriceByUnititId(string itemid, string unitid)
        {
            if (!string.IsNullOrWhiteSpace(itemid) && !string.IsNullOrWhiteSpace(unitid))
            {
                try
                {
                    var unitService = GetService<IPosItemPriceService>();
                    var unit = unitService.GetPosItemPriceByUnitid(CurrentInfo.HotelId, itemid, unitid);
                    return Json(JsonResultData.Successed(unit));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            return Json(JsonResultData.Successed(new PosItemPrice { Price = 0 }));
        }
        #endregion
    }
}
