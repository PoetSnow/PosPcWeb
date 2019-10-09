using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosCustomerType;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Controllers
{
    /// <summary>
    /// Pos客人类型
    /// </summary>
    [AuthPage(ProductType.Pos, "p99035001")]
    public class BasicDataPosCustomerTypeController : BaseEditInWindowController<PosCustomerType, IPosCustomerTypeService>
    {
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_pos_list_CustomerType", "");
            return View();
        }

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new CustomerTypeAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(CustomerTypeAddViewModel addViewModel)
        {
            var id = CurrentInfo.HotelId + addViewModel.Code;
            var service = GetService<IPosCustomerTypeService>();
            if (addViewModel.IsDefault == true)
            {
                var list = service.GetPosCustomerType(CurrentInfo.HotelId);
                if(list != null && list.Count > 0)
                {
                    foreach(var temp in list)
                    {
                        if(temp.IsDefault == true)
                        {
                            temp.IsDefault = false;
                            service.Update(temp, new PosCustomerType());
                            service.AddDataChangeLog(OpLogType.Pos客人类型修改);
                            service.Commit();
                        }
                    }
                }
            }
            bool isexsit = service.IsExists(CurrentInfo.HotelId, addViewModel.Code, addViewModel.Cname);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            ActionResult result = _Add(addViewModel, new PosCustomerType { Id = id, Hid = CurrentInfo.HotelId, ModifiedDate = DateTime.Now }, OpLogType.Pos客人类型增加);

            return result;
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(id, new CustomerTypeEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(CustomerTypeEditViewModel model)
        {
            var service = GetService<IPosCustomerTypeService>();
            bool isexsit = service.IsExists(CurrentInfo.HotelId, model.Code, model.Cname, model.Id);
            if (isexsit) { return Json(JsonResultData.Failure("操作错误,重复代码 或 重复名称！")); }
            model.ModifiedDate = DateTime.Now;
            ActionResult result = _Edit(model, new PosCustomerType(), OpLogType.Pos客人类型修改);
            if (model.IsDefault == true)
            {
                var list = service.GetPosCustomerType(CurrentInfo.HotelId);
                if (list != null && list.Count > 0)
                {
                    foreach (var temp in list)
                    {
                        if (temp.Id != model.Id && temp.IsDefault == true)
                        {
                            temp.IsDefault = false;
                            service.Update(temp, new PosCustomerType());
                            service.AddDataChangeLog(OpLogType.Pos客人类型修改);
                            service.Commit();
                        }
                    }
                }
            }
            return result;
        }
        #endregion
        
        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IPosCustomerTypeService>(), OpLogType.Pos客人类型删除);
        }
        #endregion

        #region 下拉数据绑定
        /// <summary>
        /// 获取指定酒店下的客人类型
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosCustomerType()
        {
            var service = GetService<IPosCustomerTypeService>();
            var datas = service.GetPosCustomerType(CurrentInfo.HotelId);
            var listItems = datas.Select(w => new SelectListItem { Value = w.Id, Text = w.Cname }).ToList();
            return Json(listItems, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取指定模块下的客人类型
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.None)]
        public JsonResult ListItemsForPosCustomerTypeByModules()
        {
            var service = GetService<IPosCustomerTypeService>();
            var datas = service.GetPosCustomerTypeByModule(CurrentInfo.HotelId, CurrentInfo.ModuleCode);
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

            var service = GetService<IPosCustomerTypeService>();
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

            var service = GetService<IPosCustomerTypeService>();
            var reval = Json(service.BatchUpdateStatus(id, EntityStatus.禁用));

            return reval;

        }
        #endregion
    }
}