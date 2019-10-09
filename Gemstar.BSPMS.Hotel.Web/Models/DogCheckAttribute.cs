using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gemstar.BSPMS.Hotel.Web.Models
{
    public class DogCheckAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //修改为如果是出提示时，则不显示提示页面，允许用户直接进入系统，但是不能进入报表中心
            if (!ProgVersion.IsDogOk)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "RegisterAlter", controller = "Register" }));
            }
            //修改为暂不控制提示
            else if (!string.IsNullOrEmpty(ProgVersion.DogErrorMessage) && ProgVersion.DogErrorMessage != ProgVersion.NoReportDogErrorMessage)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "RegAlter", controller = "Register" }));
            }
        }
    }
}
