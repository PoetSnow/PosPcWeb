using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosDiscType;
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
    [AuthPage(ProductType.Pos, "p99035009")]
    public class BasicDataPosDiscTypeController : BaseEditInWindowController<PosDiscType, IPosDiscTypeService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_DiscType", "");
            return View();
        }

        #region 增加

        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosDiscTypeAddViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosDiscTypeAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosDiscTypeService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            ActionResult result = _Add(addViewModel, new PosDiscType { Id = id, Hid = CurrentInfo.HotelId, ModifiedDate = DateTime.Now }, OpLogType.Pos作法分类增加);

            return result;
        }

        #endregion 增加

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new PosDiscTypeEditViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosDiscTypeEditViewModel model)
        {
            var modelService = GetService<IPosDiscTypeService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            model.ModifiedDate = DateTime.Now;
            ActionResult result = _Edit(model, new PosDiscType(), OpLogType.Pos作法分类修改);
            return result;
        }

        #endregion 修改

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosDiscTypeService>(), OpLogType.Pos作法分类删除);
        }

        #endregion 批量删除

        #region 下拉数据绑定

        [AuthButton(AuthFlag.None)]
        public ActionResult PosDiscTypeList()
        {
            var codeList = GetService<ICodeListService>();
            var datas = codeList.List("76").ToList();
            var code = "0,1,4";
            var listItems = datas.Where(m => code.Contains(m.code)).Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        #endregion 下拉数据绑定
    }
}