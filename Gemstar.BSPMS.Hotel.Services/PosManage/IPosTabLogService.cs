using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos锁台服务接口
    /// </summary>
    public interface IPosTabLogService : ICRUDService<PosTabLog>
    {
        /// <summary>
        /// 判断指定的餐台id或者台号的锁台是否已经存在，用于防止餐台id和台号重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台id</param>
        /// <param name="tabNo">台号</param>
        /// <returns>true:在酒店中已经有相同餐台id或者相同台号的锁台了，false：没有相同的</returns>
        bool IsExists(string hid, string refeid, string tabid, string tabNo);

        /// <summary>
        /// 获取指定酒店、营业点、餐台id或台号下的锁定信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台id</param>
        /// <param name="tabNo">台号</param>
        /// <returns>锁台信息</returns>
        PosTabLog GetPosTabLogByTab(string hid, string refeid, string tabid, string tabNo);

        /// <summary>
        /// 获取指定酒店、营业点、餐台id或台号下的锁定信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台id</param>
        /// <param name="tabNo">台号</param>
        /// <returns>锁台信息</returns>
        List<PosTabLog> GetPosTabLogListByTab(string hid, string refeid, string tabid, string tabNo);

        /// <summary>
        /// 根据指定酒店、营业点、计算机和操作员获取锁定信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="computer">计算机</param>
        /// <param name="transUser">操作员</param>
        /// <returns></returns>
        List<PosTabLog> GetPosTabLogListByUser(string hid, string refeid, string computer, string transUser);

        /// <summary>
        /// 根据酒店ID与营业点ID获取
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeid"></param>
        /// <returns></returns>
        List<up_pos_listTabLogResult> GetPosTabLogListByRefeId(string hid, string refeid);

        /// <summary>
        /// 根据账单ID获取锁台记录
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        PosTabLog GetPosTabLogByBillId(string hid,string billId);


    }
}