using Gemstar.BSPMS.Common.Services;
using Stimulsoft.Report.Mvc;
using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.SystemManage;

namespace Gemstar.BSPMS.Hotel.Services.AuthManages
{
    /// <summary>
    /// 权限检查
    /// </summary>
    public class AuthAttribute : FilterAttribute, IAuthorizationFilter
    {
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            //先验证当前的action或controller是否不需要权限验证
            if (NeedNotAuthorize(filterContext))
            {
                return;
            }
            if (OutputCacheAttribute.IsChildActionCacheActive(filterContext))
            {
                // If a child action cache block is active, we need to fail immediately, even if authorization
                // would have succeeded. The reason is that there's no way to hook a callback to rerun
                // authorization before the fragment is served from the cache, so we can't guarantee that this
                // filter will be re-run on subsequent requests.
                throw new InvalidOperationException("验证模块不允许缓存");
            }
            //进行权限验证
            IPrincipal user = filterContext.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                HandleUnauthorizedRequest(filterContext);
                return;
            }
            var currentInfo = DependencyResolver.Current.GetService<ICurrentInfo>();
            var currUser = currentInfo.UserId;
            if (string.IsNullOrEmpty(currUser))
            {
                currentInfo.Clear();

                redirectToLogin(filterContext);
                return; //停止往后执行报异常
            }
            //检查班次是否已经关闭
            var shiftId = currentInfo.ShiftId;
            var hid = currentInfo.HotelId;
            //if(!string.IsNullOrEmpty(shiftId) && !string.IsNullOrEmpty(hid))
            //{
            //    var shiftService = DependencyResolver.Current.GetService<IShiftService>();
            //    var shift = shiftService.GetShiftsAvailable(hid).SingleOrDefault(w => w.Id == shiftId);
            //    if(shift == null || shift.LoginStatus != ShiftLoginStatus.已开)
            //    {
            //        currentInfo.Clear();
            //        redirectToLogin(filterContext, "当前班次已关闭或者未开，请退出重新选择班次");
            //        return;
            //    }
            //}
            //检查页面权限
            var authButtonArray = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthButtonAttribute), false);
            if (authButtonArray.Length == 0)
            {
                throw new ApplicationException("Action未指定对应的权限项，请应用AuthButtonAttribute属性来指定");
            }
            var authPageArray = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AuthPageAttribute), false);
            if (authPageArray.Length == 0)
            {
                //如果action上没有定义authPage属性，则直接从controller上获取
                authPageArray = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AuthPageAttribute), false);
                if (authPageArray.Length == 0)
                {
                    throw new ApplicationException("Action和controller都未指定对应的权限项，请应用AuthPageAttribute属性来指定");
                }
            }
            //根据当前产品类型取对应的权限项id
            var product = currentInfo.ProductType;
            AuthPageAttribute authPage = null;
            foreach(var o in authPageArray) {
                var obj = o as AuthPageAttribute;
                if(obj != null && obj.ProductTypeInstance == product) {
                    authPage = obj;
                }
            }
            if(authPage == null) {
                throw new ApplicationException("当前请求功能不适用于当前登录的产品模块，请选择其他功能");
            }
            
            var authButton = (AuthButtonAttribute)authButtonArray[0];
            var authManage = DependencyResolver.Current.GetService<IAuthCheck>();
            if (!authManage.HasAuth(currUser, authPage.AuthCode, (Int64)authButton.AuthButtonValue,currentInfo.HotelId) && authButton.AuthButtonValue != AuthFlag.None)
            {
                redirectToNoAuth(filterContext);
                return;
            }
            //检查基础资料集团管控权限，只有在controller上设置了AuthBasicData属性，并且当前的authButton中的权限项是增加，修改，删除，禁用，启用时才进行判断
            var basicDataArray = filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AuthBasicDataAttribute), false);
            if(basicDataArray.Length > 0)
            {
                var basicDataCode = ((AuthBasicDataAttribute)basicDataArray[0]).Code;
                if(authButton.AuthButtonValue == AuthFlag.Add 
                    || authButton.AuthButtonValue == AuthFlag.Update 
                    || authButton.AuthButtonValue == AuthFlag.Delete
                    || authButton.AuthButtonValue == AuthFlag.Disable
                    || authButton.AuthButtonValue == AuthFlag.Enable)
                {
                    var hotelInfoService = DependencyResolver.Current.GetService<IHotelInfoService>();
                    var allBasicDatas = hotelInfoService.GetBasicDataForAll();
                    var basicDataControlService = DependencyResolver.Current.GetService<IBasicDataResortControlService>();
                    var dataControl = basicDataControlService.GetBasicDataControl(basicDataCode, currentInfo.HotelId, currentInfo.GroupId, allBasicDatas);
                    if(authButton.AuthButtonValue == AuthFlag.Add && !dataControl.CanAdd)
                    {
                        redirectToNoAuth(filterContext, dataControl.MsgStr);
                        return;
                    }
                    if (authButton.AuthButtonValue == AuthFlag.Update && !dataControl.CanUpdate)
                    {
                        redirectToNoAuth(filterContext, dataControl.MsgStr);
                        return;
                    }
                    if (authButton.AuthButtonValue == AuthFlag.Delete && !dataControl.CanDelete)
                    {
                        redirectToNoAuth(filterContext, dataControl.MsgStr);
                        return;
                    }
                    if (authButton.AuthButtonValue == AuthFlag.Disable && !dataControl.CanDisable)
                    {
                        redirectToNoAuth(filterContext, dataControl.MsgStr);
                        return;
                    }
                    if (authButton.AuthButtonValue == AuthFlag.Enable && !dataControl.CanEnable)
                    {
                        redirectToNoAuth(filterContext, dataControl.MsgStr);
                        return;
                    }
                }
            }
            HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
            cachePolicy.SetProxyMaxAge(new TimeSpan(0));
        }
        private void redirectToLogin(AuthorizationContext filterContext,string msg = "您的登录信息已经过期，请重新登录")
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {

                if (filterContext.HttpContext.Request.RequestType == "POST")
                {
                    filterContext.Result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = JsonResultData.Failure(msg, 1) };
                }
                else
                    //filterContext.Result = new ContentResult { Content = "<script>alert('" + msg + "');window.top.location.href='" + FormsAuthentication.LoginUrl + "'</script>" };
                filterContext.Result = new ContentResult { Content = "<script>jAlert('" + msg+"','知道了',function(){window.top.location.href='"+ FormsAuthentication.LoginUrl + "'})</script>" };
            }
            else
                filterContext.Result = new ContentResult { Content = "<script>alert('" + msg + "');window.top.location.href='" + FormsAuthentication.LoginUrl + "'</script>" };
        }
        private void redirectToNoAuth(AuthorizationContext filterContext,string msgText = "你没有权限访问此模块，请与系统管理员联系")
        {
            var actionName = filterContext.ActionDescriptor.ActionName;
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                if (filterContext.HttpContext.Request.RequestType == "POST")
                {
                    filterContext.Result = new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = JsonResultData.Failure(msgText) };
                }
                else
                {
                    filterContext.Result = new ContentResult { Content = msgText };
                }

            }
            else if (actionName == "SaveReportTemplate")
            {
                filterContext.Result = StiMvcDesigner.SaveReportResult("保存失败:你没有权限访问此模块，请与系统管理员联系");
            }
            else
            {
                var routeDic = new System.Web.Routing.RouteValueDictionary();
                routeDic.Add("area", "");
                routeDic.Add("controller", "Home");
                routeDic.Add("action", "Deny");
                filterContext.Result = new RedirectToRouteResult(routeDic);
            }
        }
        private bool NeedNotAuthorize(AuthorizationContext filterContext)
        {
            //验证当前的action或controller是否不需要权限验证
            return filterContext.ActionDescriptor.GetCustomAttributes(typeof(NotAuthAttribute), false).Length > 0 ||
                filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(NotAuthAttribute), false).Length > 0;
        }
        /// <summary>
        /// 判断当前post请求是否带有[KendoGridDatasourceException]
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        private bool GridNotAuthorize(AuthorizationContext filterContext)
        {
            return filterContext.ActionDescriptor.GetCustomAttributes(typeof(GridAttribute), false).Length > 0 ||
                filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(GridAttribute), false).Length > 0;
        }
        protected virtual void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Returns HTTP 401 - see comment in HttpUnauthorizedResult.cs.
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}
