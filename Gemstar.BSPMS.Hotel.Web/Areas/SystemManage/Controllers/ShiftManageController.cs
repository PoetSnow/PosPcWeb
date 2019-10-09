using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services.Enums;
using System.Web.Security;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 班次维护
    /// </summary>
    [AuthPage("99030")]
    [AuthPage(ProductType.Member, "m99040")]
    [BusinessType("角色管理")]
    public class ShiftManageController : BaseEditIncellController<Shift, IShiftService>
    { 
        public string comfirmtext = "集团分发型基础资料，被集团设置为分店不允许此操作。";
        #region 查询  
        [AuthButton(AuthFlag.Query)]
        // GET: SystemManage/ShiftManage
        public ActionResult Index()
        {
            if (CurrentInfo.IsGroupInGroup)
            {
                SetCommonQueryValues("up_list_shift_Group", "");
                return RedirectToAction("Index", "ShiftGroup"); //View("/ShiftGroupManage/Index"); 

            }
            else
            {
                if (CurrentInfo.IsHotelInGroup)
                {
                    ViewBag.isCanAdd = ishotelCanDo("add", "");
                }
                else
                {
                    ViewBag.isCanAdd = true;
                }
                SetCommonQueryValues("up_list_shift", "");
                return View();
            }
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        [KendoGridDatasourceException]
        public ActionResult Add([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<Shift> addClass)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            foreach (var model in addClass)
            {
                if (string.IsNullOrWhiteSpace(model.Code) || string.IsNullOrWhiteSpace(model.ShiftName))
                {
                    ModelState.AddModelError("Name", "班次代码和班次名称不能为空");
                    return Json(addClass.ToDataSourceResult(request, ModelState));
                }
                string strbegin = CheckDatetimeType(model.BeginTime == null ? "" : model.BeginTime);
                string strend = CheckDatetimeType(model.EndTime == null ? "" : model.EndTime);
                if (strbegin != "")
                {
                    ModelState.AddModelError("BeginTime", strbegin);
                    return Json(addClass.ToDataSourceResult(request, ModelState));
                }
                else if (strend != "")
                {
                    ModelState.AddModelError("EndTime", strend);
                    return Json(addClass.ToDataSourceResult(request, ModelState));
                }
            }
            return _Add(request, addClass, w => { w.Hid = hid; w.Id = hid + w.Code; w.Status = EntityStatus.启用; w.DataSource = BasicDataDataSource.Added.Code; }, OpLogType.班次增加);
        }

        /// <summary>
        /// 时间格式判断
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public string CheckDatetimeType(string time)
        {
            if (time.Trim().Length != 5 && time.Trim().Length != 4) { return "时间格式不正确！"; }
            if (time.Trim().Length == 4) { time = "0" + time; };
            int k = 0;
            string bt = time.Substring(0, 2);
            string et = time.Substring(3, 2);
            if (!int.TryParse(bt, out k) || !int.TryParse(et, out k))
            { return "时间格式不正确！"; }

            int a = int.Parse(bt);
            string b = time.Substring(2, 1);
            int c = int.Parse(et);
            if (a > 23 || c > 59 || b != ":")
            {
                return "时间格式不正确！";
            }
            return "";
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        [KendoGridDatasourceException]
        public ActionResult Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<Shift> updateClass, [Bind(Prefix = "originModels")]IEnumerable<Shift> originClass)
        {

            foreach (var model in updateClass)
            {
                if (string.IsNullOrWhiteSpace(model.Code) || string.IsNullOrWhiteSpace(model.ShiftName))
                {
                    ModelState.AddModelError("Name", "班次代码和班次名称不能为空");
                    return Json(updateClass.ToDataSourceResult(request, ModelState));
                }
                string strbegin = CheckDatetimeType(model.BeginTime == null ? "" : model.BeginTime);
                string strend = CheckDatetimeType(model.EndTime == null ? "" : model.EndTime);
                if (strbegin != "")
                {
                    ModelState.AddModelError("Name", strbegin);
                    return Json(updateClass.ToDataSourceResult(request, ModelState));
                }
                else if (strend != "")
                {
                    ModelState.AddModelError("Name", strend);
                    return Json(updateClass.ToDataSourceResult(request, ModelState));
                }
                if (model.Seqid == null)
                {
                    model.Seqid = 0;
                }
                foreach (var model1 in originClass)
                {
                    if (model.Id == model1.Id)
                    {
                        if (model.Code != model1.Code)
                        {
                            bool exsit = GetService<IShiftService>().IsExsitrResFolio(GetService<ICurrentInfo>().HotelId, model1.Id);
                            if (GetService<ICurrentInfo>().ShiftId == model1.Id)
                            {
                                ModelState.AddModelError("code", "班次正在使用中不可修改班次代码！");
                                return Json(updateClass.ToDataSourceResult(request, ModelState));
                            }
                            else if (exsit)
                            {
                                ModelState.AddModelError("code", "已使用班次代码不可修改！");
                                return Json(updateClass.ToDataSourceResult(request, ModelState));
                            }
                        }
                        if (!ishotelCanDo("update", "") && model1.DataSource == BasicDataDataSource.Copyed.Code)
                        {
                            ModelState.AddModelError("code", "【班次名称：" + model1.ShiftName + "】为"+ comfirmtext);
                            return Json(updateClass.ToDataSourceResult(request, ModelState));
                        }
                    }
                }

            }

            return _Update(request, updateClass, originClass, (list, u) => list.SingleOrDefault(w => w.Id == u.Id), OpLogType.班次修改, w => { w.Id = string.Format("{0}{1}", w.Hid, w.Code); });
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<IShiftService>(), OpLogType.班次删除);
        }
        #endregion
        #region 启用禁用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            if (!ishotelCanDo("disable", id))
            {
                return Json(JsonResultData.Failure(comfirmtext));
            }
            var _hotelService = GetService<IShiftService>();
            return Json(_hotelService.Enable(id));
        }

        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            if (!ishotelCanDo("disable", id))
            {
                return Json(JsonResultData.Failure(comfirmtext));
            }
            var _hotelService = GetService<IShiftService>();
            return Json(_hotelService.Disable(id));
        }
        #endregion
        
        #region 删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult CheckForDelete(string[] id)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            var shiftserv = GetService<IShiftService>();
            for (int i = 0; i < id.Count(); i++)
            {
                bool exsit = shiftserv.IsExsitrResFolio(hid, id[i]);
                if (currentInfo.ShiftId == id[i])
                {
                    return Json(JsonResultData.Failure("班次正在使用中不可删除！"));

                }
                else if (exsit)
                {
                    return Json(JsonResultData.Failure("班次在账务明细表中已使用不可删除！"));
                }
                if (!ishotelCanDo("del", id[i]))
                {
                    return Json(JsonResultData.Failure(comfirmtext));
                }
            }
            return Json(JsonResultData.Successed("可以删除！"));
        }
        #endregion

        #region 关闭班次
        /// <summary>
        /// 关闭当前班次
        /// </summary>
        /// <returns>关闭结果</returns>
        [AuthButton(AuthFlag.Close)]
        public ActionResult CloseShift()
        {
            var shiftService = GetService<IShiftService>();
            //不能关闭未开的班次
            var result = shiftService.CloseShift(CurrentInfo.HotelId, CurrentInfo.ShiftId);
            if (result.Success)
            {
                var businessDay = GetService<IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId).ToString("yyyy-MM-dd");
                AddOperationLog(OpLogType.关闭班次, string.Format("班次：{0}，营业日：{1}", CurrentInfo.ShiftName, businessDay));
                //CurrentInfo.Clear();
                //Session.Abandon();
                return Json(JsonResultData.Successed(FormsAuthentication.LoginUrl));
            }
            return Json(result);
        }
        #endregion


        #region 分店的操作判断
        /// <summary>
        /// 判断分店是否可进行操作
        /// </summary>
        /// <param name="opType">操作类型</param>
        /// <param name="id">要操作的数据id</param>
        /// <returns></returns>
        public bool ishotelCanDo(string opType, string id)
        {

            bool retval = true;
            if (CurrentInfo.IsHotelInGroup)
            {

                var resortControl = GetService<IBasicDataResortControlService>().GetResortControl(M_V_BasicDataType.BasicDataCodeShift, CurrentInfo.GroupId);
                if (resortControl != null)
                {
                    if (opType == "add" || opType == "del")
                    {
                        retval = resortControl.ResortCanAdd;
                    }
                    if (opType == "update")
                    {
                        retval = resortControl.ResortCanUpdate;
                    }
                    if (opType == "disable")
                    {
                        retval = resortControl.ResortCanDisable;
                    }
                }
                if (id == "") { return retval; }
                var shiftserv = GetService<IShiftService>();
                Shift cur = shiftserv.Get(id);
                if (retval == false && cur.DataSource == BasicDataDataSource.Copyed.Code)
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

    }
}