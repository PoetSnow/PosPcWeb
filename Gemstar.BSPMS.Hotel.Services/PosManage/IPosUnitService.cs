using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos单位定义服务接口
    /// </summary>
    public interface IPosUnitService : ICRUDService<PosUnit>
    {
        /// <summary>
        /// 判断指定的代码或者名称的单位定义是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">单位定义代码</param>
        /// <param name="name">单位定义名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的单位定义了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);
        /// <summary>
        /// 判断指定的代码或者名称的单位定义是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">单位定义代码</param>
        /// <param name="name">单位定义名称</param>
        /// <param name="exceptId">要排队的单位定义id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的单位定义了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);
        /// <summary>
        /// 获取指定酒店和模块下的单位定义列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店下的单位定义列表</returns>
        List<PosUnit> GetUnit(string hid);
        /// <summary>
        /// 根据酒店和单位ID获取单位
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="unitid"></param>
        /// <returns></returns>
        PosUnit GetEntity(string hid, string unitid);
        /// <summary>
        /// 获取指定酒店和模块下的单位定义列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的单位定义列表</returns>
        List<PosUnit> GetUnitByModule(string hid, string moduleCode);

  
           

    }
}