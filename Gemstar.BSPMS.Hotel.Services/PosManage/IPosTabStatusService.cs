using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos餐台状态服务接口
    /// </summary>
    public interface IPosTabStatusService : ICRUDService<PosTabStatus>
    {
        /// <summary>
        /// 判断指定的代码或者名称的餐台状态是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">餐台状态代码</param>
        /// <param name="name">餐台状态名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的餐台状态了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name);
        /// <summary>
        /// 判断指定的代码或者名称的餐台状态是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">餐台状态代码</param>
        /// <param name="name">餐台状态名称</param>
        /// <param name="exceptId">要排队的餐台状态id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的餐台状态了，false：没有相同的</returns>
        bool IsExists(string hid, string code, string name, string exceptId);
        /// <summary>
        /// 根据查询条件获取指定酒店下的餐台状态总数
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">台号或台名</param>
        /// <param name="refeid">营业点id、代码或名称</param>
        /// <param name="tabtypeid">餐台类型id、代码或名称</param>
        /// <param name="tabStatus">餐台状态</param>
        /// <returns></returns>
        int GetPosTabStatusTotal(string hid, string code, string refeid, string tabtypeid, byte? tabStatus);
        /// <summary>
        /// 根据查询条件获取指定酒店下的餐台状态列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">台号或台名</param>
        /// <param name="refeid">营业点id、代码或名称</param>
        /// <param name="tabtypeid">餐台类型id、代码或名称</param>
        /// <param name="tabStatus">餐台状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        List<PosTabStatus> GetPosTabStatus(string hid, string code, string refeid, string tabtypeid, byte? tabStatus, int pageIndex, int pageSize);
        /// <summary>
        /// 根据酒店、营业点、餐台设置餐台状态
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台id</param>
        /// <param name="status">要设置的状态</param>
        /// <returns></returns>
        PosTabStatus GetPosTabStatus(string hid, string refeid, string tabid);
        /// <summary>
        /// 设置餐台状态
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="opType">操作代码</param>
        /// <param name="ids"></param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        void SetTabStatus(string hid, string refeid, byte opType, string ids, string beginDate, string endDate);

        /// <summary>
        /// 根据酒店、收银点、营业点、餐台类型获取开台统计数据
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabtypeid">餐台类型ID</param>
        /// <returns></returns>
        up_pos_cmp_tabStatusResult GetPosTabStatusStatistics(string hid, string posid, string refeid, string tabtypeid);

        /// <summary>
        /// 根据查询条件获取指定酒店下的餐台状态列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">台号或台名</param>
        /// <param name="refeid">营业点id、代码或名称</param>
        /// <param name="tabtypeid">餐台类型id、代码或名称</param>
        /// <param name="tabStatus">餐台状态</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        List<up_pos_list_TabStatusResult> GetPosTabStatusResult(string hid, string code, string refeid, string tabtypeid, string tabStatus, int pageIndex, int pageSize);

        /// <summary>
        /// 根据查询条件获取指定酒店下的餐台状态列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">台号或台名</param>
        /// <param name="refeid">营业点id、代码或名称</param>
        /// <param name="tabtypeid">餐台类型id、代码或名称</param>
        /// <param name="tabStatus">餐台状态</param>
        /// <returns></returns>
        int GetPosTabStatusResultTotal(string hid, string code, string refeid, string tabtypeid, string tabStatus);

        /// <summary>
        /// 设置餐台预定状态
        /// </summary>
        /// <param name="hid"></param>
        void SetTabReserveStatus(string hid);
    }
}