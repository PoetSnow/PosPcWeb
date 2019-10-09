using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos市别服务接口
    /// </summary>
    public interface IPosShuffleService: ICRUDService<PosShuffle>
    {
        /// <summary>
        /// 判断指定的代码或者名称的市别是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">市别代码</param>
        /// <param name="name">市别名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的市别了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);

        /// <summary>
        /// 判断指定的代码或者名称的市别是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">市别代码</param>
        /// <param name="name">市别名称</param>
        /// <param name="exceptId">要排队的市别id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的市别了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);

        /// <summary>
        /// 根据酒店和市别id获取市别
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="id">市别id</param>
        /// <returns></returns>
        PosShuffle GetEntity(string hid, string id);

        /// <summary>
        /// 获取指定酒店下的市别列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店下的市别列表</returns>
        List<PosShuffle> GetPosShuffle(string hid);

        /// <summary>
        /// 获取指定酒店和模块下的市别列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的市别列表</returns>
        List<PosShuffle> GetPosShuffleByModule(string hid, string moduleCode);

        /// <summary>
        /// 根据酒店、营业点和模块获取班次列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns></returns>
        List<PosShuffle> GetPosShuffleList(string hid, string refeid, string module);

        /// <summary>
        /// 根据指定酒店、营业点获取更换市别信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <returns></returns>
        up_pos_query_shuffleChangeResult GetShuffleChange(string hid, string refeid);

        /// <summary>
        /// 根据酒店ID与code判断数据是否存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        bool IsExists(string hid, string code);

        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);
    }
}
