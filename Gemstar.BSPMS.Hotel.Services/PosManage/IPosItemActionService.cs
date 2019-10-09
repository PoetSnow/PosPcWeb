using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos消费项目对应作法服务接口
    /// </summary>
    public interface IPosItemActionService : ICRUDService<PosItemAction>
    {
        /// <summary>
        /// 判断指定的代码或者名称的消费项目对应作法是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemid">消费项目对应作法代码</param>
        /// <param name="actionid">消费项目对应作法名称</param>
        /// <param name="exceptId">要排队的消费项目对应作法id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目对应作法了，false：没有相同的</returns>
        bool IsExists(string hid, string itemid, string actionid);

        /// <summary>
        /// 判断指定的代码或者名称的消费项目对应作法是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemid">消费项目对应作法代码</param>
        /// <param name="actionid">消费项目对应作法名称</param>
        /// <param name="exceptId">要排队的消费项目对应作法id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的消费项目对应作法了，false：没有相同的</returns>
        bool IsExists(string hid, string itemid, string actionid, Guid exceptId);

        /// <summary>
        /// 根据消费项目获取消费项目对应作法
        /// </summary>
        /// <param name="itemId">消费项目ID</param>
        /// <returns></returns>
        List<up_pos_list_ItemActionByItemidResult> GetPosItemActionListByItemId(string hid, string itemId);

        /// <summary>
        /// 根据消费项目获取消费项目对应作法
        /// </summary>
        /// <param name="hid">酒店</param>
        /// <param name="itemId">消费项目</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        List<up_pos_list_ItemActionByItemidResult> GetPosItemActionListByItemId(string hid, string itemId, int pageIndex, int pageSize);

        /// <summary>
        /// 根据消费项目获取消费项目对应作法总数
        /// </summary>
        /// <param name="hid">酒店</param>
        /// <param name="itemid">消费项目</param>
        /// <returns></returns>
        int GetPosItemActionTotal(string hid, string itemid);

        /// <summary>
        /// 根据作法ID获取消费项目对应作法
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="actionId">作法ID</param>
        /// <returns></returns>
        PosItemAction GetPosItemActionListByActionId(string hid, string actionId);

        /// <summary>
        /// 根据酒店和ID获取消费项目对应作法
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="Id">ID</param>
        /// <returns></returns>
        PosItemAction GetPosItemActionListById(string hid, Guid id);
    }
}