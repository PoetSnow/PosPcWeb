using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Gemstar.BSPMS.Common.Tools
{
    public static class CommonHelper
    {
        public static string GetCheckCode()
        {
            Random r = new Random();
            var str = "";
            for (int i = 0; i < 6; i++)
            {
                str += r.Next(10);
            }
            return str;
        }

        /// <summary>
        /// 每页多少条记录
        /// </summary>
        public static int[] PageSizes = new int[] { 5, 10, 20, 50, 100 };

        /// <summary>
        /// 渲染呈现视图到字符串
        /// </summary>
        /// <param name="context"></param>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string RenderViewToString(this ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            context.Controller.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context,
                                                  viewResult.View,
                                                  context.Controller.ViewData,
                                                  context.Controller.TempData,
                                                  sw);
                try
                {
                    viewResult.View.Render(viewContext, sw);
                }
                catch (Exception ex)
                {
                    throw;
                }

                return sw.GetStringBuilder().ToString().Replace("\r", "").Replace("\n","");
            }
        }


        /// <summary>
        /// 半角转全角
        /// 全角空格为12288,半角空格为32，其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSBC(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) { return ""; }
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288; continue;
                }
                if (c[i] < 127) c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }
        /// <summary>
        /// 全角转半角
        /// 全角空格为12288，半角空格为32，其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isRemoveLastPunctuation">是否去除最后的标点符号</param>
        /// <returns></returns>
        public static string ToDBC(string input, bool isRemoveLastPunctuation = false)
        {
            if (string.IsNullOrWhiteSpace(input)) { return ""; }
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32; continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }

            if (isRemoveLastPunctuation)
            {
                if (c != null && c.Length > 0)
                {
                    if (Char.IsPunctuation(c.LastOrDefault()))
                    {
                        c[c.Length - 1] = char.Parse(" ");
                    }
                }
            }

            return new string(c).Trim();
        }

        /// <summary>
        /// 字符串反转
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        public static string Reverse(string original)
        {
            char[] arr = original.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
        /// <summary>
        /// 转换为GUID
        /// </summary>
        /// <param name="value">字符串</param>
        /// <param name="newValue">GUID</param>
        /// <returns></returns>
        public static bool ToGuid(string value, out Guid newValue)
        {
            newValue = Guid.Empty;
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (Guid.TryParse(value, out newValue))
                {
                    if (newValue != null && newValue != Guid.Empty)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
