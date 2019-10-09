using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos账单作法明细服务接口
    /// </summary>
    public interface IPosBillDetailActionService : ICRUDService<PosBillDetailAction>
    {
        /// <summary>
        /// 判断指定的作法是否存在
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="mid">账单明细id</param>
        /// <param name="actionNo">作法编码</param>
        /// <returns></returns>
        bool IsExists(string hid, string mBillid, long? mid, string actionNo);

        /// <summary>
        /// 获取指定酒店和模块下的账单作法明细列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="mBillid">模块代码</param>
        /// <returns>指定酒店和模块下的账单作法明细列表</returns>
        List<PosBillDetailAction> GetPosBillDetailActionByModule(string hid, string mBillid);

        /// <summary>
        /// 根据酒店、主单号、消费明细id获取作法分组id
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="mid">消费明细id</param>
        /// <returns></returns>
        int GetIgroupidByMid(string hid, string mBillid, long? mid);

        /// <summary>
        /// 根据酒店、主单号、消费明细id获取作法
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="mid">账单明细id</param>
        /// <param name="actionNo">作法编码</param>
        /// <returns></returns>
        PosBillDetailAction GetBillDetailActionByMid(string hid, string mBillid, long? mid, string actionNo,string actionType);

        /// <summary>
        /// 根据酒店、主单号、消费明细id获取作法列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="mid">账单明细id</param>
        /// <returns></returns>
        List<PosBillDetailAction> GetBillDetailActionByMid(string hid, string mBillid, long? mid);

        /// <summary>
        /// 根据酒店、主单号、消费明细id获取作法分组列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="mid">消费明细id</param>
        /// <returns></returns>
        List<up_pos_list_BillDetailActionGroupResult> GetBillDetailActionGroupByMid(string hid, string mBillid, long? mid);

        /// <summary>
        /// 根据酒店，主单，消费明细，分组信息获取作法列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="mid">消费明细ID</param>
        /// <param name="igroupid">分组</param>
        /// <param name="ActionTypeID">作法类别</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        List<PosAction> GetBillDetailAction(string hid, string mBillid, long? mid, int igroupid, string ActionTypeID, int pageIndex, int pageSize);
    }
}