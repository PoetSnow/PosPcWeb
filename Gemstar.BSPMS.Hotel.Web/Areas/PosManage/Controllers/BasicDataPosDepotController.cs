using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosDepot;
using Gemstar.BSPMS.Hotel.Web.Controllers;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// 二级仓库列表
    /// </summary>
    [AuthPage(ProductType.Pos, "p99035017")]
    public class BasicDataPosDepotController : BaseEditInWindowController<PosDepot, IPosDepotService>
    {

        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_posDepot", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosDepotAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosDepotAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosDepotService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            ActionResult result = _Add(addViewModel, new PosDepot { Id = id, Hid = CurrentInfo.HotelId, IStatus = 1, ModifiedDate = DateTime.Now }, OpLogType.Pos二级仓库添加);

            return result;
        }
        #endregion


        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new PosDepotEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosDepotEditViewModel model)
        {
            var modelService = GetService<IPosDepotService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            model.ModifiedDate = DateTime.Now;
            ActionResult result = _Edit(model, new PosDepot(), OpLogType.Pos二级仓库修改);
            return result;
        }
        #endregion

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosDepotService>(), OpLogType.Pos二级仓库删除);
        }

        #endregion 批量删除

        #region 启用

        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            var service = GetService<IPosDepotService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.启用));
            return reval;
        }

        #endregion 启用

        #region 禁用

        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            var service = GetService<IPosDepotService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用));

            return reval;
        }

        #endregion 禁用
    }
}