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

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Controllers
{
    /// <summary>
    /// 合约单位类别
    /// </summary>
    [AuthPage("60030001")]
    [AuthBasicData("corpType")]
    public class CompanyTypeManageController : BaseEditInWindowController<CodeList, ICodeListService>
    {
        private string typeCode = "11";

        #region 查询
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index()
        { 
            SetCommonQueryValues("up_list_pmsCodeList_group", "@h99grpid=" + CurrentInfo.GroupHotelId+"&@h99typeCode =" + typeCode);  
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            CodeType codeTypeEntity = GetService<ICodeListService>().GetCodeType(typeCode);
            if (codeTypeEntity == null)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            }
            ViewBag.Name2 = string.IsNullOrWhiteSpace(codeTypeEntity.lable12) ? null : codeTypeEntity.lable12;
            ViewBag.Name3 = string.IsNullOrWhiteSpace(codeTypeEntity.label3) ? null : codeTypeEntity.label3;
            ViewBag.Name4 = string.IsNullOrWhiteSpace(codeTypeEntity.label4) ? null : codeTypeEntity.label4;

            return _Add(new CodeListAddViewModel() { TypeCode = typeCode });
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(CodeListAddViewModel codeListViewModel)
        {
            if (codeListViewModel == null || codeListViewModel.TypeCode != typeCode)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            }
            var _codeListService = GetService<ICodeListService>();
            CodeType codeTypeEntity = _codeListService.GetCodeType(typeCode);
            if (codeTypeEntity == null)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            }
            if (_codeListService.IsExists(CurrentInfo.HotelId, codeTypeEntity.code, codeListViewModel.Code))
            {
                return Json(JsonResultData.Failure("代码已存在，请重新输入。"));
            }
            return _Add(codeListViewModel, new CodeList
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
                TypeName = codeTypeEntity.name
            }, OpLogType.合约单位类型增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(int id)
        {
            var codeListService = GetService<ICodeListService>();
            var codeListEntity = codeListService.Get(id);
            if (codeListEntity == null || codeListEntity.Hid != CurrentInfo.HotelId)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            }
            CodeType codeTypeEntity = codeListService.GetCodeType(typeCode);
            if (codeTypeEntity == null)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            }
            ViewBag.Name2 = string.IsNullOrWhiteSpace(codeTypeEntity.lable12) ? null : codeTypeEntity.lable12;
            ViewBag.Name3 = string.IsNullOrWhiteSpace(codeTypeEntity.label3) ? null : codeTypeEntity.label3;
            ViewBag.Name4 = string.IsNullOrWhiteSpace(codeTypeEntity.label4) ? null : codeTypeEntity.label4;

            return _Edit(id, new CodeListEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(CodeListEditViewModel model)
        {
            if (model == null)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            }
            var _codeListService = GetService<ICodeListService>();
            var codeListEntity = _codeListService.Get(model.Id);
            if (codeListEntity == null || codeListEntity.Hid != CurrentInfo.HotelId)
            {
                return Json(JsonResultData.Failure("错误信息，请关闭后重试。"));
            }
            if (model.Code != codeListEntity.Code)
            {
                if (_codeListService.IsExists(CurrentInfo.HotelId, typeCode, model.Code))
                {
                    return Json(JsonResultData.Failure("合约单位类型代码已存在，请重新输入。"));
                }
                if (GetService<ICompanyService>().IsExistsCompanyType(CurrentInfo.HotelId, codeListEntity.Id))
                {
                    return Json(JsonResultData.Failure("此类型在合约单位中已使用，不能修改。"));
                }
            }
            return _Edit(model, codeListEntity, OpLogType.合约单位类型修改);
        }
        #endregion

        #region 启用禁用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            var _codeListService = GetService<ICodeListService>();
            return Json(_codeListService.BatchUpdateStatus(id, EntityStatus.启用));
        }
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            var _codeListService = GetService<ICodeListService>();
            return Json(_codeListService.BatchUpdateStatus(id, EntityStatus.禁用));
        }
        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<ICodeListService>(), OpLogType.合约单位类别删除);
        }
        #endregion
        #region 下列绑定
        [AuthButton(AuthFlag.Query)]
        public JsonResult GetCompanyTypeSelectList()
        {
            var _codeListService = GetService<ICodeListService>();
            return Json(_codeListService.GetCompanyType(CurrentInfo.GroupHotelId), JsonRequestBehavior.AllowGet);
        }
        #endregion

    }
}