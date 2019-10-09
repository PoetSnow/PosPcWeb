using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Web.Areas.CRM.Models.CompanyManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Extensions;
using System.Collections.ObjectModel;
using Kendo.Mvc.Extensions;
using System.Linq;
using Gemstar.BSPMS.Common.Services.Enums;

namespace Gemstar.BSPMS.Hotel.Web.Areas.CRM.Controllers
{
    /// <summary>
    /// 合约单位列表
    /// </summary>
    [AuthPage("60030002")]
    public class CompanyManageController : BaseEditInWindowController<Company, ICompanyService>
    {
        #region 查询
        [AuthButton(AuthFlag.None)]
        public ActionResult Index()
        {
            //检查权限
            var select = IsHasAuth("60030002", 1);//综合查询-此权限包含精确查询的
            var fastSelect = IsHasAuth("60030002", 4503599627370496);//精确查询
            var IsFast = select ? 1 : 0;
            ViewBag.IsFast = IsFast;
            SetCommonQueryValues("up_list_company", "@h99IsFast=" + IsFast);
            return View();
        }
        #endregion

        #region 增加
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add()
        {
            return _Add(new CompanyAddViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Add)]
        public ActionResult Add(CompanyAddViewModel companyViewModel)
        {
            if (string.IsNullOrWhiteSpace(companyViewModel.Sales))
                return Json(JsonResultData.Failure("请选择业务员"));
            var service= GetService<ICompanyService>();
            if (service.IsCompany(CurrentInfo.HotelId, companyViewModel.Code))
                return Json(JsonResultData.Failure("该合约单位代码已经存在"));
            if (service.IsCompanyName(CurrentInfo.HotelId, companyViewModel.Name))
                return Json(JsonResultData.Failure("该合约单位名称已经存在"));
            return _Add(companyViewModel, new Company
            {
                Grpid = CurrentInfo.GroupId,
                Id = Guid.NewGuid(),
                Hid = CurrentInfo.HotelId,
                Code = companyViewModel.Code,
                Name = companyViewModel.Name,
                CompanyTypeid = companyViewModel.CompanyTypeid,
                RateCode = companyViewModel.RateCode,
                Address = companyViewModel.Add,
                Bank = companyViewModel.Bank,
                BankAccount = companyViewModel.BankAccount,
                Contact = companyViewModel.Contact,
                ContactMobile = companyViewModel.ContactMobile,
                LimitAmount = companyViewModel.LimitAmount,
                Position = companyViewModel.Position,
                Sales = companyViewModel.Sales,
                TaxNo = companyViewModel.TaxNo,
                TaxType = companyViewModel.TaxType,
                Tel = companyViewModel.Tel,
                BeginDate = companyViewModel.BeginDate,
                ValidDate = companyViewModel.ValidDate,
                Status = EntityStatus.启用,
                TaxAddTel = companyViewModel.TaxAddTel,
                TaxName = companyViewModel.TaxName,
                TaxBankAccount = companyViewModel.TaxBankAccount,
                TaxEmail=companyViewModel.TaxEmail
            },OpLogType.合约单位增加);
        }
        #endregion

