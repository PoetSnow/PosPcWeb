using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// Pos扫码点餐滚动菜式
    /// </summary>
    public interface IPosMScrollService : ICRUDService<PosMScroll>
    {
        /// <summary>
        /// 判断指定Id的滚动菜式是否已经存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsExists(string hid, string id);
        /// <summary>
        /// 获取滚动菜式列表
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<PosMScroll> GetPosMScrollList(string hid);

        /// <summary>
        /// 获取滚动菜式列表
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<up_pos_MScrollItemListResult> GetPosMScrollItemList(string hid);
    }
}