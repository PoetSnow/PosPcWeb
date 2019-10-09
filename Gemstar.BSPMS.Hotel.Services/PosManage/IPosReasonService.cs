using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos原因服务接口
    /// </summary>
    public interface IPosReasonService : ICRUDService<PosReason>
    {
        /// <summary>
        /// 判断指定的代码或者名称的原因是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">原因代码</param>
        /// <param name="name">原因名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的原因了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);

        /// <summary>
        /// 判断指定的代码或者名称的原因是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">原因代码</param>
        /// <param name="name">原因名称</param>
        /// <param name="exceptId">要排队的原因id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的原因了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);

        /// <summary>
        /// 获取指定酒店和模块下的原因列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的原因列表</returns>
        List<PosReason> GetPosReasonByModule(string hid, string moduleCode, byte istagtype, int pageIndex, int pageSize);

        /// <summary>
        /// 获取指定酒店和模块下的原因列表总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的原因列表</returns>
        int GetPosReasonByModuleTotal(string hid, string moduleCode, byte istagtype);

        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);
    }
}