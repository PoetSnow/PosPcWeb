using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Common.Extensions
{
    /// <summary>
    /// UrlHelper类的扩展方法
    /// </summary>
    public static class UrlHelperExtension
    {
        /// <summary>
        /// 单据打印地址
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="reportCode"></param>
        /// <param name="chineseName"></param>
        /// <param name="paraStr"></param>
        /// <param name="isUrlEncodeParaStr"></param>
        /// <returns></returns>
        public static string BillPrintUrl(this UrlHelper urlHelper, string reportCode, string chineseName, string paraStr, bool isUrlEncodeParaStr = true)
        {
            var request = urlHelper.RequestContext.HttpContext.Request;
            var server = urlHelper.RequestContext.HttpContext.Server;
            var url = "http://" + request.Url.Host;
            if (isUrlEncodeParaStr)
            {
                paraStr = server.UrlEncode(paraStr);
            }
            return string.Format("{0}/ReportManage/SRBillReportView/Index?ReportCode={1}&ParameterValues={2}&ChineseName={3}", url, server.UrlEncode(reportCode), paraStr, server.UrlEncode(chineseName));
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetRemoteClientIPAddress(HttpRequestBase request = null)
        {
            if (request == null)
            {
                request = new HttpRequestWrapper(HttpContext.Current.Request);
            }
            string defaultAddress = "127.0.0.1";
            var slbRealIP = request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrWhiteSpace(slbRealIP) && slbRealIP != defaultAddress && IsIP(slbRealIP))
            {
                return slbRealIP;
            }
            var remoteStr = request.Headers["X-Real-IP"];
            if (!string.IsNullOrWhiteSpace(remoteStr) && remoteStr != defaultAddress)
            {
                return remoteStr;
            }
            //UserHostAddress
            string userHostAddress = request.UserHostAddress;
            if (!string.IsNullOrWhiteSpace(userHostAddress) && userHostAddress != defaultAddress && IsIP(userHostAddress))
            {
                return userHostAddress;
            }
            //remote_addr
            userHostAddress = request.ServerVariables["REMOTE_ADDR"];
            if (!string.IsNullOrWhiteSpace(userHostAddress) && userHostAddress != defaultAddress && IsIP(userHostAddress))
            {
                return userHostAddress;
            }
            //http_x_forwarded_for
            userHostAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrWhiteSpace(userHostAddress) && userHostAddress.Contains(","))
            {
                string[] temp = userHostAddress.Split(',');
                if (temp != null && temp.Length > 0)
                {
                    userHostAddress = temp[0];
                }
            }
            if (!string.IsNullOrWhiteSpace(userHostAddress) && userHostAddress != defaultAddress && IsIP(userHostAddress))
            {
                return userHostAddress;
            }
            return defaultAddress;
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
        public static MvcHtmlString ChangeReportDesingUrl(this UrlHelper urlHelper, MvcHtmlString reportDesignString)
        {
            var request = urlHelper.RequestContext.HttpContext.Request;
            var server = urlHelper.RequestContext.HttpContext.Server;
            var originUrl = request.Url;
            var host = request.Headers["OriginNginxUri"];
            if (!string.IsNullOrWhiteSpace(host))
            {
                originUrl = new Uri(host);
            }
            else
            {
                var urlRefer = request.UrlReferrer;
                if (urlRefer != null)
                {
                    originUrl = urlRefer;
                }
            }

            var htmlStr = reportDesignString.ToHtmlString();

            var designUrl = new Uri(originUrl, VirtualPathUtility.ToAbsolute("~/ReportManage/SRReportDesign/"));
            var designUrlStr = designUrl.AbsoluteUri + "{action}";
            var index = htmlStr.IndexOf("stimulsoft_flashvars.routes");
            htmlStr = htmlStr.Insert(index, string.Format("stimulsoft_flashvars.url=\"{0}\";", server.UrlEncode(Convert.ToBase64String(Encoding.UTF8.GetBytes(designUrlStr)))));
            return MvcHtmlString.Create(htmlStr);
        }

        /// <summary>
        /// 获取页面记录数量
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int GetPageSizeForCookies(HttpRequestBase request)
        {
            int pageSize = 10;
            if (request != null)
            {
                var cookie = request.Cookies.Get("pageSize");
                if (cookie != null && !string.IsNullOrWhiteSpace(cookie.Value))
                {
                    Int32.TryParse(cookie.Value, out pageSize);
                    if (pageSize <= 0)
                    {
                        pageSize = 10;
                    }
                }
            }
            return pageSize;
        }
        /// <summary>
        /// 获取页面记录数量
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static int GetPageSizeForCookies(HttpRequest request)
        {
            return GetPageSizeForCookies((request != null) ? new HttpRequestWrapper(request) : null);
        }
        /// <summary>
        /// 获取房态图假房的勾选
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool GetIsNotRoomForCookies(HttpRequestBase request)
        {
            bool isnotroom = false;
            if (request != null)
            {
                var cookie = request.Cookies.Get("isNotRoom");
                if (cookie != null && !string.IsNullOrWhiteSpace(cookie.Value))
                {
                    Boolean.TryParse(cookie.Value, out isnotroom);
                }
            }
            return isnotroom;
        }

        /// <summary>
        /// 获取指定相对路径的绝对路径，是根据当前请求域名来动态决定的
        /// </summary>
        /// <param name="urlHelper">UrlHelper对象</param>
        /// <param name="specialUrl">指定路径，如~/aaa/bbb.aspx</param>
        /// <returns>指定路径的绝对路径，如当前访问pms.gshis.net/ccc/ddd.aspx,相对路径为~/aaa/bbb.aspx，则返回值为pms.gshis.net/aaa/bbb.aspx</returns>
        public static string GetAbsoulteUrlForSpecialUrl(this UrlHelper urlHelper, string specialUrl)
        {
            var originUrl = CurrentUri(urlHelper);
            var url = new Uri(originUrl, VirtualPathUtility.ToAbsolute(specialUrl));
            return url.AbsoluteUri;
        }

        /// <summary>
        /// 获取当前请求的原始uri
        /// </summary>
        /// <param name="urlHelper">UrlHelper对象</param>
        /// <returns>当前请求的原始uri</returns>
        public static Uri CurrentUri(this UrlHelper urlHelper)
        {
            var request = urlHelper.RequestContext.HttpContext.Request;
            var originUrl = request.Url;
            var host = request.Headers["OriginNginxUri"];
            if (string.IsNullOrWhiteSpace(host))
            {
                host = request.Headers["Host"];
            }
            if (!string.IsNullOrWhiteSpace(host))
            {
                if (!host.StartsWith("http"))
                {
                    host = "http://" + host;
                }
                originUrl = new Uri(host);
            }
            else
            {
                var urlRefer = request.UrlReferrer;
                if (urlRefer != null)
                {
                    originUrl = urlRefer;
                }
            }
            return originUrl;
        }
    }
}
