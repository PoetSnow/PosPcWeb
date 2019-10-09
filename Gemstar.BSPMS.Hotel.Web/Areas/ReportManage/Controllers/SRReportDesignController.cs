using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Stimulsoft.Report.Mvc;
using Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.ReportManage;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Controllers
{
    [AuthPage("40010")]
    [AuthPage(ProductType.Pos, "p50010")]
    public class SRReportDesignController : SRReportBaseController
    {
        /// <summary>
        /// 获取报表数据
        /// </summary>
        /// <param name="reportCode">报表代码</param>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult Index(string reportCode, string returnUrl, string styleName)
        {
            if (string.IsNullOrWhiteSpace(reportCode))
            {
                ViewBag.Message = "请先选择要查看的报表";
                return View("Error");
            }
            if (!System.IO.File.Exists(GetReportFilePath(reportCode, styleName)))
            {
                ViewBag.Message = "指定的报表文件不存在，请选择其他报表";
                return View("Error");
            }
            ViewBag.ReportCode = reportCode;
            ViewBag.styleName = styleName;
            ViewBag.returnUrl = returnUrl;//SRBillReportView需要

            return View();
        }

        #region MvcViewer Actions
        /// <summary>
        /// 读取报表模板
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetReportTemplate(string reportCode, string styleName)
        {
            try
            {
                return StiMvcDesigner.GetReportTemplateResult(GetReport(new ReportQueryModel { ReportCode = reportCode, ChineseName = "单据", StyleName = (styleName == "" ? null : styleName) }));
            }
            catch (Exception ex)
            {
                return Content("Error:" + ex.Message);
            }
        }
        /// <summary>
        /// 保存报表模板
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Update)]
        public ActionResult SaveReportTemplate(string reportCode, string styleName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(styleName)) { styleName = null; }
                bool result = SaveReport(reportCode, StiMvcDesigner.GetReportObject(this.Request), styleName);
                if (result)
                {
                    return StiMvcDesigner.SaveReportResult(false);
                }
                else
                {
                    return StiMvcDesigner.SaveReportResult("保存失败！");
                }
            }
            catch (Exception ex)
            {
                return StiMvcDesigner.SaveReportResult(string.Format("保存失败，原因:{0}", ex.Message));
            }
        }
        /// <summary>
        /// 获取本地化
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult GetLocalization()
        {
            string name = StiMvcDesigner.GetLocalizationName(this.Request);
            string path = Server.MapPath("~/SRReports/Localizations/");
            return StiMvcDesigner.GetLocalizationResult(path + name);
        }
        /// <summary>
        /// 退出设计
        /// </summary>
        /// <returns></returns>
        [AuthButton(AuthFlag.Query)]
        public ActionResult ExitDesigner(string reportCode, string returnUrl, string styleName)
        {
            //获取自定义URL
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                var urlReferrer = HttpContext.Request.UrlReferrer;
                if (urlReferrer != null)
                {
                    System.Collections.Specialized.NameValueCollection list = CommonUrlHelper.GetQueryString(urlReferrer.Query);
                    if (list != null && list.Count > 0)
                    {
                        returnUrl = list.Get("returnUrl");
                    }
                }
            }
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = Server.UrlDecode(returnUrl);
                if (Url.IsLocalUrl(returnUrl))
                {
                    //重定向自定义URL
                    return Redirect("http://" + Request.Url.Host + returnUrl);
                }
            }
            //根据reportCode判断是否是报表
            bool isReportView = false;
            if (!string.IsNullOrWhiteSpace(reportCode))
            {
                int temp = 0;
                if (Int32.TryParse(reportCode, out temp))
                {
                    if (temp > 0)
                    {
                        isReportView = true;//是报表
                    }
                }
            }
            if (isReportView)
            {
                //返回报表
                return Redirect("http://" + Request.Url.Host + "/ReportManage/SRReportView/Index?ReportCode=" + reportCode + "&styleName=" + styleName);
            }
            else
            {
                //关闭页面
                return Content("<script type=\"text/javascript\">var userAgent = navigator.userAgent;if (userAgent.indexOf(\"Firefox\") != -1 || userAgent.indexOf(\"Presto\") != -1) {window.location.replace(\"about:blank\");} else {window.opener = null;window.open(\"\", \"_self\");window.close();}</script>");
            }
        }
        #endregion

    }
}