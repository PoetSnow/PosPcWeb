using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using Gemstar.BSPMS.Hotel.Services.ResInvoiceManage;
using Gemstar.BSPMS.Hotel.Web.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.SystemManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Controllers
{
    /// <summary>
    /// 发票
    /// </summary>
    [AuthPage("20020")]
    public class ResOrderInvoiceController : BaseController
    {
        /// <summary>
        /// 获取发票信息
        /// </summary>
        /// <param name="type">发票的关联类型。 0:和订单号相关  1:和会员账务相关  2:和合约单位账务相关</param>
        /// <param name="id">reftype=0订单Id，reftype=1会员账务Id，reftype=2合约单位账务Id</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(int type, string id)
        {
            if (!ParameterValidation(type, id))
            {
                return Content("请指定参数");
            }
            JsonResultData invoiceSourceInfo = GetService<IResInvoiceService>().GetInvoiceSourceInfo(CurrentInfo.HotelId, (byte)type, id);//获取 发票来源的 发票信息
            if (invoiceSourceInfo == null || invoiceSourceInfo.Success == false)
            {
                return Content(invoiceSourceInfo.Data.ToString());
            }
            var entity = invoiceSourceInfo.Data as ResInvoiceSimple;
            if (entity != null)
            {
                ViewBag.TaxType = entity.TaxType;
                ViewBag.TaxNo = entity.TaxNo;
                ViewBag.TaxName = entity.TaxName;
                ViewBag.TaxAddTel = entity.TaxAddTel;
                ViewBag.TaxBankAccount = entity.TaxBankAccount;
                ViewBag.PlanAmount = entity.PlanAmount;
            }
            ViewBag.Type = type;
            ViewBag.Id = id;
            return View();
        }

        #region 执行数据操作
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="type">发票的关联类型。 0:和订单号相关  1:和会员账务相关  2:和合约单位账务相关</param>
        /// <param name="id">reftype=0订单Id，reftype=1会员账务Id，reftype=2合约单位账务Id</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        [KendoGridDatasourceException]
        public ActionResult AjaxQueryInvoices(int type, string id, [DataSourceRequest]DataSourceRequest request)
        {
            if (!ParameterValidation(type, id))
            {
                return Json(new DataSourceResult { Errors = "请指定参数" });
            }
            var result = new ResInvoiceMainInfo() { InvoiceInfos = new System.Collections.Generic.List<ResInvoiceInfo>() };
            var resInvoiceService = GetService<IResInvoiceService>();
            switch (type)
            {
                case 0:
                    result = resInvoiceService.GetResInvoiceMainInfoByResId(CurrentInfo.HotelId, id);
                    break;
                case 1:
                    result = resInvoiceService.GetResInvoiceMainInfoByProfileCaId(CurrentInfo.HotelId, Guid.Parse(id));
                    break;
                case 2:
                    result = resInvoiceService.GetResInvoiceMainInfoByCompanyCaId(CurrentInfo.HotelId, Guid.Parse(id));
                    break;
            }
            return Json(result.InvoiceInfos.ToDataSourceResult(request));
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键ID</param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Update)]
        public ActionResult DestroyInvoices(Guid id)
        {
            try
            {
                return Json(GetService<IResInvoiceService>().DelResInvoice(CurrentInfo.HotelId, id), JsonRequestBehavior.DenyGet);
            }
            catch(Exception ex)
            {
                return Json(ex.ToString(), JsonRequestBehavior.DenyGet);
            }
            
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="resInvoiceInfo">发票信息</param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Update)]
        public ActionResult SaveInvoices(ResInvoiceInfo resInvoiceInfo)
        {
            try
            {
                var businessDate = GetService<IHotelStatusService>().GetBusinessDate(CurrentInfo.HotelId);
                JsonResultData result = GetService<IResInvoiceService>().AddOrUpdateResInvoice(resInvoiceInfo, CurrentInfo, businessDate);
                return Json(result, JsonRequestBehavior.DenyGet);
            }
            catch (Exception ex)
            {
                return Json(ex.ToString(), JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 获取开票信息
        /// </summary>
        /// <param name="taxType">0:普通发票  1：增值税专用发票</param>
        /// <param name="taxName">模糊查询字符串</param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetInvoicePartInfo(byte taxType, string taxName)
        {
            var resultList = GetService<IResInvoiceService>().GetInvoicePartInfo(CurrentInfo.HotelId, taxType, taxName);
            return Json(resultList, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 获取订单中指定合约单位的发票信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="resid">订单ID</param>
        /// <returns></returns>
        [HttpPost]
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetCompanyInvoiceInfo(string hid, string resid)
        {
            var resultList = GetService<IResInvoiceService>().GetCompanyInvoiceInfo(CurrentInfo.HotelId, resid);
            return Json((resultList != null ? JsonResultData.Successed(resultList) : JsonResultData.Failure(resultList)), JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region 其他
        /// <summary>
        /// 参数验证
        /// </summary>
        /// <param name="type">发票的关联类型。 0:和订单号相关  1:和会员账务相关  2:和合约单位账务相关</param>
        /// <param name="id">reftype=0订单Id，reftype=1会员账务Id，reftype=2合约单位账务Id</param>
        /// <returns></returns>
        private bool ParameterValidation(int type, string id)
        {
            if ((type != 0 && type != 1 && type != 2) || string.IsNullOrWhiteSpace(id))
            {
                return false;
            }
            else
            {
                if (type == 1 || type == 2)
                {
                    Guid tempid = new Guid();
                    if (!Guid.TryParse(id, out tempid))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 生成发票明细消息
        /// </summary>
        /// <param name="type">发票的关联类型。 0:和订单号相关  1:和会员账务相关  2:和合约单位账务相关</param>
        /// <param name="id">reftype=0订单Id，reftype=1会员账务Id，reftype=2合约单位账务Id</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult GenerateInvoiceDetails(int type, string id)
        {
            if (!ParameterValidation(type, id))
            {
                return Json(JsonResultData.Failure("请指定参数"), JsonRequestBehavior.DenyGet);
            }
            var list  = GetService<IResInvoiceService>().GenerateInvoiceDetails(CurrentInfo.HotelId, (byte)type, id);
            return Json(JsonResultData.Successed(list), JsonRequestBehavior.DenyGet);
        }
        #endregion


    }
}