using System;
using System.Web;
using System.Web.Mvc;
using Stimulsoft.Report.Mvc;
using Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using Gemstar.BSPMS.Common.Services;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.Tools;
using Stimulsoft.Report;
using System.Collections.Generic;
using Gemstar.BSPMS.Hotel.Services.PosManage;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Controllers
{
    [AuthPage("40010")]
    [AuthPage(ProductType.Pos, "p50010")]
    public class SRReportViewController : SRReportBaseController
    {
        /// <summary>
        /// 获取报表数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(ReportQueryModel model)
        {
            if (!GetService<IReportService>().IsReportauth(CurrentInfo.UserId, CurrentInfo.HotelId, model.ReportCode))
            {
                return Content("你没有查看该报表的权限！");
            }
            if (model == null || string.IsNullOrWhiteSpace(model.ReportCode))
            {
                ViewBag.Message = "请先选择要查看的报表";
                return View("Error");
            }
            string reportName = GetService<IReportService>().GetReportName(model.ReportCode, (byte)CurrentInfo.ProductType);
            if (string.IsNullOrWhiteSpace(reportName ?? model.ReportCode))
            {
                ViewBag.Message = "指定的报表不存在";
                return View("Error");
            }
            if (!System.IO.File.Exists(GetReportFilePath(model.ReportCode, model.StyleName)))
            {
                ViewBag.Message = "指定的报表文件不存在，请选择其他报表";
                return View("Error");
            }
            try
            {
                model = CheckParameterValues(model);
                if (model == null)
                {
                    ViewBag.Message = "此地址已失效，请从系统中打开报表";
                    return View("Error");
                }
                var rpt = new SrReport(model.ReportCode, GetReportFilePath(model.ReportCode, model.StyleName), GetService<IReportService>(), CurrentInfo);
                model.ReportCode = model.ReportCode;
                model.ProcedureName = rpt.GetProcedureName();
                if (string.IsNullOrWhiteSpace(model.ParameterValues))
                {
                    model.ParameterValues = SetParameterValues(model.ProcedureName);
                }

                if (HttpContext.Request.RequestType == "POST")
                {
                    model.IsOpenSearchWindow = false;
                }
                else
                {
                    if (model.IsOpenSearchWindow == false && !string.IsNullOrWhiteSpace(model.ParameterValues))
                    {
                        model.IsOpenSearchWindow = false;
                    }
                    else
                    {
                        model.IsOpenSearchWindow = true;
                    }
                }
                ViewBag.Title = reportName ?? model.ChineseName;
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
                if (model.ReportCode == "pos208")   //部门销售报表
                {
                    styleItems.Add(new SelectListItem { Value = SrReport.billRefeDept, Text = SrReport.billRefeDept });
                    styleItems.Add(new SelectListItem { Value = SrReport.billDept, Text = SrReport.billDept });
                }
                if (model.ReportCode == "pos209")   //折扣控制报表
                {
                    styleItems.Add(new SelectListItem { Value = SrReport.billDsicountUsers, Text = SrReport.billDsicountUsers });
                    styleItems.Add(new SelectListItem { Value = SrReport.billDsicountItem, Text = SrReport.billDsicountItem });
                }
                if (model.ReportCode == "pos607")   //物品汇总
                {
                    styleItems.Add(new SelectListItem { Value = SrReport.WareHouseItemSummary, Text = SrReport.WareHouseItemSummary });
                   // styleItems.Add(new SelectListItem { Value = SrReport.billDsicountItem, Text = SrReport.billDsicountItem });
                }
                //if (model.ReportCode == "201")
                //{
                //    styleItems.Add(new SelectListItem { Value = SrReport.SumPosDetaile, Text = SrReport.SumPosDetaile });
                //    styleItems.Add(new SelectListItem { Value = SrReport.billDetail, Text = SrReport.billDetail });
                //}
                //else
                //{
                //    styleItems.Add(new SelectListItem { Value = SrReport.DefaultBillFormatStyleName, Text = SrReport.DefaultBillFormatStyleName });

                //}

                ViewBag.styleItems = styleItems;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View("Error");
            }
            return View(model);
        }

        /// <summary>
        /// 设置存储过程参数值
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <returns></returns>
        private string SetParameterValues(string procedureName)
        {
            var parameterValues = "";
            var parameters = GetService<IReportService>().GetProcedureParameters(procedureName);

            if (parameters != null && parameters.Count > 0)
            {
                DateTime today = DateTime.Now;

                foreach (var temp in parameters)
                {


                    #region 设置默认日期
                    //当前日期
                    if (temp.ParameterName.ToString().ToLower().IndexOf("p0101") == 1)
                    {
                        if (temp.DataType.ToString().ToLower() == "date")
                        {
                            parameterValues += "&" + temp.ParameterName.ToString() + "=" + today.ToString("yyyy-MM-dd") + "^" + today.ToString("yyyy-MM-dd");
                        }
                        else if (temp.DataType.ToString().ToLower() == "datetime")
                        {
                            parameterValues += "&" + temp.ParameterName.ToString() + "=" + today + "^" + today;
                        }
                    }
                    //当前日期的前一天
                    if (temp.ParameterName.ToString().ToLower().IndexOf("p0102") == 1)
                    {
                        if (temp.DataType.ToString().ToLower() == "date")
                        {
                            parameterValues += "&" + temp.ParameterName.ToString() + "=" + today.AddDays(-1).ToString("yyyy-MM-dd") + "^" + today.AddDays(-1).ToString("yyyy-MM-dd");
                        }
                        else if (temp.DataType.ToString().ToLower() == "datetime")
                        {
                            parameterValues += "&" + temp.ParameterName.ToString() + "=" + today.AddDays(-1) + "^" + today.AddDays(-1);
                        }
                    }
                    //当月的第一天
                    if (temp.ParameterName.ToString().ToLower().IndexOf("p0103") == 1)
                    {
                        if (temp.DataType.ToString().ToLower() == "date")
                        {
                            parameterValues += "&" + temp.ParameterName.ToString() + "=" + today.ToString("yyyy-01-01") + "^" + today.ToString("yyyy-01-01");
                        }
                        else if (temp.DataType.ToString().ToLower() == "datetime")
                        {
                            parameterValues += "&" + temp.ParameterName.ToString() + "=" + today + "^" + today;
                        }
                    }
                    #endregion

                    #region 选择当前收银点
                    if (temp.ParameterName.ToString().ToLower().IndexOf("m0101") == 1)
                    {
                        if (temp.DataType.ToString().ToLower() == "varchar")
                        {
                            parameterValues += "&" + temp.ParameterName.ToString() + "=" + CurrentInfo.PosId.Trim() + "^" + CurrentInfo.PosName.Trim();
                        }
                    }
                    #endregion

                    #region 选择当前操作员
                    if (temp.ParameterName.ToString().ToLower().IndexOf("m32") == 1)
                    {
                        if (temp.DataType.ToString().ToLower() == "varchar")
                        {
                            parameterValues += "&" + temp.ParameterName.ToString() + "=" + CurrentInfo.UserName.Trim() + "^" + CurrentInfo.UserName.Trim();
                        }
                    }
                    #endregion

                    #region 选择当前市别
                    if (temp.ParameterName.ToString().ToLower().IndexOf("m03") == 1)
                    {
                        if (temp.DataType.ToString().ToLower() == "varchar")
                        {
                            var posService = GetService<IPosPosService>();
                            var posModel = posService.GetShiftChange(CurrentInfo.HotelId, CurrentInfo.PosId);
                            if (posModel != null)
                            {
                                parameterValues += "&" + temp.ParameterName.ToString() + "=" + posModel.Shiftid.Trim() + "^" + posModel.ShiftName.Trim();


                            }
                        }
                    }
                    #endregion

                    //下拉列表默认值
                    string codeValue;
                    CommonQueryParameterHelper.GetCodeValue(temp.ParameterName, out codeValue);
                    if (!string.IsNullOrEmpty(temp.DefaulValue) && (codeValue.StartsWith("d") || codeValue.StartsWith("e") || codeValue.StartsWith("m") || codeValue.StartsWith("n") || codeValue.StartsWith("k")))
                    {

                        parameterValues += "&" + temp.ParameterName.ToString() + "=" + temp.DefaulValue + "^" + temp.ParameterName.Split('_')[1];
                    }
                }
            }
            return parameterValues.Trim('&');
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
                catch
                {
                }
            }
            if (model == null || string.IsNullOrWhiteSpace(model.ReportCode))
            {
                var urlReferrer = HttpContext.Request.UrlReferrer;
                if (urlReferrer != null)
                {
                    model.ReportCode = CommonUrlHelper.GetQueryString(urlReferrer.Query).Get("ReportCode");
                }
            }
            if (string.IsNullOrWhiteSpace(model.StyleName)) { model.StyleName = null; }
            return model;
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
        /// 查看器事件
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult ViewerEvent()
        {
            var report = GetCachedReportObject(HttpContext);
            return StiMvcViewer.ViewerEventResult(HttpContext, report);
        }
        /// <summary>
        /// 打印报告
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult PrintReport()
        {
            //try
            //{
                var report = GetCachedReportObject(HttpContext);
                return StiMvcViewer.PrintReportResult(HttpContext, report);
            //}
            //catch (Exception ex)
            //{
            //    return Content("Error:" + ex.Message);
            //}
        }
        /// <summary>
        /// 导出报告
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public FileResult ExportReport()
        {
            //try
            //{
                var report = GetCachedReportObject(HttpContext);
                return StiMvcViewer.ExportReportResult(HttpContext, report);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
        /// <summary>
        /// 交互
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        //public ActionResult Interaction()
        //{
        //    //try
        //    //{
        //        var report = GetCachedReportObject(HttpContext);
        //        return StiMvcViewer.InteractionResult(HttpContext, report);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    return Content("Error:" + ex.Message);
        //    //}
        //}

        public ActionResult Interaction()
        {
            try
            {
                var report = GetCachedReportObject(HttpContext);
                if (!string.IsNullOrWhiteSpace(Request.Form["dataBandName"]))
                {
                    var updateComponent = (Stimulsoft.Report.Components.StiDataBand)report.GetComponentByName(Request.Form["dataBandName"]);
                    var groupComponent = updateComponent.Components;
                    string[] sort = new string[2];
                    sort[0] = Request.Form["order"];
                    sort[1] = Request.Form["orderColumn"];
                    updateComponent.Sort = sort;
                    updateComponent.NewGuid();
                    //if (HttpContext.Request.Form["order"] == "ASC")
                    //{
                    //    updateComponent.Sort = new string[0];
                    //}
                }
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
        public ActionResult Design()
        {
            return RedirectToAction("Index", "SRReportDesign", new { reportCode = GetQueryModel().ReportCode });
        }
        /// <summary>
        /// 删除报表模板
        /// </summary>
        [AuthButton(AuthFlag.Delete)]
        public JsonResult DelReportTemplate(string reportCode, string styleName)
        {
            try
            {
                bool result = DelReport(GetQueryModel().ReportCode, styleName);
                return Json(result ? JsonResultData.Successed() : JsonResultData.Failure("删除自定义格式失败！"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(JsonResultData.Failure(string.Format("删除自定义格式失败，原因:{0}", ex.Message)), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 增加新格式
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
        #endregion
    }
}