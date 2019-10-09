using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Linq;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface IRateService : ICRUDService<Rate>
    {
        /// <summary>
        /// 获取价格体系键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<KeyValuePair<string, string>> List(string hid, string selectId = null);

        /// <summary>
        /// 获取所有价格体系键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<System.Web.Mvc.SelectListItem> ListAll(string hid);

        /// <summary>
        /// 获取所有价格体系键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<System.Web.Mvc.SelectListItem> PermanentRoomListAll(string hid);

        /// <summary>
        /// 获取引用价格代码信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<Rate> GetRefRateid(string hid);

        /// <summary>
        /// 获取价格代码信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<Rate> GetRate(string hid);

        /// <summary>
        /// 获取价格代码信息(状态为启用和在有效期内的)
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<Rate> GetRateref(string hid);

        /// <summary>
        /// 禁用价格代码
        /// </summary>
        /// <param name="rateid"></param>
        /// <param name="hid"></param>
        /// <returns></returns>
        int DisableRates(string rateid, string hid,EntityStatus status);

        /// <summary>
        /// 判断价格代码在预订和在住中是否存在，存在则不允许修改
        /// </summary> 
        /// <param name="hid"></param>
        /// <param name="rateid"></param>
        /// <returns></returns>
        string checkExistOthertb(string hid, string rateid, bool isdel);

        void updateRateToRefcode(string hid, string ratecode, string refratecode, bool? addmode, decimal? addamount);
    }
}
