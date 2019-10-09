using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemClass;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos消费项目大类
    /// </summary>
    [AuthPage(ProductType.Pos, "p99020001")]
    public class BasicDataPosItemClassController : BaseEditInWindowController<PosItemClass, IPosItemClassService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_ItemClass", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosItemClassAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosItemClassAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosItemClassService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            if (!string.IsNullOrEmpty(Request["Refeid"]))
            {
                addViewModel.Refeid = Request["Refeid"].ToString();
            }
            addViewModel.Code = addViewModel.Code != null ? addViewModel.Code.Trim() : "";
            ActionResult result = _Add(addViewModel, new PosItemClass { Id = id, Hid = CurrentInfo.HotelId, ModifiedDate = DateTime.Now }, OpLogType.Pos消费项目大类增加);

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            ViewBag.id = id;
            return _Edit(id, new PosItemClassEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosItemClassEditViewModel model)
        {
            var modelService = GetService<IPosItemClassService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            if (!string.IsNullOrEmpty(Request["Refeid"]))
            {
                model.Refeid = Request["Refeid"].ToString();
            }
            model.ModifiedDate = DateTime.Now;
            ActionResult result = _Edit(model, new PosItemClass(), OpLogType.Pos消费项目大类修改);
            return result;
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosItemClassService>(), OpLogType.Pos消费项目大类删除);
        }
        #endregion

        #region 下拉数据绑定
        /// <summary>
        /// 获取指定酒店下的消费项目大类
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosItemClass()
        {
            var service = GetService<IPosItemClassService>();
            var datas = service.GetPosItemClass(CurrentInfo.HotelId);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取指定模块下的消费项目大类
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosItemClassByModules()
        {
            var service = GetService<IPosItemClassService>();
            var datas = service.GetPosItemClassByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取指定模块下的消费项目大类和消费项目分类
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosItemAndSubClass()
        {
            var service = GetService<IPosItemClassService>();
            var datas = service.GetPosItemClassAndSubClass(CurrentInfo.HotelId);
            return Json(datas, JsonRequestBehavior.AllowGet);
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

            var service = GetService<IPosItemClassService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.启用, OpLogType.Pos消费项目大类启用禁用));
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

            var service = GetService<IPosItemClassService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用, OpLogType.Pos消费项目大类启用禁用));

            return reval;

        }
        #endregion


        /// <summary>
        /// 项目大类对应作法
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListPosItemActionByItemId(string id, [DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosItemClassService>();
            var list = service.GetPosItemActionListByItemClassId(CurrentInfo.HotelId, id);
            return Json(list.ToDataSourceResult(request));
        }
    }
}
