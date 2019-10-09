using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PayManage.Controllers
{
    /// <summary>
    /// 支付的基类，用于处理一些公共处理
    /// </summary>
    public class PayBaseController : Controller
    {
        #region 当前登录信息
        private ICurrentInfo _currentInfo;
        protected ICurrentInfo CurrentInfo
        {
            get
            {
                if (_currentInfo == null)
                {
                    _currentInfo = GetService<ICurrentInfo>();
                }
                return _currentInfo;
            }
        }
        #endregion

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
        #endregion

        /// <summary>
        /// 自动完成合约单位列表
        /// </summary>
        /// <param name="keyword">要查找的关键字</param>
        /// <returns>满足条件的合约单位列表</returns>
        public ActionResult AutoCompleteCorp(string keyword)
        {
            var services = GetService<ICompanyService>();
            var items = services.Query(CurrentInfo.HotelId, keyword);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

    }
}