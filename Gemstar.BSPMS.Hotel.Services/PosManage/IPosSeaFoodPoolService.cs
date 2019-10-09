using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// 海鲜池接口
    /// </summary>
    public interface IPosSeaFoodPoolService : ICRUDService<PosBillDetail>
    {
        /// <summary>
        /// 获取酒店下所有称重的数据
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        List<up_pos_SeafoodPoolListResult> GetSeaFoodPoolList(string hid, string tabId, int pageIndex, int pageSize);

        /// <summary>
        /// 获取酒店下称重数量
        /// </summary>
        /// <param name="tabId"></param>
        /// <returns></returns>
        int GetSeaFoodPoolListCount(string hid, string tabId);

        /// <summary>
        /// 获取已称重的数据
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="querytext">查询条件</param>
        /// <returns></returns>
        List<up_pos_WeighedListResult> GetWeighedList(string hid,string querytext);
    }
}
