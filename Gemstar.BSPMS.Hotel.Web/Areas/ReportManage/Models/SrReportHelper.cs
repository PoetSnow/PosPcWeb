using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using Stimulsoft.Report;
using Stimulsoft.Report.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models
{
    public static class SrReportHelper
    {
        /// <summary>
        /// 在页面上展示报表
        /// </summary>
        /// <param name="htmlHelper">支持在视图中呈现 HTML 控件。</param>
        /// <param name="model">查询参数实体</param>
        /// <returns></returns>
        public static MvcHtmlString GetStiMvcViewer(this HtmlHelper htmlHelper, ReportQueryModel model)
        {
            List<KeyValuePair<string, string>> formValues = new List<KeyValuePair<string, string>>();
            if (model != null)
            {
                formValues.Add(new KeyValuePair<string, string>("ChineseName", model.ChineseName));
                formValues.Add(new KeyValuePair<string, string>("IsOpenSearchWindow", model.IsOpenSearchWindow.ToString()));
                formValues.Add(new KeyValuePair<string, string>("ParameterValues", model.ParameterValues));
                formValues.Add(new KeyValuePair<string, string>("ProcedureName", model.ProcedureName));
                formValues.Add(new KeyValuePair<string, string>("ReportCode", model.ReportCode));
                formValues.Add(new KeyValuePair<string, string>("StyleName", model.StyleName));
            }
            return htmlHelper.GetStiMvcViewer(formValues);
        }

        /// <summary>
        /// 在页面上展示报表
        /// </summary>
        /// <param name="htmlHelper">支持在视图中呈现 HTML 控件。</param>
        /// <param name="formValues">报表自动生成的js代码中有formValues对象。在formValues中添加属性。</param>
        /// <returns></returns>
        public static MvcHtmlString GetStiMvcViewer(this HtmlHelper htmlHelper, List<KeyValuePair<string, string>> formValues = null)
        {
            var stiMvcViewerHtml = htmlHelper.Stimulsoft().StiMvcViewer(new StiMvcViewerOptions
            {
                Theme = StiTheme.Windows7,
                ActionGetReportSnapshot = "GetReportSnapshot",
                ActionViewerEvent = "ViewerEvent",
                ActionPrintReport = "PrintReport",
                ActionExportReport = "ExportReport",
                ActionInteraction = "Interaction",
                DateFormat = "yyyy-MM-dd",
                Localization = "~/SRReports/Localizations/zh-CHS.xml",
                ShowButtonDesign = true,
                ActionDesignReport = "Design",
                ClientRequestTimeout = 120,
                ShowButtonPrint = false,
                ServerCacheMode = StiCacheMode.Page
            }).ToHtmlString();
            stiMvcViewerHtml = RemoveUrlPort(stiMvcViewerHtml);
            if (formValues != null && formValues.Count > 0)
            {
                stiMvcViewerHtml = AddFormValues(stiMvcViewerHtml, formValues);
            }
            return MvcHtmlString.Create(stiMvcViewerHtml);
        }
        /// <summary>
        /// 移除URL中的端口号
        /// </summary>
        /// <param name="stiMvcViewerHtml">HTML字符串</param>
        /// <remarks>报表自动生成的js代码url中有端口号，但是地址栏url是不用端口号的。为了和地址栏统一URL,所以此方法去除js代码中的端口号。</remarks>
        /// <returns></returns>
        private static string RemoveUrlPort(string stiMvcViewerHtml)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(stiMvcViewerHtml))
                {
                    var start = stiMvcViewerHtml.IndexOf("\"requestUrl\":\"") + 14;
                    if (start > 14)
                    {
                        var end = stiMvcViewerHtml.IndexOf("{action}\"", start);
                        if (end > start)
                        {
                            var url = stiMvcViewerHtml.Substring(start, end - start);
                            if (!string.IsNullOrWhiteSpace(url))
                            {
                                System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(@"^(?<proto>\w+)://[^/]+?(?<port>:\d+)?/", System.Text.RegularExpressions.RegexOptions.None, TimeSpan.FromMilliseconds(150));
                                System.Text.RegularExpressions.Match m = r.Match(url);
                                if (m.Success)
                                {
                                    var port = r.Match(url).Result("${port}");
                                    if (!string.IsNullOrWhiteSpace(port))
                                    {
                                        var newUrl = url.Replace(port, "");
                                        if (!string.IsNullOrWhiteSpace(newUrl))
                                        {
                                            stiMvcViewerHtml = stiMvcViewerHtml.Replace(url, newUrl);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
            return stiMvcViewerHtml;
        }
        /// <summary>
        /// 添加表单值
        /// </summary>
        /// <param name="stiMvcViewerHtml">HTML字符串</param>
        /// <param name="formValues">表单值列表</param>
        /// <remarks>报表自动生成的js代码中有formValues对象。在formValues中添加属性。</remarks>
        /// <returns></returns>
        private static string AddFormValues(string stiMvcViewerHtml, List<KeyValuePair<string, string>> formValues)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(stiMvcViewerHtml) && formValues != null && formValues.Count > 0)
                {
                    formValues = formValues.Where(c => !string.IsNullOrWhiteSpace(c.Key)).ToList();
                    if (formValues != null && formValues.Count > 0)
                    {
                        string formValuesStartsWith = "\"formValues\":\"%7b";
                        string formValuesNull = "\"formValues\":\"%7b%7d\",";
                        if (stiMvcViewerHtml.Contains(formValuesStartsWith))//formValues对象存在
                        {
                            //组合字符串
                            System.Text.StringBuilder formValuesStr = new System.Text.StringBuilder();
                            foreach (var item in formValues)
                            {
                                formValuesStr.AppendFormat("%22{0}%22%3a%22{1}%22%2c", item.Key, HttpUtility.UrlEncode(item.Value));//%22转义双引号，%3a转义冒号，%2c转义逗号
                            }
                            if (stiMvcViewerHtml.Contains(formValuesNull))//formValues对象存在并且内容为空
                            {
                                stiMvcViewerHtml = stiMvcViewerHtml.Replace(formValuesNull, string.Format("\"formValues\":\"%7b{0}%7d\",", formValuesStr.ToString().Substring(0, formValuesStr.ToString().Length - 3)));
                            }
                            else//formValues对象存在并且内容不为空
                            {
                                stiMvcViewerHtml = stiMvcViewerHtml.Replace(formValuesStartsWith, string.Format("\"formValues\":\"%7b{0}", formValuesStr.ToString()));
                            }
                        }
                        else//formValues对象不存在
                        {

                        }
                    }
                }
            }
            catch { }
            return stiMvcViewerHtml;
        }
        
    }
}