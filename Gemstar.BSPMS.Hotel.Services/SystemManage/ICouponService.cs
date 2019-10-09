using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface ICouponService : ICRUDService<Coupon>
    {
        /// <summary>
        /// 获取优惠券类型
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="itemtypeid"></param>
        /// <returns></returns>
        List<Coupon> GetCouponbyitemTypeid(string hid, int itemtypeid);
        /// <summary>
        /// 启用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        JsonResultData Enable(string id);
        /// <summary>
        /// 禁用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        JsonResultData Disable(string id);
        /// <summary>
        /// 是否发放过优惠券
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool isExistTicket(string id);
        /// <summary>
        /// 获取优惠券类型
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        List<KeyValuePair<string, string>> List(string hid);
    }
}