        #region 修改
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(string id)
        {
            return _Edit(Guid.Parse(id), new CompanyEditViewModel());
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Edit(CompanyEditViewModel model)
        {
            return _Edit(model, new Company(),OpLogType.合约单位修改);
        }
        #endregion

        #region 批量延期
        [AuthButton(AuthFlag.BatchDelay)]
        public ActionResult Delay(string id)
        {
            ViewBag.ids = id;
            return PartialView("_Delay");
        }
        [AuthButton(AuthFlag.BatchDelay)]
        public ActionResult Delays(string id, DateTime validdate)
        {
            var _mbrCardService = GetService<ICompanyService>();
            return Json(_mbrCardService.BatchDelayValidDate(id, validdate, OpLogType.合约单位延期, CurrentInfo));
        }
        #endregion
        
        #region  批量更改业务员
        [AuthButton(AuthFlag.ReplaceSalesman)]
        public ActionResult UpdateSales(string id)
        {
            ViewBag.ids = id;
            return PartialView("_UpdateSales");
        }
        [AuthButton(AuthFlag.ReplaceSalesman)]
        public ActionResult UpdateSalees(string id, string sales)
        {
            var _mbrCardService = GetService<ICompanyService>();
            return Json(_mbrCardService.BatchUpdateSales(id, sales, OpLogType.合约单位更换业务员,CurrentInfo));
        }
        #endregion

        #region 启用禁用
        [AuthButton(AuthFlag.Enable)]
        public ActionResult Enable(string id)
        {
            var _companyService = GetService<ICompanyService>();
            return Json(_companyService.BatchUpdateStatus(id, EntityStatus.启用, OpLogType.合约单位启用,CurrentInfo));
        }
        [AuthButton(AuthFlag.Disable)]
        public ActionResult Disable(string id)
        {
            var _companyService = GetService<ICompanyService>();
            return Json(_companyService.BatchUpdateStatus(id, EntityStatus.禁用, OpLogType.合约单位禁用,CurrentInfo));
        }

        #endregion

        #region 批量删除
        [AuthButton(AuthFlag.Delete)]
        public ActionResult BatchDelete(string id)
        {
            return _BatchDelete(id, GetService<ICompanyService>(), OpLogType.合约单位删除);
        }
        #endregion

        #region 销户
        [AuthButton(AuthFlag.Delete)]
        public ActionResult Cancellation(string id )
        {
            var _companyService = GetService<ICompanyService>();
            return Json(_companyService.BatchUpdateStatus(id, EntityStatus.销户, OpLogType.合约单位销户,CurrentInfo));
        }
        #endregion

        #region 下拉绑定
        [AuthButton(AuthFlag.Query)]
        public JsonResult GetTaxTypeSelectList()
        {
            return Json(TaxTypeList(), JsonRequestBehavior.AllowGet);
        }
        private Collection<SelectListItem> TaxTypeList()
        {
            return new Collection<SelectListItem>() {
                   new SelectListItem() { Value = "0", Text = "普通发票" },
                   new SelectListItem() { Value = "1", Text = "增值税专用发票" }
            };
        }
        [AuthButton(AuthFlag.Query)]
        public JsonResult GetCompanySelectList(string name)
        {
            var _companyService = GetService<ICompanyService>();
            return Json(_companyService.List(CurrentInfo.HotelId, name), JsonRequestBehavior.AllowGet);
        }
        [AuthButton(AuthFlag.Query)]
        public JsonResult GetCompanySelectListNotId(string name, Guid notId)
        {
            var _companyService = GetService<ICompanyService>();
            return Json(_companyService.List(CurrentInfo.HotelId, name, notId), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 详情
        [AuthButton(AuthFlag.Details)]
        public ActionResult Detail(string id)
        {
            var _companyService = GetService<ICompanyService>();
            Company entity = _companyService.Get(Guid.Parse(id));
            CompanyDeatilViewModel resultEntity = new CompanyDeatilViewModel();
            if (entity != null)
            {
                resultEntity.Address = entity.Address;
                resultEntity.Amount = entity.Amount;
                resultEntity.Balance = entity.Balance;
                resultEntity.Bank = entity.Bank;
                resultEntity.BankAccount = entity.BankAccount;
                resultEntity.BeginDateDetail = entity.BeginDate == null ? "" : entity.BeginDate.ToDateString();
                resultEntity.Code = entity.Code;
                resultEntity.Contact = entity.Contact;
                resultEntity.ContactMobile = entity.ContactMobile;
                resultEntity.Id = entity.Id;
                resultEntity.LimitAmount = entity.LimitAmount;
                resultEntity.Name = entity.Name;
                resultEntity.Nights = entity.Nights;
                resultEntity.Position = entity.Position;
                resultEntity.Sales = entity.Sales;
                resultEntity.TaxNo = entity.TaxNo;
                resultEntity.Tel = entity.Tel;
                resultEntity.ValidDate = entity.ValidDate == null ? "" : entity.ValidDate.ToDateString();
                resultEntity.TaxAddTel = entity.TaxAddTel;
                resultEntity.TaxBankAccount = entity.TaxBankAccount;
                resultEntity.TaxName = entity.TaxName;
                resultEntity.TaxEmail = entity.TaxEmail;

                resultEntity.CompanyTypeid = entity.CompanyTypeid;
                if (!string.IsNullOrWhiteSpace(entity.CompanyTypeid))
                {
                    //var companyTypeEntity = GetService<ICodeListService>().Get(entity.CompanyTypeid);
                    var companyTypeEntity = GetService<ICodeListService>().GetCodeListByID(entity.CompanyTypeid);
                    resultEntity.CompanyTypename = companyTypeEntity != null ? companyTypeEntity.Name : "";
                }

                resultEntity.RateCode = entity.RateCode;
                if (!string.IsNullOrWhiteSpace(entity.RateCode))
                {
                    var rateCodeEntity = GetService<IRateService>().Get(entity.RateCode);
                    resultEntity.RateCodeName = rateCodeEntity != null ? rateCodeEntity.Name : "";
                }

                resultEntity.TaxType = entity.TaxType;
                if (entity.TaxType != null)
                {
                    var taxTypeEntity = TaxTypeList().Where(c => c.Value == entity.TaxType.ToString()).FirstOrDefault();
                    resultEntity.TaxTypeName = taxTypeEntity != null ? taxTypeEntity.Text : "";
                }

                resultEntity.Status = (byte)entity.Status;
                resultEntity.StatusName = entity.Status.ToString();
            }
            return PartialView("_Detail", resultEntity);
        }
        [HttpPost]
        [JsonException]
        [AuthButton(AuthFlag.Update)]
        public ActionResult Detail(CompanyDeatilViewModel companyViewModel)
        {
            if (companyViewModel.DelayValidDate != null)
            {
                var _companyService = GetService<ICompanyService>();
                return Json(_companyService.DelayValidDate(companyViewModel.Id, (DateTime)companyViewModel.DelayValidDate, OpLogType.合约单位延期));
            }
            return Json(JsonResultData.Failure("请选择延期日期"));
        }
        #endregion

        #region 发送合约单位营销短信
        /// <summary>
        /// 发送营销短信
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.AddCardAuth)]
        public ActionResult MarketSms(string para)
        {
            ViewBag.Para = para;
            return View("_MarketSms");
        }
        /// <summary>
        /// 发送营销短信
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="mobiles"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.AddCardAuth)]
        public JsonResult SendSms(string ids, string mobiles, string content)
        {
            var _companyService = GetService<ICompanyService>();
            var entity = _companyService.SendMarketSms(CurrentInfo.HotelId,ids, mobiles, content);
            if (entity.Success)
            {
                var count = ids.Split(',').Length;
                string opLog = "发送合约单位数：" + count + "，营销短信内容：" + content;
                AddOperationLog(OpLogType.合约单位营销短信, opLog);
            }
            return Json(entity);
        }
        #endregion
    }
}