using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos折扣类型服务接口
    /// </summary>
    public interface IPosDiscTypeService : ICRUDService<PosDiscType>
    {
        /// <summary>
        /// 判断指定的代码或者名称的折扣类型是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">折扣类型代码</param>
        /// <param name="name">折扣类型名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的折扣类型了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);
        /// <summary>
        /// 判断指定的代码或者名称的折扣类型是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">折扣类型代码</param>
        /// <param name="name">折扣类型名称</param>
        /// <param name="exceptId">要排队的折扣类型id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的折扣类型了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);
        /// <summary>
        /// 获取指定酒店和模块下的折扣类型列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的折扣类型列表</returns>
        List<PosDiscType> GetDiscTypeByModule(string hid, string moduleCode);

        /// <summary>
        /// 获取指定酒店和模块下的折扣类型列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的折扣类型列表</returns>
        List<up_pos_discTypeList> GetPosDiscTypeByModule(string hid, string moduleCode, string discType, int pageIndex, int pageSize);

        /// <summary>
        /// 获取指定酒店和模块下的折扣类型列表总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的折扣类型列表</returns>
        int GetPosDiscTypeByModuleTotal(string hid, string moduleCode,string discType);
    }
}