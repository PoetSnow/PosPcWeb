using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stimulsoft.Report.Mvc;
using Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using Gemstar.BSPMS.Common.Services;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.Tools;
using System.Collections.Specialized;
using Stimulsoft.Report;
using System.IO;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Microsoft.Win32;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Controllers
{
    [AuthPage("40010")]
    [AuthPage(ProductType.Pos, "p50010")]
    public class SRBillReportViewController : SRReportBaseController
    {
        /// <summary>
        /// 获取报表数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// sType: 需要签名单据的类型，默认为0
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(ReportQueryModel model, string print, string sType = "0", string regid = null, string roomNo = null, string refeid = null)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.ReportCode))
            {
                ViewBag.Message = "请先选择要查看的报表";
                return View("Error");
            }
            if (!System.IO.File.Exists(GetReportFilePath(model.ReportCode, "")))
            {
                ViewBag.Message = "指定的报表文件不存在，请选择其他报表";
                return View("Error");
            }
            model = CheckParameterValues(model);
            if (model == null)
            {
                ViewBag.Message = "此地址已失效，请从系统中打开报表";
                return View("Error");
            }
            model.IsOpenSearchWindow = false;
            ViewBag.Title = model.ChineseName;
            ViewBag.ReportQueryModel = model;
            ViewBag.print = print;
            ViewBag.sType = sType;
            ViewBag.regid = regid;
            ViewBag.roomNo = roomNo;
            if (print == "10")
            {
                var signature = IsHasAuth("40010", 2251799813685248);
                ViewBag.signature = signature;
            }
            ViewBag.hid = CurrentInfo.HotelId;
            ViewBag.userName = CurrentInfo.UserName;
            var service = GetService<IReportService>();
            var styleNames = service.GetStyleNames(CurrentInfo.HotelId, model.ReportCode);
            var styleItems = new List<SelectListItem>();
            foreach (var name in styleNames)
            {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    styleItems.Add(new SelectListItem { Value = name, Text = name });
                }
                else
                {
                    styleItems.Add(new SelectListItem { Value = SrReport.DefaultBillUserFormatStyleName, Text = SrReport.DefaultBillUserFormatStyleName });
                }
            }
            //添加默认格式选项
            styleItems.Add(new SelectListItem { Value = SrReport.DefaultBillFormatStyleName, Text = SrReport.DefaultBillFormatStyleName });
            if (HasPosFile(model.ReportCode))
            {
                styleItems.Add(new SelectListItem { Value = SrReport.DefaultBillPosFormatStyleName, Text = SrReport.DefaultBillPosFormatStyleName });
            }
            if (model.ReportCode == "up_print_bill")
            {
                styleItems.Add(new SelectListItem { Value = SrReport.sumbill, Text = SrReport.sumbill });
                styleItems.Add(new SelectListItem { Value = SrReport.sumbillpos, Text = SrReport.sumbillpos });
            }
            if (model.ReportCode == "resRCBill")
            {
                styleItems.Add(new SelectListItem { Value = SrReport.resRelationRC, Text = SrReport.resRelationRC });
            }
            ViewBag.styleItems = styleItems;
            if (string.IsNullOrWhiteSpace(model.StyleName) && styleItems.Count > 0)
            {
                model.StyleName = styleItems[0].Value;

                //查询默认账单格式
                if (model.ReportCode.ToLower() == "posbillprint")
                {
                    var posRefe = Session["PosRefe"] as PosRefe;
                    if (posRefe == null)
                    {
                        //获取账单的id
                        var ParameterValuesList = model.ParameterValues.Split('@');
                        var billid = "0";
                        foreach (var billIdArr in ParameterValuesList)
                        {
                            //字符串分割得到账单ID
                            if (!string.IsNullOrWhiteSpace(billIdArr) && billIdArr.IndexOf("&") == -1)
                            {
                                billid = billIdArr.Split('=')[1];
                                break;
                            }
                        }
                        var BillService = GetService<IPosBillService>();
                        var billModel = BillService.Get(billid);

                        var refeService = GetService<IPosRefeService>();
                        posRefe = refeService.Get(billModel.Refeid);
                    }

                    if (!string.IsNullOrWhiteSpace(posRefe.PosBillPrint))
                    {
                        model.StyleName = posRefe.PosBillPrint;
                    }
                }
            }
            return View(model);
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns></returns>
        private ReportQueryModel GetQueryModel()
        {
            ReportQueryModel model = new ReportQueryModel();
            string formValues = HttpContext.Request.Form.Get("formValues");
            if (!string.IsNullOrWhiteSpace(formValues))
            {
                try
                {
                    formValues = Server.UrlDecode(formValues);
                    model = new JavaScriptSerializer().Deserialize<ReportQueryModel>(formValues);
                }
                catch { }
            }
            if (model == null || string.IsNullOrWhiteSpace(model.ReportCode))
            {
                var urlReferrer = HttpContext.Request.UrlReferrer;
                if (urlReferrer != null)
                {
                    NameValueCollection list = CommonUrlHelper.GetQueryString(urlReferrer.Query);
                    model.ReportCode = list.Get("ReportCode");
                    model.ChineseName = list.Get("ChineseName");
                }
            }
            if (string.IsNullOrWhiteSpace(model.StyleName))
            {
                var service = GetService<IReportService>();
                var styleNames = service.GetStyleNames(CurrentInfo.HotelId, model.ReportCode);
                if (styleNames.Count > 0)
                {
                    model.StyleName = styleNames[0];
                }
            }
            return model;
        }
        public string GetLogoUrl(string hid)
        {
            var hotelServices = GetService<IHotelInfoService>();
            return hotelServices.GetLogoUrl(hid);
        }
        public string GetHotelShortName(string hid)
        {
            var hotelServices = GetService<IHotelInfoService>();
            return hotelServices.GetHotelShortName(hid);
        }
        public CenterHotel GetHotelInfo(string hid)
        {
            var hotelServices = GetService<IHotelInfoService>();
            return hotelServices.Get(hid);
        }
        #region MvcViewer Actions
        /// <summary>
        /// 获取报表快照
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetReportSnapshot()
        {
            try
            {
                //每次更改查询条件后，都更改一次报表的guid值，以便各服务器可以正确的从redis缓存中取正确的值
                var model = GetQueryModel();
                Session[model.ReportCode] = "";
                var report = GetReport(model);
                return StiMvcViewer.GetReportSnapshotResult(HttpContext, report);
            }
            catch (Exception ex)
            {
                return Content("Error:" + ex.Message);
            }
        }
        /// <summary>
        /// 查看器事件
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult ViewerEvent()
        {
            var report = GetCachedReportObject(HttpContext);
            return StiMvcViewer.ViewerEventResult(HttpContext, report);
        }

        private StiReport GetCachedReportObject(HttpContextBase httpContext)
        {
            var report = StiMvcViewer.GetReportObject(HttpContext);
            if (report == null)
            {
                var model = GetQueryModel();
                report = GetReport(model);
            }
            return report;
        }

        /// <summary>
        /// 打印报告
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult PrintReport()
        {
            try
            {
                var report = GetCachedReportObject(HttpContext);
                return StiMvcViewer.PrintReportResult(HttpContext, report);
            }
            catch (Exception ex)
            {
                return Content("Error:" + ex.Message);
            }
        }
        /// <summary>
        /// 导出报告
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public FileResult ExportReport()
        {
            try
            {
                var report = GetCachedReportObject(HttpContext);
                return StiMvcViewer.ExportReportResult(HttpContext, report);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 交互
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult Interaction()
        {
            try
            {
                var report = GetCachedReportObject(HttpContext);
                return StiMvcViewer.InteractionResult(HttpContext, report);
            }
            catch (Exception ex)
            {
                return Content("Error:" + ex.Message);
            }
        }
        /// <summary>
        /// 设计
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult Design(ReportQueryModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ReportCode))
            {
                model = GetQueryModel();
            }
            return RedirectToAction("Index", "SRReportDesign", new { reportCode = model.ReportCode, styleName = model.StyleName });
        }
        /// <summary>
        /// 增加新格式名称
        /// </summary>
        /// <param name="reportCode">报表号</param>
        /// <param name="sourceStyle">源格式名称</param>
        /// <param name="styleName">新格式名称</param>
        /// <param name="chineseName">中文名称</param>
        /// <returns>增加是否成功</returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult AddStyle(string reportCode, string sourceStyle, string styleName, string chineseName)
        {
            if (string.IsNullOrWhiteSpace(reportCode))
            {
                return Json(JsonResultData.Failure("请指定要增加格式的报表号"));
            }
            if (string.IsNullOrWhiteSpace(styleName))
            {
                return Json(JsonResultData.Failure("请指定新格式名称"));
            }
            if (styleName == SrReport.DefaultBillFormatStyleName || styleName == SrReport.DefaultBillPosFormatStyleName || styleName == SrReport.DefaultBillUserFormatStyleName)
            {
                return Json(JsonResultData.Failure("新格式名称不能使用系统保留名称，请使用其他名称"));
            }
            try
            {
                var service = GetService<IReportService>();
                var exists = service.IsExistsTemplate(CurrentInfo.HotelId, reportCode, styleName);
                if (exists)
                {
                    return Json(JsonResultData.Failure("指定的新格式名称已经存在，请输入其他名称"));
                }
                var rpt = new SrReport(reportCode, GetReportFilePath(reportCode, sourceStyle), service, CurrentInfo, chineseName, sourceStyle);
                var success = rpt.SaveReport(rpt.GetReport("", false), styleName);
                if (success)
                {
                    return Json(JsonResultData.Successed(""));
                }
                else
                {
                    return Json(JsonResultData.Failure("保存失败，请稍后再试"));
                }
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(ex));
            }
        }
        /// <summary>
        /// 删除报表模板
        /// </summary>
        [AuthButton(AuthFlag.Delete)]
        public JsonResult DelReportTemplate(string reportCode, string styleName)
        {
            try
            {
                bool result = DelReport(reportCode, styleName);
                return Json(result ? JsonResultData.Successed() : JsonResultData.Failure("删除自定义格式失败！"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(string.Format("删除自定义格式失败，原因:{0}", ex.Message)), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #region 电子签名
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetPdfFileUrl(ReportQueryModel model)
        {
            var result = GetReport(model);
            MemoryStream stream = new MemoryStream();
            result.Render(false);
            var setting = new Stimulsoft.Report.Export.StiPdfExportSettings() { EmbeddedFonts = true };
            result.ExportDocument(StiExportFormat.Pdf, stream, setting);
            Web.Controllers.QiniuController qin = new Web.Controllers.QiniuController();
            string key = "pdf" + Guid.NewGuid();
            qin.UploadFile(stream, key);
            string url = "http://res.gshis.com/" + key;
            return Json(url);
        }
        #endregion
    }
}