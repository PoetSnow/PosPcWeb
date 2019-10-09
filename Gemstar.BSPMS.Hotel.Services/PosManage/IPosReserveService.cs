using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// 预定接口
    /// </summary>
    public interface IPosReserveService : ICRUDService<PosBill>
    {
        /// <summary>
        /// 获取预定餐台状态
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="tabTypeId">餐台类型</param>
        /// <param name="TimeList">时间段（营业点市别的最早时间到最晚时间）</param>
        /// <param name="Business">营业日</param>
        /// <param name="Flag">类型（1：预定日期，2：预抵日期）</param>
        /// <returns></returns>
        List<up_pos_ReserveTabStatusResult> GetReserveTabStatus(string hid, string refeId, string tabTypeId, string TimeList, DateTime Business, string Flag);


        /// <summary>
        /// 统计餐台类型该时间段可用的餐台数量
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="module">模块</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="Business">时间</param>
        /// <returns></returns>
        List<up_pos_ReserveTabTypeListResult> GetReserveTabTypeInfo(string hid, string module, string refeId, DateTime ReserveDate);

        /// <summary>
        /// 根据餐台类型获取该时段所有餐台信息
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="module">模块</param>
        /// <param name="refeId">营业点Id</param>
        /// <param name="ReserveDate">预抵日期</param>
        /// <param name="tabTypeId">餐台类型Id</param>
        /// <returns></returns>
        List<PosTab> GetTabListByTabTypeId(string hid, string module, string refeId, DateTime ReserveDate, string tabTypeId);


        /// <summary>
        /// 获取餐台所有的预定账单
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="tabId">餐台Id</param>
        /// <returns></returns>
        List<PosBill> GetOrderBillByTabId(string hid, string tabId);

        /// <summary>
        /// 根据日期获取所有的预订账单
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="tabId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        List<PosBill> GetOrderBillByTabId(string hid, string tabId, DateTime date);

        /// <summary>
        /// 查询日期下所有的订单列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="refeId">营业点Id</param>
        /// <param name="business">日期</param>
        /// <param name="tabTypeId">餐台类型</param>
        /// <param name="Flag">日期类型</param>
        /// <param name="status">账单状态</param>
        /// <returns></returns>
        List<up_pos_OrderBillListByDateResult> GetOrderBillList(string hid, string refeId, DateTime business, string tabTypeId, string Flag, string status);


        /// <summary>
        /// 获取预订账单信息
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billId">账单Id</param>
        /// <returns></returns>
        PosBill GetBillOrder(string hid, string billId);

        /// <summary>
        /// 获取预定账单信息
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="tabId">餐台Id</param>
        /// <param name="business">预订日期</param>
        /// <param name="refeId">营业点Id</param>
        /// <returns></returns>
        PosBill GetBillOrder(string hid, string tabId, DateTime stime, DateTime eTime, string refeId);
    }
}
