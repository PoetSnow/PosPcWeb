using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF;

namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Controllers
{
    /// <summary>
    /// 基础的微信控制器，用于实现一些通用实现
    /// </summary>
    [NotAuth]
    public class BaseWeixinController : Controller
    {
        #region 获取服务接口
        /// <summary>
        /// 获取指定服务接口的实例
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        /// <returns>指定服务接口的实例</returns>
        protected T GetService<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }
        /// <summary>
        /// 获取中央数据库实例
        /// </summary>
        /// <returns>中央数据库实例</returns>
        protected DbCommonContext GetCenterDb()
        {
            return GetService<DbCommonContext>();
        }
        /// <summary>
        /// 获取指定酒店id对应的营业数据库实例
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>酒店id对应的营业数据库实例</returns>
        protected DbHotelPmsContext GetHotelDb(string hid)
        {
            var hotelInfoService = GetService<IHotelInfoService>();
            var hotelInfo = hotelInfoService.GetHotelInfo(hid);
            var isConnectViaInternet = hotelInfoService.IsConnectViaInternte();
            var connStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "GemstarBSPMS",hotelInfo.DbServerInternet,isConnectViaInternet);
            var hotelDb = new DbHotelPmsContext(connStr, hid, "", Request);
            return hotelDb;
        }
        #endregion
    }
}