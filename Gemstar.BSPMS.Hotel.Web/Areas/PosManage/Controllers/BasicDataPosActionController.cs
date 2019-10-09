using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosAction;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosActionMultisub;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos作法基础资料
    /// </summary>
    [AuthPage(ProductType.Pos, "p99090003")]
    public class BasicDataPosActionController : BaseEditInWindowController<PosAction, IPosActionService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_Action", "");
            return View();
        }

        #region 增加

        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new ActionAddViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(ActionAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var modelService = GetService<IPosActionService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            if (Request["ProdPrinter"] != null)
            {
                addViewModel.ProdPrinter = Request["ProdPrinter"].ToString().Replace(",", "");
            }

            ActionResult result = _Add(addViewModel, new PosAction { Id = id, Hid = CurrentInfo.HotelId, ModifiedDate = DateTime.Now }, OpLogType.Pos作法基础资料增加);

            return result;
        }

        #endregion 增加

        #region 修改

        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            ViewBag.id = id;
            return _Edit(id, new ActionEditViewModel());
        }

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(ActionEditViewModel model)
        {
            var modelService = GetService<IPosActionService>();
            bool isexsit = modelService.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }

            //出品打印机为空时追加表单提交信息
            Type type = Request.Form.GetType();
            type.GetMethod("MakeReadWrite", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(Request.Form, null);
            if (Request["ProdPrinter"] != null)
            {
                model.ProdPrinter = Request["ProdPrinter"].ToString().Replace(",", "");
            }
            else
            {
                Request.Form.Add("ProdPrinter", "");
            }

            model.ModifiedDate = DateTime.Now;
            ActionResult result = _Edit(model, new PosAction(), OpLogType.Pos作法基础资料修改);
            return result;
        }

        #endregion 修改

        #region 批量删除

        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosActionService>(), OpLogType.Pos作法基础资料删除);
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
            var service = GetService<IPosActionService>();
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
            var service = GetService<IPosActionService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用));

            return reval;
        }

        #endregion 禁用

        #region 同组作法

        /// <summary>
        /// 列表查询
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public ActionResult ListActionMultisubByActionId(string id, [DataSourceRequest]DataSourceRequest request)
        {
            var service = GetService<IPosActionMultisubService>();
            var list = service.GetPosActionMultisubByactionid(CurrentInfo.HotelId, id);
            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加消费项目对应作法视图
        /// </summary>
        /// <param name="id">消费项目ID</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public PartialViewResult _AddActionMultisub(string id)
        {
            PosActionMultisubAddViewModel viewModel = new PosActionMultisubAddViewModel();
            viewModel.Actionid = id;
            return PartialView("_AddActionMultisub", viewModel);
        }

        /// <summary>
        /// 添加同组作法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [AuthButton(AuthFlag.None)]
        public ActionResult AddPosActionMultisub(PosActionMultisubAddViewModel model)
        {
            var id = Guid.NewGuid();
            var service = GetService<IPosActionMultisubService>();

            if (string.IsNullOrEmpty(model.Actionid2))
            {
                return Json(JsonResultData.Failure("请选择同组作法"));
            }

            if (model.Actionid.Trim() == model.Actionid2.Trim())
            {
                return Json(JsonResultData.Failure("同组作法不能和当前作法相同"));
            }

            var boolResult = service.IsExists(CurrentInfo.HotelId, model.Actionid, model.Actionid2);
            if (boolResult)
            {
                return Json(JsonResultData.Failure("添加了重复的同组作法"));
            }

            var newModel = new PosActionMultisub();
            AutoSetValueHelper.SetValues(model, newModel);

            newModel.Modified = DateTime.Now;
            newModel.Id = id;
            newModel.Hid = CurrentInfo.HotelId;
            service.Add(newModel);
            service.AddDataChangeLog(OpLogType.Pos同组作法增加);
            service.Commit();

            return Json(JsonResultData.Successed());
        }

        /// <summary>
        /// 修改同组作法视图
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

        [AuthButton(AuthFlag.None)]
        public PartialViewResult _EditActionMultisub(string Id)
        {
            PosActionMultisubEditViewModel viewModel = new PosActionMultisubEditViewModel();
            var service = GetService<IPosActionMultisubService>();
            var entity = service.Get(new Guid(Id));
            var serializer = new JavaScriptSerializer();
            AutoSetValueHelper.SetValues(entity, viewModel);
            viewModel.OriginJsonData = ReplaceJsonDateToDateString(serializer.Serialize(entity));
            return PartialView("_EditActionMultisub", viewModel);
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult EditPosActionMultisub(PosActionMultisubEditViewModel model)
        {
            var service = GetService<IPosActionMultisubService>();

            if (string.IsNullOrEmpty(model.Actionid2))
            {
                return Json(JsonResultData.Failure("请选择同组作法"));
            }

            if (model.Actionid.Trim() == model.Actionid2.Trim())
            {
                return Json(JsonResultData.Failure("同组作法不能和当前作法相同"));
            }

            var newModel = service.Get(model.Id);
            var boolResult = service.IsExists(CurrentInfo.HotelId, model.Actionid, model.Actionid2, model.Id);
            if (boolResult)
            {
                return Json(JsonResultData.Failure("添加了重复的同组作法"));
            }

            AutoSetValueHelper.SetValues(model, newModel);
            try
            {
                newModel.Modified = DateTime.Now;
                service.Update(newModel, new PosActionMultisub());
                service.AddDataChangeLog(OpLogType.Pos同组作法修改);
                service.Commit();

                return Json(JsonResultData.Successed());
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex.Message.ToString()));
            }
        }

        [AuthButton(AuthFlag.None)]
        public ActionResult BatchDeleteActionMultisub(string id)
        {
            var service = GetService<IPosActionMultisubService>();
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
                var model = service.Get(new Guid(item));

                service.Delete(model);
                service.AddDataChangeLog(OpLogType.Pos同组作法删除);
                service.Commit();
            }
            return Json(JsonResultData.Successed());
        }

        #endregion 同组作法

        #region 下拉数据绑定

        /// <summary>
        /// 获取指定模块下的作法分类
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosActionTypeByModules()
        {
            var service = GetService<IPosActionService>();
            var datas = service.GetActionByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }

        #endregion 下拉数据绑定
    }
}