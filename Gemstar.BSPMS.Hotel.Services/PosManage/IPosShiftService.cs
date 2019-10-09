using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos班次服务接口
    /// </summary>
    public interface IPosShiftService : ICRUDService<PosShift>
    {
        /// <summary>
        /// 判断指定的代码或者名称的班次是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">班次代码</param>
        /// <param name="name">班次名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的班次了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);
        /// <summary>
        /// 判断指定的代码或者名称的班次是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">班次代码</param>
        /// <param name="name">班次名称</param>
        /// <param name="exceptId">要排队的班次id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的班次了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);
        /// <summary>
        /// 根据酒店和班次id获取班次
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="id">班次id</param>
        /// <returns></returns>
        PosShift GetEntity(string hid, string id);
        /// <summary>
        /// 根据酒店、收银点和模块获取班次列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns></returns>
        List<PosShift> GetPosShiftList(string hid, string posid, string moduleCode);
        
        /// <summary>
        /// 根据酒店、收银点和模块获取班次
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <returns></returns>
        PosShift GetPosShift(string hid, string posid, string moduleCode);
        
        /// <summary>
        /// 获取酒店所用班次
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<PosShift> GetShiftList(string hid);

       /// <summary>
       /// 根据酒店ID与
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
