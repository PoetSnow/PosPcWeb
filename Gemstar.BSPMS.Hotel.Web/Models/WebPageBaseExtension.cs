using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace Gemstar.BSPMS.Hotel.Web
{
    /// <summary>
    /// 页面扩展类，用于提供section的默认实现
    /// </summary>
    public static class WebPageBaseExtension
    {
        public static HelperResult RenderSection(WebPageBase webPage, string name, Func<dynamic, HelperResult> defaultContents)
        {
            if (webPage.IsSectionDefined(name))
            {
                return webPage.RenderSection(name);
            }
            return defaultContents(null);
        }
    }
}