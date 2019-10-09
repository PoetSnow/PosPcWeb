using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos营业点价格服务接口
    /// </summary>
    public interface IPosItemRefeService : ICRUDService<PosItemRefe>
    {
        /// <summary>
        /// 判断指定的代码或者名称的营业点价格是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="refe">营业点</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的营业点价格了，false：没有相同的</returns>
        bool IsExists(string hid, string itemId, string refe);
        /// <summary>
        /// 判断指定的代码或者名称的营业点价格是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="refe">营业点</param>
        /// <param name="exceptId">要排队的营业点价格id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的营业点价格了，false：没有相同的</returns>
        bool IsExists(string hid, string itemId, string refe, Guid exceptId);
        /// <summary>
        /// 获取酒店和项目下的对应营业点列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目Id</param>
        /// <returns>酒店和项目下的对应营业点列表</returns>
        List<up_pos_list_ItemRefeByItemidResult> GetPosItemRefeByItemId(string hid, string itemId);

        /// <summary>
        /// 获取酒店和项目下的对应营业点列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">项目Id</param>
        /// <returns>酒店和项目下的对应营业点列表</returns>
        List<PosItemRefe> GetPosItemRefeForCopy(string hid,string itemId);

        /// <summary>
        /// 根据酒店ID，消费项目，以及营业点获取数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <returns></returns>
        PosItemRefe GetPosItemRefeByEditAll(string hid,string itemId,string refeId);
    }
}
