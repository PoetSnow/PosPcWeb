using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// 扫码点餐点菜记录
    /// </summary>
    public interface IPosMorderListService : ICRUDService<PosMorderList>
    {
        /// <summary>
        /// 根据账单ID获取点菜记录
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billId">账单ID</param>
        /// <returns></returns>

        List<PosMorderList> GetPosMorderListByBillId(string hid, string billId);

       
        /// <summary>
        /// 根据微信ID与酒店ID 获取账单信息
        /// </summary>
        /// <param name="openId">微信ID</param>
        /// <param name="hid">酒店代码</param>
        /// <returns></returns>
        PosMorderList GetPosMorderListByOpenId(string openId,string hid);
    }
}