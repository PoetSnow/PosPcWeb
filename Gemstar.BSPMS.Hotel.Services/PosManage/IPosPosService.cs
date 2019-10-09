using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos收银点服务接口
    /// </summary>
    public interface IPosPosService : ICRUDService<PosPos>
    {
        /// <summary>
        /// 判断指定Id的收银点是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="id">收银点Id</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的收银点了，false：没有相同的</returns>
        bool IsExists(string hid, string id);

        /// <summary>
        /// 判断指定的代码或者名称的收银点是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">收银点代码</param>
        /// <param name="name">收银点名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的收银点了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);

        /// <summary>
        /// 判断指定的代码或者名称的收银点是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">收银点代码</param>
        /// <param name="name">收银点名称</param>
        /// <param name="exceptId">要排队的收银点id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的收银点了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);

        /// <summary>
        /// 获取指定酒店下的收银点列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的收银点列表</returns>
        List<PosPos> GetPosByHid(string hid);

        /// <summary>
        /// 获取指定酒店下的收银点列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>指定酒店和模块下的收银点列表</returns>
        List<PosPos> GetPosByHids(string hids);

        /// <summary>
        /// 获取指定酒店下的收银点
        /// </summary>
        /// <param name="hid">酒店id</param>
        ///  <param name="id">收银点id</param>
        /// <returns>指定酒店和模块下的收银点</returns>
        PosPos GetPosByHid(string hid, string id);

        /// <summary>
        /// 获取指定酒店和模块下的收银点列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="moduleCode">模块代码</param>
        /// <returns>指定酒店和模块下的收银点列表</returns>
        List<PosPos> GetPosByModule(string hid, string moduleCode);

        /// <summary>
        /// 根据酒店和收银点查询需要更换的班次信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <returns></returns>
        up_pos_query_shiftChangeResult GetShiftChange(string hid, string posid);

        /// <summary>
        /// 根据酒店和收银点获取清机的信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <returns></returns>
        up_pos_query_cleaningMachineResult GetCleaningMachine(string hid, string posid);

        /// <summary>
        /// 用存储过程实现清机
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="posId">收银点ID</param>
        /// <param name="errFlag">是否提示错误信息（0,：不执行，1：执行）</param>
        void CleaningMachineBusiness(string hid,string posId,string errFlag="1");


        /// <summary>
        /// 启用，禁用
        /// </summary>
        /// <param name="ids">主键ID</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        JsonResultData BatchUpdateStatus(string ids, EntityStatus status);
    }
}
