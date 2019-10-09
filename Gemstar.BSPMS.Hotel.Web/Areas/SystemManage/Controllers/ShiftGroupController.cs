using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities; 
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.ShiftGroupManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 班次维护
    /// </summary>
    [AuthPage("99030")]
    [BusinessType("角色管理")]
    [AuthBasicData(M_V_BasicDataType.BasicDataCodeShift)]
    public class ShiftGroupController : BaseEditInWindowController<Shift, IShiftService>
    {
        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            SetCommonQueryValues("up_list_shift_Group", "");
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(string typeCode)
        {
            return _AddGroup(new ShiftGroupAddViewModel() { }, M_V_BasicDataType.BasicDataCodeShift, "_Add");
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(ShiftGroupAddViewModel ShiftAddViewModel)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;

            if (string.IsNullOrWhiteSpace(ShiftAddViewModel.Code) || string.IsNullOrWhiteSpace(ShiftAddViewModel.ShiftName))
            { 
                return Json(JsonResultData.Failure("班次代码和班次名称不能为空"));
            }
            string strbegin = CheckDatetimeType(ShiftAddViewModel.BeginTime == null ? "" : ShiftAddViewModel.BeginTime);
            string strend = CheckDatetimeType(ShiftAddViewModel.EndTime == null ? "" : ShiftAddViewModel.EndTime);
            if (strbegin != "")
            { 
                return Json(JsonResultData.Failure(strbegin));
            }
            else if (strend != "")
            { 
                return Json(JsonResultData.Failure(strend));
            }
            return _AddGroup(ShiftAddViewModel, new Shift {
                Hid = hid,
                Id = hid + ShiftAddViewModel.Code,
                Status = EntityStatus.启用,
                DataSource = BasicDataDataSource.Added.Code
            }, OpLogType.班次增加);
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
        public ActionResult Edit(string id)
        {
            var originEntity = GetService<IShiftService>().Get(id);
            var editResult = _CanEdit(originEntity, M_V_BasicDataType.BasicDataCodeRoomType);
            if (editResult != null)
            {
                return editResult;
            }
            ViewBag.canEditCode = CurrentInfo.IsGroup ? false : true;
            return _EditGroup(id, new ShiftGroupEditViewModel(), M_V_BasicDataType.BasicDataCodeShift, "_Edit");
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult EditGroup(ShiftGroupEditViewModel model)
        { 
            var originEntity = GetService<IShiftService>().Get(model.Id); 
            if (string.IsNullOrWhiteSpace(model.Code) || string.IsNullOrWhiteSpace(model.ShiftName))
            {
                return Json(JsonResultData.Failure("班次代码和班次名称不能为空"));
            }
            string strbegin = CheckDatetimeType(model.BeginTime == null ? "" : model.BeginTime);
            string strend = CheckDatetimeType(model.EndTime == null ? "" : model.EndTime);
            if (strbegin != "")
            {
                return Json(JsonResultData.Failure(strbegin));
            }
            else if (strend != "")
            {
                return Json(JsonResultData.Failure(strend));
            }
            if (model.Seqid == null)
            {
                model.Seqid = 0;
            }
            if (model.Code != originEntity.Code)
            {
                bool exsit = GetService<IShiftService>().IsExsitrResFolio(GetService<ICurrentInfo>().HotelId, originEntity.Id);
                if (GetService<ICurrentInfo>().ShiftId == originEntity.Id)
                {
                    return Json(JsonResultData.Failure("班次正在使用中不可修改班次代码"));
                }
                else if (exsit)
                {
                    return Json(JsonResultData.Failure("已使用班次代码不可修改"));
                }
            }
            return _EditGroup(model, originEntity, OpLogType.班次修改);
        }
        #endregion
         
        #region 启用禁用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            var _shiftService = GetService<IShiftService>();
            return _BatchBatchChangeStatusGroup(id, EntityKeyDataType.String, M_V_BasicDataType.BasicDataCodeShift, _shiftService, EntityStatus.启用, OpLogType.班次启用禁用);
              
             
        }
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            var _shiftService = GetService<IShiftService>();
            return _BatchBatchChangeStatusGroup(id, EntityKeyDataType.String, M_V_BasicDataType.BasicDataCodeShift, _shiftService, EntityStatus.禁用, OpLogType.班次启用禁用); 
        }

        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        { 
                return _BatchDeleteGroup(id, EntityKeyDataType.String, GetService<IShiftService>(), OpLogType.班次删除); 
        }
        #endregion
     
    }
}