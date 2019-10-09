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
    public interface IPosSellOutService : ICRUDService<PosSellout>
    {
        /// <summary>
        /// 根据沽清表的数据查询出消费项目
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <returns></returns>
        List<up_list_positemBySellOutResult> GetItemListBySellOut(string hid,string module,string refeId);

        /// <summary>
        /// 根据酒店ID，项目ID，单位ID，与营业点ID判断沽清表是否有数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="unitId">单位ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <returns></returns>
        bool IsExists(string hid,string itemId);

        /// <summary>
        /// 根据酒店ID，项目ID，单位ID，与营业点Id获取数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="unitId">单位ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <returns></returns>
        PosSellout GetPosSelloutByItemId(string hid, string itemId);

        /// <summary>
        /// 根据酒店ID，项目ID，单位ID，与营业点Id获取数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="unitId">单位ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <returns></returns>
        List<PosSellout> GetPosSelloutListByItemId(string hid, string itemId);
    }
}
