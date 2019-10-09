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
    /// Pos开台项目
    /// </summary>
    [AuthPage(ProductType.Pos, "p30002")]
    public class BasicDataPosTabOpenItemController : BaseEditInWindowController<PosTabOpenItem, IPosTabOpenItemService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_tabOpenItem", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosTabOpenItemAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosTabOpenItemAddViewModel addViewModel)
        {
            var id = Guid.NewGuid();
            var modelService = GetService<IPosTabOpenItemService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Module, addViewModel.Refeid, addViewModel.TabTypeid, addViewModel.CustomerTypeid, addViewModel.Itemid, addViewModel.Unitid, addViewModel.IsCharge, addViewModel.ITagperiod, addViewModel.StartTime, addViewModel.EndTime);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            ActionResult result = _Add(addViewModel, new PosTabOpenItem { Id = id, Hid = CurrentInfo.HotelId, ModifiedDate = DateTime.Now }, OpLogType.Pos开台项目增加);

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            return _Edit(id, new PosTabOpenItemEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosTabOpenItemEditViewModel model)
        {
            var modelService = GetService<IPosTabOpenItemService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Module, model.Refeid, model.TabTypeid, model.CustomerTypeid,model.Itemid,model.Unitid,model.IsCharge, model.ITagperiod, model.StartTime, model.EndTime, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            ActionResult result = _Edit(model, new PosTabOpenItem { ModifiedDate = DateTime.Now }, OpLogType.Pos开台项目修改);
            return result;
        }
        #endregion
        
        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosTabOpenItemService>(), OpLogType.Pos开台项目删除);
        }
        #endregion
    }
}