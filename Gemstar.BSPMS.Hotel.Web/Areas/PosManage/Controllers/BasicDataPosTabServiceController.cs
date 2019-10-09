using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosService;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos服务费政策
    /// </summary>
    [AuthPage(ProductType.Pos, "p30001")]
    public class BasicDataPosTabServiceController : BaseEditInWindowController<PosTabService, IPosTabServiceService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_tabService", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosTabServiceAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosTabServiceAddViewModel addViewModel)
        {
            var id = Guid.NewGuid();
            var modelService = GetService<IPosTabServiceService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Module, addViewModel.Refeid, addViewModel.TabTypeid, addViewModel.CustomerTypeid, addViewModel.ITagperiod, addViewModel.StartTime, addViewModel.EndTime);
            if (isexsit) { return Json(JsonResultData.Failure("服务费政策时间重叠，请重新设置！")); }
            ActionResult result = _Add(addViewModel, new PosTabService { Id = id, Hid = CurrentInfo.HotelId, ModifiedDate = DateTime.Now }, OpLogType.Pos服务费政策增加);

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            return _Edit(id, new PosTabServiceEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosTabServiceEditViewModel model)
        {
            var modelService = GetService<IPosTabServiceService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Module, model.Refeid, model.TabTypeid, model.CustomerTypeid, model.ITagperiod, model.StartTime, model.EndTime, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("服务费政策时间重叠，请重新设置！")); }
            ActionResult result = _Edit(model, new PosTabService { ModifiedDate = DateTime.Now }, OpLogType.Pos服务费政策修改);
            return result;
        }
        #endregion
        
        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosTabServiceService>(), OpLogType.Pos服务费政策删除);
        }
        #endregion
    }
}