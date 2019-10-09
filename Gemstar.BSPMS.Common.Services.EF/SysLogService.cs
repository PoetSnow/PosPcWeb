using Gemstar.BSPMS.Common.Services.Entities;
using System;
using System.Text;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Extensions;

namespace Gemstar.BSPMS.Common.Services.EF
{
    public class SysLogService : CRUDService<SysLog>, ISysLogService
    {
        public SysLogService(DbCommonContext dbContext) : base(dbContext, dbContext.SysLogs)
        {
            _dbContext = dbContext;
        }
        protected override SysLog GetTById(string id)
        {
            var log = new SysLog();
            log.Id = int.Parse(id);
            return log;
        }
        /// <summary>
        /// 增加运营管理的系统异常日志
        /// </summary>
        /// <param name="exceptionContext">异常上下文对象</param>
        /// <param name="currentUserName">当前登录的用户名</param>
        public void AddSysLog(ExceptionContext exceptionContext, string currentUserName)
        {
            var log = GetLogInstanceFromExceptionContext(exceptionContext);
            log.User = currentUserName;
            _dbContext.SysLogs.Add(log);
            _dbContext.SaveChanges();
        }
        public void AddSysLog(string info, string name,string url, string currentUserName, string currentHotelId)
        {
            var log = new SysLog
            {
                CDate = DateTime.Now,
                Name = name,
                Hid = currentHotelId,
                Info = info,
                Url = url,
                User = currentUserName
            };
            _dbContext.SysLogs.Add(log);
            _dbContext.SaveChanges();
        }
        /// <summary>
        /// 增加酒店pms的系统异常日志
        /// </summary>
        /// <param name="exceptionContext">异常上下文对象</param>
        /// <param name="currentUserName">当前登录的用户名</param>
        /// <param name="currentHotelId">当前的酒店id</param>
        public void AddSysLog(ExceptionContext exceptionContext, string currentUserName, string currentHotelId)
        {
            var log = GetLogInstanceFromExceptionContext(exceptionContext);
            log.Hid = currentHotelId;
            log.User = currentUserName;
            _dbContext.SysLogs.Add(log);
            _dbContext.SaveChanges();
        }
        #region 静态的记录日志方法,用于记录日志信息
        public static void AddSysLog(string name,string info,string user,DbCommonContext db)
        {
            var log = new SysLog
            {
                CDate = DateTime.Now,
                Name = name,
                Info = info,
                User = user
            };
            db.SysLogs.Add(log);
        }
        public static void AddSysLog(string name,Exception ex,string user,DbCommonContext db)
        {
            var infoBuilder = new StringBuilder();
            infoBuilder.AppendFormat("异常信息:{0}", ex.Message).AppendLine();
            if (ex.InnerException != null)
            {
                var inner = ex.InnerException;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }
                infoBuilder.AppendFormat("内部异常信息:{0}", inner.Message).AppendLine();
            }
            infoBuilder.AppendFormat("调用堆栈:{0}", ex.StackTrace).AppendLine();
            var log = new SysLog
            {
                CDate = DateTime.Now,
                Name = name,
                User = user,
                Info = infoBuilder.ToString()
            };
            db.SysLogs.Add(log);
        }
        #endregion
        private SysLog GetLogInstanceFromExceptionContext(ExceptionContext exceptionContext)
        {
            var log = new SysLog();
            log.CDate = DateTime.Now;

            var routeData = exceptionContext.RouteData;
            var controllerName = routeData.Values["controller"] as string;
            var actionName = routeData.Values["action"] as string;
            var areaName = "";
            if (routeData.DataTokens.ContainsKey("area"))
            {
                areaName = routeData.DataTokens["area"].ToString();
            }
            log.Url = string.Format("/{0}/{1}/{2}", areaName, controllerName, actionName);

            var controllerDescriptor = new ReflectedControllerDescriptor(exceptionContext.Controller.GetType());
            var businessTypeAttributes = controllerDescriptor.GetCustomAttributes(typeof(BusinessTypeAttribute), false);
            var businessTypeName = businessTypeAttributes.Length > 0 ? ((BusinessTypeAttribute)businessTypeAttributes[0]).Name : controllerDescriptor.ControllerName;
            log.Name = businessTypeName;

            var request = exceptionContext.HttpContext.Request;
            var remoteIp = UrlHelperExtension.GetRemoteClientIPAddress(request);
            log.Ip = remoteIp;


            var infoBuilder = new StringBuilder();
            infoBuilder.AppendFormat("异常信息:{0}", exceptionContext.Exception.Message).AppendLine();
            if (exceptionContext.Exception.InnerException != null)
            {
                var inner = exceptionContext.Exception.InnerException;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }
                infoBuilder.AppendFormat("内部异常信息:{0}", inner.Message).AppendLine();
            }
            infoBuilder.AppendFormat("调用堆栈:{0}", exceptionContext.Exception.StackTrace).AppendLine();

            var keys = request.QueryString.Keys;
            infoBuilder.Append("调用时的查询参数:");
            foreach (string key in keys)
            {
                infoBuilder.AppendFormat("{0}:{1};", key, request.QueryString[key]);
            }
            infoBuilder.AppendLine();
            keys = request.Form.Keys;
            infoBuilder.Append("调用时的form参数:");
            foreach (string key in keys)
            {
                infoBuilder.AppendFormat("{0}:{1};", key, request.Form[key]);
            }
            infoBuilder.AppendLine();
            keys = request.Cookies.Keys;
            infoBuilder.Append("调用时的cookie参数:");
            foreach (string key in keys)
            {
                infoBuilder.AppendFormat("{0}:{1};", key, request.Cookies[key].Value);
            }
            infoBuilder.AppendLine();
            log.Info = infoBuilder.ToString();

            return log;
        }
        private DbCommonContext _dbContext;
    }
}
