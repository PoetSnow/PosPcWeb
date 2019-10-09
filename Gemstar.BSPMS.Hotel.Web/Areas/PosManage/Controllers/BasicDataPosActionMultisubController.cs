using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosActionMultisub;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos同组作法
    /// </summary>
    [AuthPage(ProductType.Pos, "p99090004")]
    public class BasicDataPosActionMultisubController : BaseEditInWindowController<PosActionMultisub, IPosActionMultisubService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_ActionMultisub", "");
            return View();
        }

        #region 增加

        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosActionMultisubAddViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosActionMultisubAddViewModel addViewModel)
        {
            var id = Guid.NewGuid();
            var modelService = GetService<IPosActionMultisubService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Actionid, addViewModel.Actionid2);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            if (addViewModel.Actionid.Trim() == addViewModel.Actionid2.Trim())
            {
                return Json(JsonResultData.Failure("同组作法不能和当前作法相同"));
            }

            ActionResult result = _Add(addViewModel, new PosActionMultisub { Id = id, Hid = CurrentInfo.HotelId, Modified = DateTime.Now }, OpLogType.Pos同组作法增加);
            return result;
        }

        #endregion 增加

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            return _Edit(id, new PosActionMultisubEditViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosActionMultisubEditViewModel model)
        {
            var modelService = GetService<IPosActionMultisubService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Actionid, model.Actionid2, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            if (model.Actionid.Trim() == model.Actionid2.Trim())
            {
                return Json(JsonResultData.Failure("同组作法不能和当前作法相同"));
            }

            model.Modified = DateTime.Now;

            ActionResult result = _Edit(model, new PosActionMultisub(), OpLogType.Pos同组作法修改);
            return result;
        }

        #endregion 修改

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosActionMultisubService>(), OpLogType.Pos同组作法删除);
        }

        #endregion 批量删除
    }
}