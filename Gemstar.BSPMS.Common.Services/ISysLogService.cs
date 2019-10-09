using Gemstar.BSPMS.Common.Services.Entities;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Common.Services
{
    /// <summary>
    /// 日志服务接口
    /// </summary>
    public interface ISysLogService : ICRUDService<SysLog>
    {
        /// <summary>
        /// 增加运营管理的系统异常日志
        /// </summary>
        /// <param name="exceptionContext">异常上下文对象</param>
        /// <param name="currentUserName">当前登录的用户名</param>
        void AddSysLog(ExceptionContext exceptionContext, string currentUserName);
        /// <summary>
        /// 增加酒店pms的系统异常日志
        /// </summary>
        /// <param name="exceptionContext">异常上下文对象</param>
        /// <param name="currentUserName">当前登录的用户名</param>
        /// <param name="currentHotelId">当前的酒店id</param>
        void AddSysLog(ExceptionContext exceptionContext, string currentUserName, string currentHotelId);
        void AddSysLog(string info, string name,string url, string currentUserName, string currentHotelId);
    }
}
