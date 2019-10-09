using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos餐台类型服务接口
    /// </summary>
    public interface IPosTabtypeService : ICRUDService<PosTabtype>
    {
        /// <summary>
        /// 判断指定的代码或者名称的餐台类型是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">餐台类型代码</param>
        /// <param name="name">餐台类型名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的餐台类型了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);
        /// <summary>
        /// 判断指定的代码或者名称的餐台类型是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">餐台类型代码</param>
        /// <param name="name">餐台类型名称</param>
        /// <param name="exceptId">要排队的餐台类型id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的餐台类型了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);
        /// <summary>
        /// 获取指定酒店和模块下的餐台类型列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店下的餐台类型列表</returns>
        List<PosTabtype> GetTabtype(string hid);
        /// <summary>
        /// 获取指定酒店和模块下的餐台类型列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的餐台类型列表</returns>
        List<PosTabtype> GetTabtypeByModule(string hid, string moduleCode);

        /// <summary>
        /// 根据酒店ID，模块，营业点ID获取餐台类型列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="moduleCode"></param>
        /// <param name="refeId">营业点ID</param>
        /// <returns></returns>
        List<PosTabtype> GetTabtypeByModuleOrRefe(string hid, string moduleCode, string refeId,string posId);


        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);

        /// <summary>
        /// 根据酒店ID和餐台类型ID获取餐台类型信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">餐台类型ID</param>
        /// <returns></returns>
        PosTabtype GetEntity(string hid, string id);
    }
}