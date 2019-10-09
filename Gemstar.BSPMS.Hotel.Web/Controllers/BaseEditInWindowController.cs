using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Web.Models.BasicDatas;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Services;
using System.Linq;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Web.Controllers
{
    /// <summary>
    /// 弹出窗口进行编辑的基类，将公共的方法进行封装，以减少代码量
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseEditInWindowController<T, I> : BaseController where T : class where I : ICRUDService<T>
    {
        #region 增加
        /// <summary>
        /// 返回增加的分部视图
        /// </summary>
        /// <typeparam name="TV">分部视图需要的视图对象类型</typeparam>
        /// <param name="viewModel">分部视图需要的视图对象实例</param>
        /// <param name="viewName">视图名称，只有在集团和单体酒店增加需要使用不同视图时才需要手工指定</param>
        /// <returns>分部视图</returns>
        [AuthButton(AuthFlag.Add)]
        protected ActionResult _Add<TV>(TV viewModel, string viewName = "_Add")
        {
            return PartialView(viewName, viewModel);
        }
        /// <summary>
        /// 返回集团增加的分部视图
        /// </summary>
        /// <typeparam name="TV">分部视图需要的视图对象类型</typeparam>
        /// <param name="viewModel">分部视图需要的视图对象实例</param>
        /// <param name="basicDataCode">基础数据代码</param>
        /// <param name="viewName">视图名称</param>
        /// <returns>分部视图</returns>
        [AuthButton(AuthFlag.Add)]
        protected ActionResult _AddGroup<TV>(TV viewModel, string basicDataCode, string viewName = "_AddGroup") where TV : BasicDataGroupAddViewModel
        {
            var controlService = GetService<IBasicDataResortControlService>();
            var dataControl = controlService.GetGroupSetDataCopyType(basicDataCode, CurrentInfo.HotelId, CurrentInfo.GroupId);
            //分发选择酒店时与权限无关，直接选择集团下的所有分店
            var hotelService = GetService<IPmsHotelService>();
            var resorts = hotelService.GetHotelsInGroupExceptGroupHotel(CurrentInfo.GroupId);
            var resortItems = new List<SelectListItem>();
            var hidList = (dataControl.SelectedHids ?? "").Split(',');
            foreach (var r in resorts)
            {
                var item = new SelectListItem { Value = r.Hid, Text = r.Name };
                if (hidList.Contains(r.Hid))
                {
                    item.Selected = true;
                }
                resortItems.Add(item);
            }
            viewModel.DataControlCode = dataControl.Code;
            viewModel.DataControlName = dataControl.Name;
            viewModel.ResortItems = resortItems;
            return PartialView(viewName, viewModel);
        }
        /// <summary>
        /// 处理增加
        /// </summary>
        /// <typeparam name="TV">增加的分部视图的视图对象类型</typeparam>
        /// <param name="viewModel">增加的分部视图的视图对象实例</param>
        /// <param name="dataModel">需要保存到数据库的数据库实体实例</param>
        /// <returns>保存的结果</returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        protected ActionResult _Add<TV>(TV viewModel, T dataModel, OpLogType opType)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var service = GetService<I>();
                    AutoSetValueHelper.SetValues(viewModel, dataModel);
                    service.Add(dataModel);
                    service.AddDataChangeLog(opType);
                    service.Commit();
                    AfterAddedAndSaved(dataModel);
                    return Json(JsonResultData.Successed(""));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            return Json(JsonResultData.Failure(ModelState.Values));
        }
        /// <summary>
        /// 处理集团增加并且分发基础资料
        /// </summary>
        /// <typeparam name="TV">增加的分部视图的视图对象类型</typeparam>
        /// <param name="viewModel">增加的分部视图的视图对象实例</param>
        /// <param name="dataModel">需要保存到数据库的数据库实体实例</param>
        /// <param name="opType">操作日志类型</param>
        /// <returns>保存的结果</returns>
        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        [JsonException]
        protected ActionResult _AddGroup<TV>(TV viewModel, T dataModel, OpLogType opType) where TV : BasicDataGroupAddViewModel
        {
            var hid = CurrentInfo.GroupId;

            if (ModelState.IsValid)
            {
                try
                {
                    var service = GetService<I>();
                    AutoSetValueHelper.SetValues(viewModel, dataModel);
                    service.AddAndCopy(dataModel, hid, viewModel.DataControlCode, viewModel.SelectedResortHids);
                    service.AddDataChangeLog(opType);
                    service.Commit();
                    return Json(JsonResultData.Successed(""));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            return Json(JsonResultData.Failure(ModelState.Values));
        }
        /// <summary>
        /// 增加保存完成后调用，在子类需要做某些操作时，可以重写此方法
        /// </summary>
        /// <param name="data">刚保存的对象</param>
        protected virtual void AfterAddedAndSaved(T data) { }
        #endregion

        #region 修改
        /// <summary>
        /// 判断要修改的基础资料记录是否允许修改
        /// </summary>
        /// <typeparam name="U">修改记录类型</typeparam>
        /// <param name="model">修改记录</param>
        /// <param name="basicDataCode">基础数据代码</param>
        /// <returns>null：允许修改，其他：不允许修改</returns>
        protected ActionResult _CanEdit<U>(U model, string basicDataCode) where U : IBasicDataCopyEntity
        {
            if (model == null)
            {
                return Content("指定的要修改记录不存在");
            }
            if (CurrentInfo.IsGroup)
            {
                if (string.IsNullOrWhiteSpace(model.DataSource) || model.DataSource == BasicDataDataSource.Added.Code)
                {
                    //如果是自主增加的,则判断是否是当前酒店自己增加的
                    if (model.Hid != CurrentInfo.HotelId)
                    {
                        //其他酒店增加的记录，不允许修改
                        return Content("不允许修改其他酒店自主增加的记录");
                    }
                }
                else
                {
                    //集团分发的记录
                    if (CurrentInfo.IsHotelInGroup)
                    {
                        var resortControlService = GetService<IBasicDataResortControlService>();
                        var resortControl = resortControlService.GetResortControl(basicDataCode, CurrentInfo.GroupId);
                        //如果集团设置为分店不允许修改                        
                        if (resortControl!=null && !resortControl.ResortCanUpdate)
                        {
                            return Content("集团分发型资料，集团设置为不允许修改");
                        }
                    }
                    else if (CurrentInfo.IsGroupInGroup)
                    {
                        if (model.DataSource == BasicDataDataSource.Copyed.Code)
                        {
                            return Content("不允许修改集团分发的分店资料");
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 修改的分部视图
        /// </summary>
        /// <typeparam name="TV">修改分部视图需要的视图对象类型</typeparam>
        /// <param name="idForT">要修改的数据库实体实例的主键值，类型必须与实体定义的类型相同</param>
        /// <param name="viewModel">修改分部视图需要的视图对象实例</param>
        /// <param name="viewName">视图名称，只有在集团和单体酒店修改视图名称不相同时才需要手工指定</param>
        /// <returns>修改的分部视图</returns>
        [AuthButton(AuthFlag.Update)]
        protected ActionResult _Edit<TV>(object idForT, TV viewModel, string viewName = "_Edit") where TV : BaseEditViewModel
        {
            var service = GetService<I>();
            var user = service.Get(idForT);
            var serializer = new JavaScriptSerializer();
            AutoSetValueHelper.SetValues(user, viewModel);
            viewModel.OriginJsonData = serializer.Serialize(user);
            return PartialView(viewName, viewModel);
        }
        /// <summary>
        /// 修改并且分发集团基础资料的分部视图
        /// </summary>
        /// <typeparam name="TV">修改分部视图需要的视图对象类型</typeparam>
        /// <param name="idForT">要修改的数据库实体实例的主键值，类型必须与实体定义的类型相同</param>
        /// <param name="viewModel">修改分部视图需要的视图对象实例</param>
        /// <param name="basicDataCode">基础数据代码</param>
        /// <param name="viewName">视图名称</param>
        /// <returns>修改的分部视图</returns>
        [AuthButton(AuthFlag.Update)]
        protected ActionResult _EditGroup<TV>(object idForT, TV viewModel, string basicDataCode, string viewName = "_EditGroup") where TV : BasicDataGroupEditViewModel
        {
            var service = GetService<I>();
            var user = service.Get(idForT);
            var serializer = new JavaScriptSerializer();
            AutoSetValueHelper.SetValues(user, viewModel);
            viewModel.OriginJsonData = serializer.Serialize(user);

            var controlService = GetService<IBasicDataResortControlService>();
            var dataControl = controlService.GetResortControl(basicDataCode, CurrentInfo.GroupId);
            if (dataControl == null)
            { 
                dataControl = new Services.Entities.BasicDataResortControl() { DataCopyType= DataControlType.AllResorts.Code ,SelectedHids=""};
            }
            viewModel.DataControlCode = dataControl.DataCopyType;
            //判断是否是固定分发类型
            if (dataControl.DataCopyType == DataControlType.AllResorts.Code)
            {
                viewModel.DataControlName = DataControlType.AllResorts.Name;
            }
            else if (dataControl.DataCopyType == DataControlType.SelectedResorts.Code)
            {
                viewModel.DataControlName = DataControlType.SelectedResorts.Name;
            }
            else
            {
                //取出分店属性分发类型
                var allResortManageTypes = controlService.GetDataControlTypes();
                viewModel.DataControlName = allResortManageTypes.Single(w => w.Code == dataControl.DataCopyType).Name;
            }
            //分发选择酒店时与权限无关，直接选择集团下的所有分店
            var hotelService = GetService<IPmsHotelService>();
            var resorts = hotelService.GetHotelsInGroupExceptGroupHotel(CurrentInfo.GroupId);
            var resortItems = new List<SelectListItem>();
            var hidList = (dataControl.SelectedHids ?? "").Split(',');
            foreach (var r in resorts)
            {
                var item = new SelectListItem { Value = r.Hid, Text = r.Name };
                if (hidList.Contains(r.Hid))
                {
                    item.Selected = true;
                }
                resortItems.Add(item);
            }
            viewModel.ResortItems = resortItems;
            //设置分店是否可修改，以及保存时要更新的属性列表让操作员选择
            viewModel.ResortCanUpdate = dataControl.ResortCanUpdate;
            var type = typeof(T);
            var propertyItems = BasicDataUpdateAttribute.GetBasicDataUpdateAttributeProperties(type);
            viewModel.PropertyItems = propertyItems;

            return PartialView(viewName, viewModel);
        }
        /// <summary>
        /// 处理修改
        /// </summary>
        /// <typeparam name="TV">修改分部视图需要的视图对象类型</typeparam>
        /// <param name="viewModel">修改分部视图需要的视图对象实例</param>
        /// <param name="dataModel">要保存到数据库的实体实例</param>
        /// <returns>修改结果</returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        protected ActionResult _Edit<TV>(TV viewModel, T dataModel, OpLogType opType) where TV : BaseEditViewModel
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var originUser = viewModel.GetOriginObject<T>();
                    List<string> fieldNames;
                    AutoSetValueHelper.SetValues(viewModel, dataModel, Request.Form, out fieldNames);
                    var service = GetService<I>();
                    service.Update(dataModel, originUser, fieldNames);
                    service.AddDataChangeLog(opType);
                    service.Commit();
                    AfterEditedAndSaved(dataModel);
                    return Json(JsonResultData.Successed(""));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            return Json(JsonResultData.Failure(ModelState.Values));
        }
        /// <summary>
        /// 修改保存完成后调用，在子类需要做某些操作时，可以重写此方法
        /// </summary>
        /// <param name="data">刚保存的对象</param>
        protected virtual void AfterEditedAndSaved(T data) { }
        /// <summary>
        /// 处理集团修改并且进行分发
        /// </summary>
        /// <typeparam name="TV">修改分部视图需要的视图对象类型</typeparam>
        /// <param name="model">修改分部视图需要的视图对象实例</param>
        /// <param name="dataModel">要保存到数据库的实体实例</param>
        /// <param name="opType">操作日志类型</param>
        /// <returns>修改结果</returns>
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        protected ActionResult _EditGroup<TV>(TV model, T dataModel, OpLogType opType) where TV : BasicDataGroupEditViewModel
        {
            if (!model.ResortCanUpdate)
            {
                //如果分店不允许修改，则前端界面上的属性列表选择会被禁用，不会传递选中的属性列表回来，此处直接赋值所有可更新属性
                model.UpdateProperties = BasicDataUpdateAttribute.GetBasicDataUpdateAttributePropertyNames(typeof(T));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var originUser = model.GetOriginObject<T>();
                    List<string> fieldNames;
                    AutoSetValueHelper.SetValues(model, dataModel, Request.Form, out fieldNames);
                    var service = GetService<I>();
                    var updatedTList = service.EditAndCopy(dataModel, originUser, fieldNames, CurrentInfo.GroupId, model.DataControlCode, model.SelectedResortHids, model.UpdateProperties);
                    service.AddDataChangeLog(opType);
                    service.Commit();
                    AfterEditedGroupAndSaved(updatedTList);
                    return Json(JsonResultData.Successed(""));
                }
                catch (Exception ex)
                {
                    return Json(JsonResultData.Failure(ex));
                }
            }
            return Json(JsonResultData.Failure(ModelState.Values));
        }
        /// <summary>
        /// 修改保存完成后调用，在子类需要做某些操作时，可以重写此方法
        /// </summary>
        /// <param name="data">刚保存的对象</param>
        protected virtual void AfterEditedGroupAndSaved(List<T> data) { }
        #endregion

        [AuthButton(AuthFlag.Update)]
        protected ActionResult _Detail<TV>(object idForT, TV viewModel) where TV : BaseEditViewModel
        {
            var service = GetService<I>();
            var user = service.Get(idForT);
            var serializer = new JavaScriptSerializer();
            AutoSetValueHelper.SetValues(user, viewModel);
            viewModel.OriginJsonData = serializer.Serialize(user);
            return PartialView("_Detail", viewModel);
        }
    }
}