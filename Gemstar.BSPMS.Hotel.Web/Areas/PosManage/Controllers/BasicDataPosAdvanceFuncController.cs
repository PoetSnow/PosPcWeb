using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosAdvanceFunc;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    [AuthPage(ProductType.Pos, "p99035014")]
    public class BasicDataPosAdvanceFuncController : BaseEditInWindowController<PosAdvanceFunc, IPosAdvanceFuncService>
    {

        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_AdvanceFunc", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new PosAdvanceFuncAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(PosAdvanceFuncAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.FuncCode;
            var modelService = GetService<IPosAdvanceFuncService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.FuncCode);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码！")); }
            if (!string.IsNullOrEmpty(Request["RefeId"]))
            {
                addViewModel.RefeId = Request["RefeId"].ToString();

            }
            if (!string.IsNullOrEmpty(Request["FuncGrade"]))
            {
                addViewModel.FuncGrade = Request["FuncGrade"].ToString();

            }
            if (!string.IsNullOrEmpty(Request["FuncType"]))
            {
                addViewModel.FuncType = Request["FuncType"].ToString();

            }

            ActionResult result = _Add(addViewModel, new PosAdvanceFunc { Id = id, Hid = CurrentInfo.HotelId, CreateDate = DateTime.Now }, OpLogType.Pos高级功能增加);

            return result;
        }
        #endregion

        #region 编辑
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new PosAdvanceFuncEditViewModel());
        }


        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(PosAdvanceFuncEditViewModel model)
        {
            var modelService = GetService<IPosAdvanceFuncService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.FuncCode, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码！")); }

            Type type = Request.Form.GetType();
            type.GetMethod("MakeReadWrite", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Request.Form, null);
            if (Request["RefeId"] == null)
            {
                Request.Form.Add("RefeId", "");
            }
            if (Request["FuncGrade"] == null)
            {
                Request.Form.Add("FuncGrade", "");
            }
            if (Request["FuncType"] == null)
            {
                Request.Form.Add("FuncType", "");
            }
            if (!string.IsNullOrEmpty(Request["RefeId"]))
            {
                model.RefeId = Request["RefeId"].ToString();

            }
            if (!string.IsNullOrEmpty(Request["FuncGrade"]))
            {
                model.FuncGrade = Request["FuncGrade"].ToString();

            }
            if (!string.IsNullOrEmpty(Request["FuncType"]))
            {
                model.FuncType = Request["FuncType"].ToString();

            }
            ActionResult result = _Edit(model, new PosAdvanceFunc(), OpLogType.Pos高级功能修改);
            return result;
        }
        #endregion

        #region 删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {

            var service = GetService<IPosAdvanceFuncService>();
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(JsonResultData.Failure("请选择要删除的数据！"));
            }
            if (id == "0")
            {
                return Json(JsonResultData.Failure("要删除的数据不存在！"));
            }
            foreach (var item in id.Split(','))
            {
                var model = service.Get(item);
                model.IsUsed = false;//禁用状态
                service.Update(model, new PosAdvanceFunc());
                service.AddDataChangeLog(OpLogType.Pos高级功能删除);
                service.Commit();
            }
            return Json(JsonResultData.Successed());
            // return _BatchDelete(id, GetService<IPosActionService>(), OpLogType.Pos作法基础资料删除);
        }
        #endregion

        #region 数据绑定


        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosMode()
        {
            var service = GetService<ICodeListService>();
            var datas = service.List("72");

            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForFuncGrade()
        {
            var service = GetService<ICodeListService>();
            var datas = service.List("82");

            var listItems = datas.Select(w => new SelectListItem { Value = w.code, Text = w.name }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}