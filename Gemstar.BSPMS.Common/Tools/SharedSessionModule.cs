using System;
using System.Reflection;
using System.Web;
using System.Linq;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// A HttpModule used for sharing the session between Applications in 
    /// sub domains.
    /// </summary>
    public class SharedSessionModule : IHttpModule
    {
        // Cache settings on memory.
        protected static string applicationName;
        protected static string rootDomain;
        /// <summary>
        /// 获取访问域名的最后三级域名字符串
        /// </summary>
        /// <param name="accessDomain">当前访问域名</param>
        /// <returns>最后三级域名字符串</returns>
        public static string GetLastThreeLevelDomain(string accessDomain, bool addDotAtBegin = false)
        {
            //如果域名是三级的，比如pms.gshis.com，则修改为.pms.gshis.com
            //如果域名是四级的，比如vip1.pms.gshis.com，则修改为.pms.gshis.com
            //如果小于三级的，则直接使用原域名
            var parts = accessDomain.Split('.');
            var length = parts.Length;
            if (length >= 3)
            {
                return string.Format("{3}{0}.{1}.{2}", parts[length - 3], parts[length - 2], parts[length - 1], addDotAtBegin ? "." : "");
            }
            else
            {
                return accessDomain;
            }
        }

        #region IHttpModule Members
        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">
        /// An System.Web.HttpApplication
        /// that provides access to the methods,
        /// properties, and events common to all application objects within 
        /// an ASP.NET application.
        /// </param>
        public void Init(HttpApplication context)
        {
            // This module requires both Application Name and Root Domain to work.
            if (string.IsNullOrEmpty(applicationName) ||
                string.IsNullOrEmpty(rootDomain))
            {
                var settingProvider = context as ISettingProvider;
                if (settingProvider == null || settingProvider.SettingInfo == null)
                {
                    return;
                }
                var settingInfo = settingProvider.SettingInfo;
                applicationName = settingInfo.ApplicationName;
                rootDomain = settingInfo.RootDomain;

            }

            // Change the Application Name in runtime.
            FieldInfo runtimeInfo = typeof(HttpRuntime).GetField("_theRuntime",
                BindingFlags.Static | BindingFlags.NonPublic);
            HttpRuntime theRuntime = (HttpRuntime)runtimeInfo.GetValue(null);
            FieldInfo appNameInfo = typeof(HttpRuntime).GetField("_appDomainAppId",
                BindingFlags.Instance | BindingFlags.NonPublic);

            appNameInfo.SetValue(theRuntime, applicationName);

            // Subscribe Events.
            context.PostRequestHandlerExecute += new EventHandler(context_PostRequestHandlerExecute);
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module
        /// that implements.
        /// </summary>
        public void Dispose()
        {
        }
        #endregion

        /// <summary>
        /// Before sending response content to client, change the Cookie to Root Domain
        /// and store current Session Id.
        /// </summary>
        /// <param name="sender">
        /// An instance of System.Web.HttpApplication that provides access to
        /// the methods, properties, and events common to all application
        /// objects within an ASP.NET application.
        /// </param>
        void context_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            try
            {
                HttpApplication context = (HttpApplication)sender;


                if (context.Session != null &&
                    !string.IsNullOrEmpty(context.Session.SessionID))
                {

                    var cookieDomain = rootDomain;
                    if (context.Context != null)
                    {
                        var accessDomain = context.Context.Request.Headers["Host"];
                        if (!string.IsNullOrWhiteSpace(accessDomain))
                        {
                            cookieDomain = GetLastThreeLevelDomain(accessDomain, true);
                        }
                    }
                    setCookieInfos("ASP.NET_SessionId", context, cookieDomain, true, context.Session.SessionID);
                    setCookieInfos(".ASPXAUTH", context, cookieDomain, false, null);
                }
            }
            catch
            {
                //有些请求的会话信息会不可用，直接忽略即可
            }
        }
        void setCookieInfos(string cookieName, HttpApplication context, string cookieDomain, bool isSetValue, string value)
        {
            var allKeys = context.Response.Cookies.AllKeys;
            if (allKeys.Contains(cookieName))
            {
                // All Applications use one root domain to store this Cookie
                // So that it can be shared.
                // ASP.NET store a Session Id in cookie to specify current Session.
                HttpCookie cookie = context.Response.Cookies[cookieName];
                // Need to store current Session Id during every request.
                if (isSetValue)
                {
                    cookie.Value = value;
                }

                cookie.Domain = cookieDomain;

                cookie.Path = "/";
            }
        }
    }
}
