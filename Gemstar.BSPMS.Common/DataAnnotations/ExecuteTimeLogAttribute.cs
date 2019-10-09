using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Common.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ExecuteTimeLogAttribute : ActionFilterAttribute
    {
        public ExecuteTimeLogAttribute()
        {
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.AddHeader("ActionExecutingTime", DateTime.Now.ToString("HH:mm:ss.fff"));
            base.OnActionExecuting(filterContext);
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.AddHeader("ActionExecutedTime", DateTime.Now.ToString("HH:mm:ss.fff"));
            base.OnActionExecuted(filterContext);
        }
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.AddHeader("ResultExecutedTime", DateTime.Now.ToString("HH:mm:ss.fff"));
            base.OnResultExecuted(filterContext);
        }
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.AddHeader("ResultExecutingTime", DateTime.Now.ToString("HH:mm:ss.fff"));
            base.OnResultExecuting(filterContext);
        }
    }
}
