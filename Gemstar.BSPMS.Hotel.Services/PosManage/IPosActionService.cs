using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos作法基础资料服务接口
    /// </summary>
    public interface IPosActionService : ICRUDService<PosAction>
    {
        /// <summary>
        /// 判断指定的代码或者名称的作法基础资料是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">作法基础资料代码</param>
        /// <param name="name">作法基础资料名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的作法基础资料了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);

        /// <summary>
        /// 判断指定的代码或者名称的作法基础资料是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">作法基础资料代码</param>
        /// <param name="name">作法基础资料名称</param>
        /// <param name="exceptId">要排队的作法基础资料id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的作法基础资料了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);

        /// <summary>
        /// 获取指定酒店和模块下的作法列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的作法列表</returns>
        List<PosAction> GetPosActionByModule(string hid, string moduleCode);

        /// <summary>
        /// 获取指定酒店和模块下的作法基础资料列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的作法基础资料列表</returns>
        List<PosAction> GetActionByModule(string hid, string moduleCode);

        /// <summary>
        /// 获取指定酒店和模块、类型下的作法基础资料列表
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="moduleCode">模板代码</param>
        /// <param name="actionTypeId">作法类型Id</param>
        /// <returns>指定酒店和模块下的作法基础资料列表</returns>
        List<PosAction> GetActionByModuleAndType(string hid, string moduleCode, string actionTypeId, int pageIndex, int pageSize);

        /// <summary>
        /// 获取指定酒店和模块、类型下的作法基础资料页索引
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="moduleCode">模板代码</param>
        /// <param name="actionTypeId">作法类型Id</param>
        /// <param name="actionId">作法Id</param>
        /// <returns>指定酒店和模块下的作法基础资料列表</returns>
        int GetActionPageIndex(string hid, string moduleCode, string actionTypeId, string actionId, int pageSize);

        /// <summary>
        /// 获取指定酒店和模块下的要求列表总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <param name="actionTypeId">作法类型Id</param>
        /// <returns>指定酒店和模块下的要求列表</returns>
        int GetActionByModuleTotal(string hid, string moduleCode, string actionTypeId);

        /// <summary>
        /// 根据酒店id、代码和名称获取作法
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">代码</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        PosAction GetActionByCode(string hid, string code, string name);

        /// <summary>
        /// 根据酒店ID、作法ID获取作法
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="actionid">作法ID</param>
        /// <returns></returns>
        PosAction GetActionByID(string hid, string actionid);

        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);
    }
}