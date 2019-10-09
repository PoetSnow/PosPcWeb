using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos账单服务接口
    /// </summary>
    public interface IPosBillService : ICRUDService<PosBill>
    {
        /// <summary>
        /// 判断指定单号的账单是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="billid">单号</param>
        /// <returns>true:在酒店中已经有相同单号或者相同客账号的账单了，false：没有相同的</returns>
        bool IsExists(string hid, string refeid, string billid);

        /// <summary>
        /// 判断指定单号的账单是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="billid">单号</param>
        /// <param name="exceptId">要排队的单号</param>
        /// <returns>true:在酒店中已经有相同单号或者相同客账号的账单了，false：没有相同的</returns>
        bool IsExists(string hid, string refeid, string billid, string exceptId);

        /// <summary>
        /// 根据酒店、餐台id、营业日获取当前餐台是否为抹台
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="tabid">餐台Id</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <returns></returns>
        List<PosBill> GetSmearList(string hid, string tabid, DateTime? billBsnsDate);

        /// <summary>
        /// 根据酒店、营业点、营业日判断是否有未清台的账单
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeids">营业点</param>
        /// <param name="business">营业日</param>
        /// <returns></returns>
        bool IsNotPaid(string hid, string refeids, DateTime business);

        /// <summary>
        /// 根据酒店、营业点、营业日生成新的流水号
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="billBsnsDate">当前营业日</param>
        /// <returns></returns>
        PosBill GetLastBillId(string hid, string refeid, DateTime? billBsnsDate);

        /// <summary>
        /// 根据酒店、账单ID获取账单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <returns></returns>
        PosBill GetEntity(string hid, string billid);

        /// <summary>
        /// 根据酒店、营业点、餐台获取账单信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台id</param>
        /// <param name="billid">账单id</param>
        /// <returns></returns>
        up_pos_list_billByRefeidAndTabidResult GetPosBillByTabId(string hid, string refeid, string tabid, string billid = "");

        /// <summary>
        /// 根据酒店、收银点、营业日获取收银账单列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="tabNo">台号</param>
        /// <returns></returns>
        List<up_pos_list_billByPosidResult> GetPosBillByPosid(string hid, string posid, DateTime? billBsnsDate, string tabNo);

        /// <summary>
        /// 根据酒店、收银点、营业日查询账单反结列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="tabNo">台号</param>
        /// <returns></returns>
        List<up_pos_list_billByPosidResult> GetPosBillReverseCheckout(string hid, string posid, DateTime? billBsnsDate, string tabNo);

        /// <summary>
        /// 根据酒店、收银点、营业日获取迟付结账列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="tabNo">台号</param>
        /// <returns></returns>
        List<up_pos_list_billByPosidResult> GetPosBillDelayedPayment(string hid, string posid, DateTime? billBsnsDate, string tabNo);

        /// <summary>
        /// 根据酒店、收银点、营业日获取客账账单列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="tabNo">台号</param>
        /// <returns></returns>
        List<up_pos_list_billByPosidResult> GetPosBillGuestQuery(string hid, string posid, DateTime? billBsnsDate, string tabNo);

        /// <summary>
        /// 根据酒店、账单ID获取账单信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <returns>账单信息</returns>
        up_pos_list_billByBillidResult GetPosBillByBillid(string hid, string billid);

        /// <summary>
        /// 根据酒店、营业点、登记营业日获取新的快餐台号
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="tabFlag">餐台标识(0：物理台，1：快餐台)</param>
        /// <returns>快餐台号</returns>
        string GetTakeoutForTabid(string hid, string refeid, DateTime? billBsnsDate, byte? tabFlag);

        /// <summary>
        /// 更换餐台资料
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="oldbillId">原账单ID</param>
        /// <param name="oldTabId">原餐台ID</param>
        /// <param name="newServiceRate">服务费率</param>
        /// <param name="newLimit">最低消费</param>
        /// <param name="ServiceRateFlag">服务费选择（1：原台  2：新台）</param>
        /// <param name="ItemFlag">消费项目选择 （1：原台  2：新台）</param>
        void ChangeTab(string hid, string oldbillId, string oldTabId, decimal? newServiceRate, decimal? newLimit, string ServiceRateFlag, string ItemFlag, string Newtabid);

        /// <summary>
        /// 根据酒店ID，营业点ID，营业日期获取所有的账单数据
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeId"></param>
        /// <param name="billBsnsDate"></param>
        /// <returns></returns>
        List<PosBill> GetBillListForPosRefe(string hid, string refeId, DateTime? billBsnsDate);

        /// <summary>
        /// 根据酒店ID与餐台ID获取开台状态的账单列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="tabid"></param>
        /// <returns></returns>
        List<PosBill> GetSmearListByClearTab(string hid, string tabid);

        /// <summary>
        /// 客账查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        List<up_pos_list_billByPosidResult> GetPosBillGuestQuery(QueryBillModel model);


        /// <summary>
        /// 根据账单ID 获取账单信息
        /// </summary>
        /// <param name="billid">账单ID</param>
        /// <param name="hid">酒店代码</param>
        /// <returns></returns>
        PosBill GetBillByCancalLockTab(string billid, string hid);
        //获取账单的消费项目的所有消费项目大类
        /// <summary>
        /// 根据账单获取账单消费项目详情的消费大类
        /// </summary>
        /// <param name="posBill"></param>
        /// <returns></returns>
        List<up_pos_queryBillDetailPositemResult> GetPosItemByPosBill(string hid, PosBill posBill);


        /// <summary>
        /// 退出餐台删除账单数据以及修改餐台状态
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billId">账单ID</param>
        /// <param name="tabId">餐台ID</param>
        void DeletePosBillByCCancelLockTab(string hid, string billId, string tabId);

        /// <summary>
        /// 根据酒店ID以及openID 获取账单
        /// </summary>
        /// <param name="hid">酒店代码</param>
        ///  <param name="tabId"></param>
        /// <param name="openWxid"></param>
        /// <returns></returns>
        PosBill GetPosBillByScanOrder(string hid, string tabId, string openWxid);

        /// <summary>
        /// 根据酒店ID以及餐台ID 获取账单
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="tabId"></param>
        /// <returns></returns>
        PosBill GetPosbillByScanOrder(string hid, string tabId);

        /// <summary>
        /// 根据酒店、营业点、餐台获取非迟付和结账的账单信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台id</param>
        /// <param name="billid">账单id</param>
        /// <returns></returns>
        up_pos_list_billByRefeidAndTabidResult GetPosBillByTabIdForNotDelayed(string hid, string refeid, string tabid, string billid = "");

        /// <summary>
        /// 根据餐台id查找最新的账单
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeid"></param>
        /// <param name="tabid"></param>
        /// <returns></returns>
        PosBill GetPosBillByTabid(string hid, string refeid, string tabid);

        /// <summary>
        /// 根据酒店ID，营业点ID，营业日期获取所有的账单数据
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeId"></param>
        /// <param name="billBsnsDate"></param>
        /// <returns></returns>
        List<PosBill> GetAllBillListForPosRefe(string hid, string refeId, DateTime? billBsnsDate);
    }
}