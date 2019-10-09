using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos作法分类服务接口
    /// </summary>
    public interface IPosActionTypeService : ICRUDService<PosActionType>
    {
        /// <summary>
        /// 判断指定的代码或者名称的作法分类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">作法分类代码</param>
        /// <param name="name">作法分类名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的作法分类了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);
        /// <summary>
        /// 判断指定的代码或者名称的作法分类是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">作法分类代码</param>
        /// <param name="name">作法分类名称</param>
        /// <param name="exceptId">要排队的作法分类id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的作法分类了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);
        /// <summary>
        /// 获取指定酒店和模块下的作法分类列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的作法分类列表</returns>
        List<PosActionType> GetActionTypeByModule(string hid, string moduleCode);

        /// <summary>
        /// 获取指定酒店和模块下的要求列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的要求列表</returns>
        List<PosActionType> GetPosActionTypeByModule(string hid, string moduleCode, int pageIndex, int pageSize);

        /// <summary>
        /// 获取指定酒店和模块下的要求列表总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的要求列表</returns>
        int GetPosActionTypeByModuleTotal(string hid, string moduleCode);

        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);
    }
}