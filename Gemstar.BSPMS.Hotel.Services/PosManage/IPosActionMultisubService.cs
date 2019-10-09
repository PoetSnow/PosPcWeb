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
    public interface IPosActionMultisubService : ICRUDService<PosActionMultisub>
    {
        /// <summary>
        /// 判断指定的代码或者名称的同组作法是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="actionid">作法id</param>
        /// <param name="actionid2">同组作法id</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的同组作法了，false：没有相同的</returns>
        bool IsExists(string hid, string actionid, string actionid2);

        /// <summary>
        /// 判断指定的代码或者名称的同组作法是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="actionid">作法id</param>
        /// <param name="actionid2">同组作法id</param>
        /// <param name="exceptId">要排队的同组作法id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的同组作法了，false：没有相同的</returns>
        bool IsExists(string hid, string actionid, string actionid2, Guid exceptId);

        /// <summary>
        /// 获取指定酒店和模块下的同组作法列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="actionid">模块代码</param>
        /// <returns>指定酒店和模块下的同组作法列表</returns>
        List<PosActionMultisub> GetPosItemClassByModule(string hid, string actionid);

        /// <summary>
        /// 获取指定酒店和作法下的单位价格列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="actionid">作法Id</param>
        /// <returns>指定酒店和作法下的同组作法列表</returns>
        List<up_pos_list_ActionMultisubByActionidResult> GetPosActionMultisubByactionid(string hid, string actionid);

        /// <summary>
        /// 根据消费项目获取消费项目对应作法
        /// </summary>
        /// <param name="hid">酒店</param>
        /// <param name="actionid">作法Id</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        List<up_pos_list_ActionMultisubByActionidResult> GetPosActionMultisubByactionid(string hid, string actionid, int pageIndex, int pageSize);

        /// <summary>
        /// 获取指定酒店和模块下的同组作法列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="actionid">模块代码</param>
        /// <returns>指定酒店和模块下的同组作法列表</returns>

        List<PosActionMultisub> GetPosActionMultisubByactionidForCopy(string hid, string actionid);
    }
}