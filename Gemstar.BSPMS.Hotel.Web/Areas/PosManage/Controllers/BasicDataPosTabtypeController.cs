using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosTabtype;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos餐台类型
    /// </summary>
    [AuthPage(ProductType.Pos, "p99070001")]
    public class BasicDataPosTabtypeController : BaseEditInWindowController<PosTabtype, IPosTabtypeService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_tabtype", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new TabtypeAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(TabtypeAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosTabtypeService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            ActionResult result = _Add(addViewModel, new PosTabtype { Id = id, Hid = CurrentInfo.HotelId, ModifiedDate = DateTime.Now }, OpLogType.Pos餐台类型增加);

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new TabtypeEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(TabtypeEditViewModel model)
        {
            var modelService = GetService<IPosTabtypeService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            model.ModifiedDate = DateTime.Now;
            ActionResult result = _Edit(model, new PosTabtype(), OpLogType.Pos餐台类型修改);
            return result;
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosTabtypeService>(), OpLogType.Pos餐台类型删除);
        }
        #endregion

        #region 下拉数据绑定
        /// <summary>
        /// 获取指定酒店下的餐台类型
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosTabtype()
        {
            var service = GetService<IPosTabtypeService>();
            var datas = service.GetTabtype(CurrentInfo.HotelId).Where(w => w.Module == "CY").ToList();
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取指定模块下的餐台类型
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosTabtypeByModules()
        {
            var service = GetService<IPosTabtypeService>();
            var datas = service.GetTabtypeByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
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

            var service = GetService<IPosTabtypeService>();
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

            var service = GetService<IPosTabtypeService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用));

            return reval;

        }
        #endregion
    }
}