using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos出品部门服务接口
    /// </summary>
    public interface IPosDepartService : ICRUDService<PosDepart>
    {
        /// <summary>
        /// 判断指定的代码或者名称的出品部门是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">出品部门代码</param>
        /// <param name="name">出品部门名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的出品部门了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);
        /// <summary>
        /// 判断指定的代码或者名称的出品部门是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">出品部门代码</param>
        /// <param name="name">出品部门名称</param>
        /// <param name="exceptId">要排队的出品部门id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的出品部门了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);
        /// <summary>
        /// 获取指定酒店和模块下的出品部门列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的出品部门列表</returns>
        List<PosDepart> GetDepartByModule(string hid, string moduleCode);

        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);

        /// <summary>
        /// 查询出品部门列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="moduleCode"></param>
        /// <returns></returns>
        List<up_pos_list_DepartResult> GetDepartByProc(string hid);



    }
}