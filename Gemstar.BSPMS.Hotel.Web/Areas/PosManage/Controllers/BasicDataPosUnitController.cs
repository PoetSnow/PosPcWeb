using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosUnit;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos单位定义
    /// </summary>
    [AuthPage(ProductType.Pos, "p99020005")]
    public class BasicDataPosUnitController : BaseEditInWindowController<PosUnit, IPosUnitService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_Unit", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new UnitAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(UnitAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosUnitService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            ActionResult result = _Add(addViewModel, new PosUnit { Id = id, Hid = CurrentInfo.HotelId, ModifiedDate = DateTime.Now }, OpLogType.Pos单位定义增加);

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new UnitEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(UnitEditViewModel model)
        {
            var modelService = GetService<IPosUnitService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            model.ModifiedDate = DateTime.Now;
            ActionResult result = _Edit(model, new PosUnit(), OpLogType.Pos单位定义修改);
            return result;
        }
        #endregion
        
        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosUnitService>(), OpLogType.Pos单位定义删除);
        }
        #endregion

        #region 下拉数据绑定
        /// <summary>
        /// 获取指定模块下的单位定义
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosUnit()
        {
            var service = GetService<IPosUnitService>();
            var datas = service.GetUnit(CurrentInfo.HotelId);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取指定模块下的单位定义
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosUnitByModules()
        {
            var service = GetService<IPosUnitService>();
            var datas = service.GetUnitByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
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

            var service = GetService<IPosUnitService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.启用,OpLogType.Pos单位定义启用禁用));
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

            var service = GetService<IPosUnitService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用, OpLogType.Pos单位定义启用禁用));

            return reval;

        }
        #endregion
    }
}