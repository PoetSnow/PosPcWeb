using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos餐台服务接口
    /// </summary>
    public interface IPosTabService : ICRUDService<PosTab>
    {
        /// <summary>
        /// 判断指定的代码或者名称的餐台是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="tabNo">台号</param>
        /// <param name="cname">餐台名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的餐台了，false：没有相同的</returns>
        bool IsExists(string hid, string tabNo, string cname);
        /// <summary>
        /// 判断指定的代码或者名称的餐台是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">台号</param>
        /// <param name="cname">餐台名称</param>
        /// <param name="exceptId">要排队的餐台id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的餐台了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string cname, string exceptId);
        /// <summary>
        /// 根据酒店ID和餐台ID获取餐台信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">餐台ID</param>
        /// <returns></returns>
        PosTab GetEntity(string hid, string id);
        /// <summary>
        /// 根据酒店ID和模块编码获取餐台信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="moduleCode">模块编码</param>
        /// <returns></returns>
        List<PosTab> GetPosTabByModule(string hid, string moduleCode);


        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);

        /// <summary>
        /// 根据酒店查询扫码餐台列表
        /// </summary>
        /// <param name="hid">酒店</param>
        /// <param name="code">代码</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        List<up_pos_scan_list_TabListByHidResult> GetPosTabByHid(string hid, string code, string name);

        /// <summary>
        /// 根据酒店id、营业点id获取餐台列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeid"></param>
        /// <returns></returns>
        List<PosTab> GetEntityByRefeId(string hid, string refeid);
    }
}
