using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    public interface IPosCostItemService: ICRUDService<PostCost>
    {
        /// <summary>
        /// 获取库存物品集合根据项目id
        /// </summary>
        /// <param name="hId"></param>
        /// <param name="itemId">物品id</param>
        /// <returns></returns>
        List<PostCost> GetListPostCostByItemId(string hId, string itemId);
        /// <summary>
        /// 获取库存物品信息根据id
        /// </summary>
        /// <param name="hId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        PostCost GetPostCostInfoByKey(string hId, Guid id);

        /// <summary>
        /// 根据酒店ID，项目ID，单位ID 获取物品列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="unitId">单位ID</param>
        /// <returns></returns>
        List<PostCost> GetListPostCostByItemId(string hid, string itemId, string unitId);
    }
}
