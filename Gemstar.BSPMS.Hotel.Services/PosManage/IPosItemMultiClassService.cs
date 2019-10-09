using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos消费项目对应大类服务接口
    /// </summary>
    public interface IPosItemMultiClassService : ICRUDService<PosItemMultiClass>
    {
        /// <summary>
        /// 判断指定的代码或者名称的消费消费项目对应大类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="itemClassid">消费消费项目对应大类id</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目对应大类了，false：没有相同的</returns>
        bool IsExists(string hid, string itemId, string itemClassid);
        /// <summary>
        /// 判断指定的代码或者名称的消费项目对应大类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="itemClassid">消费消费项目对应大类id</param>
        /// <param name="exceptId">要排队的消费项目对应大类id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目对应大类了，false：没有相同的</returns>
        bool IsExists(string hid, string itemId, string itemClassid, Guid exceptId);
        /// <summary>
        /// 获取指定酒店和项目下的消费项目对应大类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目Id</param>
        /// <returns>指定酒店和项目下的消费项目对应大类列表</returns>
        List<up_pos_list_ItemMultiClassByItemidResult> GetPosItemMultiClassByItemId(string hid, string itemId);

        List<PosItemMultiClass> GetPosItemMultiClassByItemIdForCopy(string hid, string itemId);


        /// <summary>
        /// 根据酒店ID，项目,大类获取数据
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="itemId"></param>
        /// <param name="itemClassId"></param>
        /// <returns></returns>
        PosItemMultiClass GetPosItemMultiClassByItemEditAll(string hid, string itemId, string itemClassId);
    }
}
