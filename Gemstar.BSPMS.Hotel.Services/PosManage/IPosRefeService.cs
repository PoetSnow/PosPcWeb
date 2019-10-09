using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// 营业点服务接口
    /// </summary>
    public interface IPosRefeService : ICRUDService<PosRefe>
    {
        /// <summary>
        /// 判断指定的代码或者名称的营业厅是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">营业厅代码</param>
        /// <param name="name">营业厅名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的营业厅了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);

        /// <summary>
        /// 判断指定的代码或者名称的营业厅是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">营业厅代码</param>
        /// <param name="name">营业厅名称</param>
        /// <param name="exceptId">要排队的营业厅id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的营业厅了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);

        /// <summary>
        /// 获取指定酒店和营业点的营业点信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="id">营业点id</param>
        /// <returns></returns>
        PosRefe GetEntity(string hid, string id);

        /// <summary>
        /// 获取指定酒店下的营业厅列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的营业厅列表</returns>
        List<PosRefe> GetRefe(string hid);

        /// <summary>
        /// 获取指定酒店和收银点下的营业厅列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <returns>指定酒店和收银点下的营业厅列表</returns>
        List<PosRefe> GetRefeByPosid(string hid, string posid);

        /// <summary>
        /// 指定酒店、收银点和模块下的营业厅列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店、收银点和模块下的营业厅列表</returns>
        List<PosRefe> GetRefeByPosAndModule(string hid, string posid, string moduleCode);

        /// <summary>
        /// 获取指定酒店和模块下的营业厅列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的营业厅列表</returns>
        List<PosRefe> GetRefeByModule(string hid, string moduleCode);

        /// <summary>
        /// 根据指定酒店、收银点、模块获取营业点
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns></returns>
        List<PosRefe> GetRefeByPos(string hid, string posid, string moduleCode);


        /// <summary>
        /// 根据酒店ID，编码判断营业点是否存在
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
