using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemMultiClass;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos消费项目对应大类
    /// </summary>
    [AuthPage(ProductType.Pos, "p99020009")]
    public class BasicDataPosItemMultiClassController : BaseEditInWindowController<PosItemMultiClass, IPosItemMultiClassService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_ItemMultiClass", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosItemMultiClassAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosItemMultiClassAddViewModel addViewModel)
        {
            var id = Guid.NewGuid();
            var modelService = GetService<IPosItemMultiClassService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Itemid, addViewModel.ItemClassid);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(addViewModel.ItemClassid);
            ActionResult result = _Add(addViewModel, new PosItemMultiClass { Id = id, Hid = CurrentInfo.HotelId, IsSubClass = item == null ? null : item.IsSubClass, Modified = DateTime.Now }, OpLogType.Pos消费项目对应大类增加);
            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(Guid id)
        {
            return _Edit(id, new PosItemMultiClassEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosItemMultiClassEditViewModel model)
        {
            var modelService = GetService<IPosItemMultiClassService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Itemid, model.ItemClassid, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            var itemService = GetService<IPosItemService>();
            var item = itemService.Get(model.ItemClassid);
            if(item != null)
            {
                model.IsSubClass = item.IsSubClass;
            }
            model.Modified = DateTime.Now;

            ActionResult result = _Edit(model, new PosItemMultiClass(), OpLogType.Pos消费项目对应大类修改);
            return result;
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosItemMultiClassService>(), OpLogType.Pos消费项目对应大类删除);
        }
        #endregion
    }
}
