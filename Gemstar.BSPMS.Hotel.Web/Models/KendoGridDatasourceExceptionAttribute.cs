using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System;

namespace Gemstar.BSPMS.Hotel.Web.Models
{
    /// <summary>
    /// kendo grid异步查询数据时出现异常时的返回信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class KendoGridDatasourceExceptionAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                var dt = new DataTable();
                var modelState = new ModelStateDictionary();
                Exception inner = filterContext.Exception;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }
                modelState.AddModelError("exception", inner.Message);
                //返回异常JSON
                filterContext.Result = new JsonResult
                {
                    Data = new DataSourceResult
                    {
                        Data = new List<object>(),
                        Total = 0,
                        Errors = modelState.SerializeErrors()
                    }
                };
            }
        }
    }
}