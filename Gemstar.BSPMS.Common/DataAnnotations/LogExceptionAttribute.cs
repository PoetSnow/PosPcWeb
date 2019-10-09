using Gemstar.BSPMS.Common.Services;
using System;
using System.Web.Mvc;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// 通用的异常错误信息记录
    /// 站点所有页面在异常发生后，均需要记录异常日志，并转向错误提示页面（异常内容的详略程度由具体需求决定）、
    /// 所有返回JSON数据的异步请求，不但需要记录异常日志，而且需要向客户端返回JSON格式的错误信息提示，而不是转向错误提示页面（异步请求也不可能转向错误提示页面）
    /// 采用AOP思想，将异常处理解耦
    /// 尽量精简声明Attribute的重复代码
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class LogExceptionAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                try
                {
                    var logService = DependencyResolver.Current.GetService<ISysLogService>();
                    var username = "";
                    if (filterContext.HttpContext.Session != null)
                    {
                        username = filterContext.HttpContext.Session["UserName"] as string;
                    }
                    logService.AddSysLog(filterContext, username);
                }catch
                {
                    //记录异常时如果重新抛出异常则不需要记录，继续处理之前的业务异常
                }
            }

            if (filterContext.Result is JsonResult)
            {
                //当结果为json时，设置异常已处理
                filterContext.ExceptionHandled = true;
            }
            else
            {
                //否则调用原始设置
                base.OnException(filterContext);
            }
        }
    }
}
