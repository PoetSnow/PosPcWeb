using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemPayWay;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos付款方式
    /// </summary>
    [AuthPage(ProductType.Pos, "p99035003")]
    public class BasicDataPosItemPayWayController : BaseEditInWindowController<PosItem, IPosItemService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_ItemPayWay", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosItemPayWayAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosItemPayWayAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosItemService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname, PosItemDcFlag.C.ToString());
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            if(!string.IsNullOrEmpty(Request["Refeid"]))
            {
                addViewModel.Refeid = Request["Refeid"].ToString();
            }
            ActionResult result = _Add(addViewModel, new PosItem { Id = id, Hid = CurrentInfo.HotelId, DcFlag = PosItemDcFlag.C.ToString(), OperName = CurrentInfo.UserName, ModifiedDate = DateTime.Now }, OpLogType.Pos付款方式增加);

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            var modelService = GetService<IPosItemService>();
            var data = modelService.Get(id);
            ViewBag.Rate = data.Rate == null ? 1 : data.Rate;
            return _Edit(id, new PosItemPayWayEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosItemPayWayEditViewModel model)
        {
            var modelService = GetService<IPosItemService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, PosItemDcFlag.C.ToString(), model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            if (!string.IsNullOrEmpty(Request["Refeid"]))
            {
                model.Refeid = Request["Refeid"].ToString();
            }
            model.OperName = CurrentInfo.UserName;
            model.ModifiedDate = DateTime.Now;
            ActionResult result = _Edit(model, new PosItem(), OpLogType.Pos付款方式修改);
            return result;
        }
        #endregion
        
        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosItemService>(), OpLogType.Pos付款方式删除);
        }
        #endregion

        #region 下拉数据绑定
        /// <summary>
        /// 获取指定酒店下的付款方式
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosItem()
        {
            var service = GetService<IPosItemService>();
            var datas = service.GetPosItem(CurrentInfo.HotelId, PosItemDcFlag.C.ToString());
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取指定模块下的付款方式
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosItemByModules()
        {
            var service = GetService<IPosItemService>();
            var datas = service.GetPosItemByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode, PosItemDcFlag.C.ToString());
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 启用
        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {

            var service = GetService<IPosItemService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.启用,OpLogType.Pos付款方式启用禁用));
            return reval;
        }
        #endregion

        #region 禁用
        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {

            var service = GetService<IPosItemService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用, OpLogType.Pos付款方式启用禁用));

            return reval;

        }
        #endregion
    }
}
