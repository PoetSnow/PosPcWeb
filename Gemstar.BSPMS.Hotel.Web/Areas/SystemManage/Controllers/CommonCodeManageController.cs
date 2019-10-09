using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.CommonCodeManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Extensions;
using System.Collections.ObjectModel;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.CodelistGroupManage;
using Gemstar.BSPMS.Common.Services.BasicDataControls;
using Gemstar.BSPMS.Hotel.Services.SystemManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 通用代码设置
    /// </summary>
    [AuthPage("99070")]
    [AuthPage(ProductType.Member, "m99035")]
    [AuthPage(ProductType.Pos, "p99035")]
    public class CommonCodeManageController : BaseEditInWindowController<CodeList, ICodeListService>
    {

        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        {
            string typeCode = "0";
            if (CurrentInfo.IsGroupInGroup)
            {
                SetCommonQueryValues("up_list_pmsCodeList_GrpDistrib", "@h99typeCode=" + typeCode + "&@s23分店=" + CurrentInfo.HotelId);
            }
            else
            {
                SetCommonQueryValues("up_list_pmsCodeList", "@h99typeCode=" + typeCode);
            }
            //是否启用打扫报表
            var paraService = GetService<IPmsParaService>();
            var isDirtyLog = paraService.GetValue(CurrentInfo.HotelId, "isDirtyLog");
            ViewBag.isDirtyLog = isDirtyLog;
            ViewBag.isgrouphotel = CurrentInfo.IsHotelInGroup;
            ViewBag.IsPermanentRoom = IsPermanentRoom();//是否启用长租管理功能
            if (CurrentInfo.IsGroupInGroup)
            {

                var pmsHotelService = GetService<IPmsHotelService>();
                ViewBag.Hotels = pmsHotelService.GetHotelsInGroup(CurrentInfo.GroupId);
                return View("IndexGroup");
            }
            else
            {
                return View();
            }
        }
        #endregion 
        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(string typeCode)
        {
            if (CurrentInfo.IsHotelInGroup)
            {
                var resortControl = GetService<IBasicDataResortControlService>().GetResortControl(typeCode, CurrentInfo.GroupId);
                if (resortControl != null && !resortControl.ResortCanAdd)
                {
                    return Content("集团分发型资料，集团设置为分店不可增加！");
                }
            }
            CodeType codeTypeEntity = GetService<ICodeListService>().GetCodeType(typeCode);
            if (codeTypeEntity == null)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            }
            ViewBag.Name2 = string.IsNullOrWhiteSpace(codeTypeEntity.lable12) ? null : codeTypeEntity.lable12;
            ViewBag.Name3 = string.IsNullOrWhiteSpace(codeTypeEntity.label3) ? null : codeTypeEntity.label3;
            ViewBag.Name4 = string.IsNullOrWhiteSpace(codeTypeEntity.label4) ? null : codeTypeEntity.label4;
            if (CurrentInfo.IsGroupInGroup && Array.IndexOf("04,05,08,13,23".Split(','), typeCode) > -1)
            {
                return _AddGroup(new CodeListGroupAddModel() { TypeCode = typeCode }, typeCode);
            }
            else
            {
                return _Add(new CodeListAddViewModel() { TypeCode = typeCode });
            }
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(CodeListAddViewModel codeListViewModel)
        {
            var _codeListService = GetService<ICodeListService>();
            CodeType codeTypeEntity = _codeListService.GetCodeType(codeListViewModel.TypeCode);
            if (codeTypeEntity == null)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            }
            if (_codeListService.IsExists(CurrentInfo.HotelId, codeTypeEntity.code, codeListViewModel.Code))
            {
                return Json(JsonResultData.Failure("代码已存在，请重新输入。"));
            }
            if (codeListViewModel.Code.Substring(0, 1) == "0")
            {
                return Json(JsonResultData.Failure("自定义代码前缀不能为0，<br/>前缀为0是系统固定代码！"));
            }
            var retvalue = _Add(codeListViewModel, new CodeList
            {
                Hid = CurrentInfo.HotelId,
                Id = CurrentInfo.HotelId + codeTypeEntity.code + codeListViewModel.Code,
                Code = codeListViewModel.Code,
                Name = codeListViewModel.Name,
                Name2 = codeListViewModel.Name2,
                Name3 = codeListViewModel.Name3,
                Name4 = codeListViewModel.Name4,
                Status = EntityStatus.启用,
                Seqid = codeListViewModel.Seqid,
                TypeCode = codeTypeEntity.code,
                TypeName = codeTypeEntity.name,
                DataSource = BasicDataDataSource.Added.Code
            }, OpLogType.通用代码增加);
            ////同步修改分店的市场分类和客人来源
            //if (CurrentInfo.IsGroup && !CurrentInfo.IsHotelInGroup && (codeListViewModel.TypeCode == "04" || codeListViewModel.TypeCode == "05"))
            //{
            //    _codeListService.updateGrpHotelCodelist(CurrentInfo.HotelId, codeListViewModel.TypeCode);
            //}
            return retvalue;
        }
        [HttpPost]
        [AuthButton(AuthFlag.Add)]
        [JsonException]
        public ActionResult AddGroup(CodeListGroupAddModel codeListViewModel)
        {
            var _codeListService = GetService<ICodeListService>();
            CodeType codeTypeEntity = _codeListService.GetCodeType(codeListViewModel.TypeCode);
            if (codeTypeEntity == null)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            }
            if (_codeListService.IsExists(CurrentInfo.HotelId, codeTypeEntity.code, codeListViewModel.Code))
            {
                return Json(JsonResultData.Failure("代码已存在，请重新输入。"));
            }
            if (codeListViewModel.Code.Substring(0, 1) == "0")
            {
                return Json(JsonResultData.Failure("自定义代码前缀不能为0，<br/>前缀为0是系统固定代码！"));
            }
            return _AddGroup(codeListViewModel, new CodeList
            {
                Hid = CurrentInfo.HotelId,
                Id = CurrentInfo.HotelId + codeTypeEntity.code + codeListViewModel.Code,
                Code = codeListViewModel.Code,
                Name = codeListViewModel.Name,
                Name2 = codeListViewModel.Name2,
                Name3 = codeListViewModel.Name3,
                Name4 = codeListViewModel.Name4,
                Status = EntityStatus.启用,
                Seqid = codeListViewModel.Seqid,
                TypeCode = codeTypeEntity.code,
                TypeName = codeTypeEntity.name,
                DataSource = BasicDataDataSource.Added.Code
            }, OpLogType.消费项目增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(int id)
        {
            var codeListService = GetService<ICodeListService>();
            var codeListEntity = codeListService.Get(id);
            CodeType codeTypeEntity = codeListService.GetCodeType(codeListEntity.TypeCode);
            //if (codeTypeEntity == null)
            //{
            //    return Json(JsonResultData.Failure("错误信息，请关闭后重试。"), JsonRequestBehavior.AllowGet);
            //}
            ViewBag.Name2 = string.IsNullOrWhiteSpace(codeTypeEntity.lable12) ? null : codeTypeEntity.lable12;
            ViewBag.Name3 = string.IsNullOrWhiteSpace(codeTypeEntity.label3) ? null : codeTypeEntity.label3;
            ViewBag.Name4 = string.IsNullOrWhiteSpace(codeTypeEntity.label4) ? null : codeTypeEntity.label4;
            if (CurrentInfo.IsGroup && Array.IndexOf("04,05,08,13,23".Split(','), codeListEntity.TypeCode) > -1)
            {
                var editResult = _CanEdit(codeListEntity, codeListEntity.TypeCode);
                if (editResult != null)
                {
                    return editResult;
                }
                if (CurrentInfo.IsGroupInGroup)
                {
                    return _EditGroup(id, new CodeListGroupEditModel(), codeListEntity.TypeCode);
                }
                else
                {
                    return _Edit(id, new CodeListEditViewModel());
                }
            }
            else
            {
                return _Edit(id, new CodeListEditViewModel());
            }
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(CodeListEditViewModel model)
        {
            var _codeListService = GetService<ICodeListService>();
            var entity = _codeListService.Get(model.Pk);
            model.Id = entity.Id;
            //if (entity == null || entity.Hid != CurrentInfo.HotelId)
            //{
            //    return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            //}
            if (model.Code != entity.Code && _codeListService.IsExists(CurrentInfo.HotelId, entity.TypeCode, model.Code))
            {
                return Json(JsonResultData.Failure("代码已存在，请重新输入。"));
            }
            if (model.Code != entity.Code)
            {
                //为防止其他模块匹配不到此条记录。所以不能修改code。
                model.Code = entity.Code;
            }
            if (entity.TypeCode == "13")//发票项目自动同步汇率到消费项目的汇率
            {
                var _itemService = GetService<IItemService>();
                _itemService.syncRate(CurrentInfo.HotelId, model.Id, decimal.Parse(model.Name2));
            }
            string log = "";
            if (entity.Name != model.Name)
            {
                log += "  名称：" + entity.Name + "=>" + model.Name;
            }
            if (entity.Name2 != model.Name2)
            {
                log += "  " + (entity.TypeCode == "04" ? "免费/自用标志：" : "名称2：") + entity.Name2 + "=>" + model.Name2;
            }
            if (entity.Seqid != model.Seqid)
            {
                log += "  序号：" + entity.Seqid + "=>" + model.Seqid;
            }
            if (log != "")
            {
                log = entity.TypeName + "：" + entity.Name + log;
                AddOperationLog(OpLogType.通用代码修改, log);
            }
            var retvalue = _Edit(model, entity, OpLogType.通用代码修改);
            //if (CurrentInfo.IsGroup && !CurrentInfo.IsHotelInGroup && (entity.TypeCode == "04" || entity.TypeCode == "05"))
            //{
            //    _codeListService.updateGrpHotelCodelist(CurrentInfo.HotelId, entity.TypeCode);
            //}
            return retvalue;
        }
        #endregion

        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult EditGroup(CodeListGroupEditModel model)
        {
            var _codeListService = GetService<ICodeListService>();
            var entity = _codeListService.Get(model.Pk);
            model.Id = entity.Id;

            if (model.Code != entity.Code && _codeListService.IsExists(CurrentInfo.HotelId, entity.TypeCode, model.Code))
            {
                return Json(JsonResultData.Failure("代码已存在，请重新输入。"));
            }
            if (model.Code != entity.Code)
            {
                //为防止其他模块匹配不到此条记录。所以不能修改code。
                model.Code = entity.Code;
            }
            if (entity.TypeCode == "13")//发票项目自动同步汇率到消费项目的汇率
            {
                var _itemService = GetService<IItemService>();
                _itemService.syncRate(CurrentInfo.HotelId, model.Id, decimal.Parse(model.Name2));
            }
            string log = "";
            if (entity.Name != model.Name)
            {
                log += "  名称：" + entity.Name + "=>" + model.Name;
            }
            if (entity.Name2 != model.Name2)
            {
                log += "  " + (entity.TypeCode == "04" ? "免费/自用标志：" : "名称2：") + entity.Name2 + "=>" + model.Name2;
            }
            if (entity.Seqid != model.Seqid)
            {
                log += "  序号：" + entity.Seqid + "=>" + model.Seqid;
            }
            if (log != "")
            {
                log = entity.TypeName + "：" + entity.Name + log;
                AddOperationLog(OpLogType.通用代码修改, log);
            }
            var retvalue = _EditGroup(model, entity, OpLogType.通用代码修改);
            // var retvalue = _Edit(model, entity, OpLogType.通用代码修改);
            //if (CurrentInfo.IsGroup && !CurrentInfo.IsHotelInGroup && (entity.TypeCode == "04" || entity.TypeCode == "05"))
            //{
            //    _codeListService.updateGrpHotelCodelist(CurrentInfo.HotelId, entity.TypeCode);
            //}
            return retvalue;
        }

        #region 启用禁用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            var _codeListService = GetService<ICodeListService>();
            var entity = _codeListService.Get(int.Parse(id.Split(',')[0]));
            if (CurrentInfo.IsGroup)
            {
                var reval = _BatchBatchChangeStatusGroup(id, EntityKeyDataType.Int, entity.TypeCode, _codeListService, EntityStatus.启用, OpLogType.房型启用禁用);
                return reval;
            }
            else
            {
                return Json(_codeListService.BatchUpdateStatus(id, EntityStatus.启用));
            }

        }
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            var _codeListService = GetService<ICodeListService>();
            var entity = _codeListService.Get(int.Parse(id.Split(',')[0]));
            if (CurrentInfo.IsGroup)
            {

                var reval = _BatchBatchChangeStatusGroup(id, EntityKeyDataType.Int, entity.TypeCode, _codeListService, EntityStatus.禁用, OpLogType.房型启用禁用);
                return reval;
            }
            else
            {
                return Json(_codeListService.BatchUpdateStatus(id, EntityStatus.禁用));
            }
        }

        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            if (CurrentInfo.IsGroup)
            {
                return _BatchDeleteGroup(id, EntityKeyDataType.Int, GetService<ICodeListService>(), OpLogType.通用代码删除);
            }
            else
            {
                return _BatchDelete(id, GetService<ICodeListService>(), OpLogType.通用代码删除);
            }
        }
        /// <summary>
        /// 判断数据是否存在其他表中
        /// </summary>
        /// <param name="id">item表中的id</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Delete)]
        public ActionResult CheckForDelete(string[] id)
        {
            var currentInfo = GetService<ICurrentInfo>();
            var hotelserver = GetService<ICodeListService>();
            string str = "";
            var arr = id[0].Split(',');
            for (int i = 0; i < arr.Length; i++)
            {
                CodeList cl = hotelserver.Get(int.Parse(arr[i]));
                str = hotelserver.checkIsExistOtherTable(currentInfo.HotelId, cl.Id);
                if (str != "")
                {
                    return Json(JsonResultData.Failure("该数据已存在【" + str.Trim('、') + "】中，不允许修改！"));
                }
            }
            return Json(JsonResultData.Successed("可以删除！"));
        }

        #endregion
        #region 下拉绑定
        [AuthButton(AuthFlag.None)]
        [NotAuth]
        public JsonResult GetRoomFeaturesSelectList()
        {
            var _roomTypeService = GetService<ICodeListService>();
            return Json(_roomTypeService.GetRoomFeatures(CurrentInfo.HotelId), JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.None)]
        [NotAuth]
        public JsonResult GetCustomerSourceSelectList()
        {
            var _roomTypeService = GetService<ICodeListService>();
            return Json(_roomTypeService.GetCustomerSource(CurrentInfo.HotelId), JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.None)]
        [NotAuth]
        public JsonResult GetMarketCategorySelectList()
        {
            var _roomTypeService = GetService<ICodeListService>();
            return Json(_roomTypeService.GetMarketCategory(CurrentInfo.HotelId), JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.None)]
        [NotAuth]
        public JsonResult GetMakeList()
        {
            Collection<SelectListItem> list = new Collection<SelectListItem>() {
                 new SelectListItem() { Value ="", Text = "", Selected = true },
                   new SelectListItem() { Value = "免费", Text = "免费", Selected = false },
                   new SelectListItem() { Value ="自用", Text = "自用", Selected = false }

            };

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}