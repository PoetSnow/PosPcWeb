using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosActionType;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos作法分类
    /// </summary>
    [AuthPage(ProductType.Pos, "p99090001")]
    public class BasicDataPosActionTypeController : BaseEditInWindowController<PosActionType, IPosActionTypeService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_ActionType", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new ActionTypeAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(ActionTypeAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosActionTypeService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            ActionResult result = _Add(addViewModel, new PosActionType { Id = id, Hid = CurrentInfo.HotelId, ModifiedDate = DateTime.Now }, OpLogType.Pos作法分类增加);

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new ActionTypeEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(ActionTypeEditViewModel model)
        {
            var modelService = GetService<IPosActionTypeService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            model.ModifiedDate = DateTime.Now;
            ActionResult result = _Edit(model, new PosActionType(), OpLogType.Pos作法分类修改);
            return result;
        }
        #endregion
        
        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosActionTypeService>(), OpLogType.Pos作法分类删除);
        }
        #endregion

        #region 下拉数据绑定
        /// <summary>
        /// 获取指定模块下的作法分类
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosActionTypeByModules()
        {
            var service = GetService<IPosActionTypeService>();
            var datas = service.GetActionTypeByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
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

            var service = GetService<IPosActionTypeService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.启用));
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

            var service = GetService<IPosActionTypeService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用));

            return reval;

        }
        #endregion
    }
}