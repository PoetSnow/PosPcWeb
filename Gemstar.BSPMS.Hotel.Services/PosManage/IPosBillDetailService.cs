using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    /// <summary>
    /// pos账单明细服务接口
    /// </summary>
    public interface IPosBillDetailService : ICRUDService<PosBillDetail>
    {
        /// <summary>
        /// 根据酒店、账单ID、消费项目id判断当前明细是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="itemid">消费项目ID</param>
        /// <returns></returns>
        bool IsExists(string hid, string billid, string itemid);

        /// <summary>
        /// 根据酒店、账单ID、消费项目id判断当前明细是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="itemid">消费项目ID</param>
        /// <param name="itemName">消费项目名称</param>
        /// <returns></returns>
        bool IsExists(string hid, string billid, string itemid, string itemName);

        /// <summary>
        /// 根据酒店和账单ID获取账单明细
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <param name="dcFlag">付款标记（D：消费；C：付款）</param>
        /// <returns></returns>
        List<up_pos_list_BillDetailByBillidResult> GetUpBillDetailByBillid(string hid, string billid, string dcFlag = "");

        /// <summary>
        /// 根据酒店和账单ID获取账单收银明细
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <param name="dcFlag">付款标记（D：消费；C：付款）</param>
        /// <returns></returns>
        List<up_pos_list_BillDetailByCashierResult> GetUpBillDetailByCashier(string hid, string billid, string dcFlag = "");

        /// <summary>
        /// 根据酒店和账单明细ID获取应付金额
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <returns></returns>
        decimal GetBillDetailAmountSumByBillid(string hid, string billid);

        /// <summary>
        /// 根据酒店、账单ID、付款标识获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="dcFlag">付款标识（D：消费；C：付款）</param>
        /// <returns></returns>
        List<PosBillDetail> GetBillDetailByDcFlag(string hid, string billid, string dcFlag = "");

        /// <summary>
        /// 根据酒店、账单、结账交易号查询账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="settleTransno">结账交易号</param>
        /// <returns></returns>
        PosBillDetail GetBillDetailBySettleTransno(string hid, string billid, string settleTransno);

        /// <summary>
        /// 根据酒店、账单、套餐ID获取套餐明细
        /// </summary>
        /// <param name="hid">酒店</param>
        /// <param name="billid">账单</param>
        /// <param name="upid">套餐ID</param>
        /// <returns></returns>
        List<PosBillDetail> GetBillDetailByUpid(string hid, string billid, Guid? upid);

        /// <summary>
        /// 根据酒店结账ID获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单id</param>
        /// <param name="settleid">结账ID</param>
        /// <returns></returns>
        PosBillDetail GetEntity(string hid, string billid, Guid? settleid);

        /// <summary>
        /// 根据酒店结账ID和自动标志获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单id</param>
        /// <param name="isauto">自动标志</param>
        /// <returns></returns>
        PosBillDetail GetEntity(string hid, string billid, byte? isauto);

        /// <summary>
        /// 根据酒店明细ID获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">明细ID</param>
        /// <returns></returns>
        PosBillDetail GetEntity(string hid, long id);

        /// <summary>
        /// 根据酒店、账单、项目、单位获取账单明细
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <param name="itemid">消费项目id</param>
        /// <returns></returns>
        PosBillDetail GetBillDetailByBillid(string hid, string billid, string itemid);

        /// <summary>
        /// 根据酒店、账单、消费项目获取账单明细
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <param name="itemid">消费项目id</param>
        /// <param name="itemName">消费项目id</param>
        /// <returns></returns>
        PosBillDetail GetBillDetailByBillid(string hid, string billid, string itemid, string itemName);

        /// <summary>
        /// 根据酒店、账单ID、付款标识、计费状态获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="dcFlag">付款标记（D：消费；C：付款）</param>
        /// <param name = "status" >计费状态</ param >
        /// <returns></returns>
        List<PosBillDetail> GetBillDetailByBillid(string hid, string billid, string dcFlag, byte status);

        /// <summary>
        /// 根据酒店、账单ID、付款标识、计费状态获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="dcFlag">付款标记（D：消费；C：付款）</param>
        /// <param name = "status" >计费状态</ param >
        /// <returns></returns>
        List<PosBillDetail> GetBillDetailByBillidAndStatus(string hid, string billid, string dcFlag, byte status);

        /// <summary>
        /// 根据酒店、账单ID、付款标识获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="dcFlag">付款标识</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        List<PosBillDetail> GetBillDetailByBillid(string hid, string billid, string dcFlag, int pageIndex, int pageSize);

        /// <summary>
        /// 根据酒店和账单查询付款明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <returns></returns>
        List<up_pos_list_BillDetailForPaymentResult> GetBillDetailForPaymentByBillid(string hid, string billid);

        /// <summary>
        /// 根据酒店和账单查询付款金额合计
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <returns></returns>
        up_pos_cmp_PaymentTotalResult GetBillDetailForPaymentTotalByBillid(string hid, string billid);

        /// <summary>
        /// 根据酒店、账单和付款标记获取新的流水号
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="dcFlag">付款标记（D：消费；C：付款）</param>
        /// <returns></returns>
        string GetNewBatchTimeByBillid(string hid, string billid, string dcFlag);

        /// <summary>
        /// 根据酒店id、单号、主单号统计账单明细数据
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">单号</param>
        /// <param name="mBillid">主单号</param>
        void StatisticsBillDetail(string hid, string billid, string mBillid);

        /// <summary>
        /// 根据酒店id、单号、主单号统计账单明细数据
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">单号</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="isave">1：点菜；2：落单</param>
        up_pos_cmp_billDetailStatisticsResult GetBillDetailStatistics(string hid, string billid, string mBillid, int isave);

        /// <summary>
        /// 获取尾数处理后的金额
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        decimal GetAmountByBillTailProcessing(string hid, string refeid, decimal amount);

        /// <summary>
        /// 捷信达外部接口
        /// </summary>
        /// <param name="grpid">酒店id</param>
        /// <param name="channelCode">渠道代码</param>
        /// <param name="xmlStr">业务xml</param>
        /// <returns></returns>
        string RealOperate(string grpid, string channelCode, string xmlStr);

        /// <summary>
        /// 根据酒店ID，账单ID查询对应分组的账单信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billId">账单ID</param>
        /// <param name="Flag">类别 A：部门类别 B：大类 C：项目分类</param>
        /// <param name="dcFlag">付款标记（D：消费,C：付款）</param>
        /// <returns></returns>
        List<up_pos_list_BillDetailByGroupItemClassResult> GetBillDetailGroupItemClass(string hid, string billId, string Flag, string dcFlag);

        /// <summary>
        /// 根据酒店、账单ID、消费项目id判断当前明细是否存在落单的项目
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="itemid">项目ID</param>
        /// <returns></returns>
        bool IsExistsForLD(string hid, string billid, string itemid);

        /// <summary>
        /// 根据酒店、账单ID、消费项目id判断当前明细是否存在落单的项目
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="itemid">项目ID</param>
        ///  <param name="itemName">项目名称</param>
        /// <returns></returns>
        bool IsExistsForLD(string hid, string billid, string itemid, string itemName);

        /// <summary>
        /// 根据酒店、账单、项目、单位获取没有落单的账单明细
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <param name="itemid">消费项目id</param>
        /// <returns></returns>
        PosBillDetail GetBillDetailByBillidForLD(string hid, string billid, string itemid);

        /// <summary>
        /// 根据酒店、账单、项目、单位获取没有落单的账单明细
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <param name="itemid">消费项目id</param>
        /// <param name="itemName">消费项目名称</param>
        /// <returns></returns>
        PosBillDetail GetBillDetailByBillidForLD(string hid, string billid, string itemid, string itemName);

        /// <summary>
        /// 根据酒店ID，账单ID，消费项目获取账单明细
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billid"></param>
        /// <param name="dcFlag"></param>
        /// <returns></returns>
        List<up_pos_BillDetailResult> GetBillDetailByDcFlagForPosInSing(string hid, string billid, string dcFlag = "");

        /// <summary>
        /// 根据酒店id、单号、主单号获取账单明细统计数据
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">单号</param>
        /// <param name="mBillid">主单号</param>
        List<up_pos_print_billDetailResult> GetBillDetailByPrint(string hid, string billid, string mBillid);

        /// <summary>
        /// 根据酒店id、单号、主单号获取账单明细统计数据
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">单号</param>
        /// <param name="mBillid">主单号</param>
        List<up_pos_print_billOrderResult> GetBillOrderByPrint(string hid, string billid, string mBillid);

        /// <summary>
        /// 重算折扣金额
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billid"></param>
        /// <param name="mBillid"></param>
        void UpdateBillDetailDisc(string hid, string billid, string mBillid);

        /// <summary>
        /// 打印账单返回dataset
        /// </summary>
        /// <param name="sql">SQL 语句</param>
        /// <param name="para">参数</param>
        /// <returns></returns>
        DataSet GetDataSetByPayPrint(string sql, params SqlParameter[] para);

        /// <summary>
        /// 查询转菜数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="tabid">当前餐台ID</param>
        /// <param name="billDetailList">换台之后的账单明细ID集合</param>
        /// <returns></returns>
        List<up_pos_changeItemListResult> GetDataSetByChangeItem(string hid, string tabid, string billDetailList);

        /// <summary>
        /// 查询转台数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="tabid">当前餐台ID</param>
        /// <param name="billId">账单ID</param>
        /// <returns></returns>
        List<up_pos_ChangeTabBillDetailListResult> GetDataSetByChangeTab(string hid, string tabid, string billId);

        /// <summary>
        /// 获取特价菜
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="itemid">项目ID</param>
        /// <param name="Isauto"></param>
        /// <returns></returns>
        PosBillDetail GetBillDetailByBillidForLD(string hid, string billid, string itemid, PosBillDetailIsauto Isauto);

        /// <summary>
        /// 非特价菜
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="itemid">项目ID</param>
        /// <param name="Isauto"></param>
        /// <returns></returns>

        PosBillDetail GetBillDetailByBillidForLDByTJC(string hid, string billid, string itemid, PosBillDetailIsauto Isauto);


        /// <summary>
        /// 扫码点草重新计算金额
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billId">账单ID</param>
        void CmpBillAmountByScanOrder(string hid, string billId);

        /// <summary>
        /// 扫码点餐处理出品记录
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billId">账单ID</param>
        void CmpProducelist(string hid, string billId);


        /// <summary>
        /// 查询已付款金额
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billid"></param>
        /// <returns></returns>
        decimal GetIsPayAmount(string hid, string billid);



        /// <summary>
        /// 判断当前套餐明细是否已经添加了此消费项目
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billid">账单ID</param>
        /// <param name="upid">所属套餐</param>
        /// <param name="itemid">消费项目</param>
        /// <returns></returns>
        bool IsExistsSuiteItem(string hid, string billid, Guid? upid, string itemid);


        /// <summary>
        /// 平摊不计收入金额到各个明细的公司帐字段中
        /// </summary>
        /// <param name="billid"></param>
        /// <param name="Amount"></param>
        void AverageOutComeAmount(string hid, string billid, decimal Amount, decimal rate);

        /// <summary>
        /// 更新传菜状态
        /// </summary>
        /// <param name="detailid">明细id</param>
        /// <param name="sentStatus">（0：未传菜；1：已传菜）</param>
        void UpdatePosBillDetailStatus(string detailid, byte sentStatus);

       


        /// <summary>
        /// 获取最大额acbillno
        /// </summary>
        /// <returns></returns>
        string GetMaxAcBillNo(string hid, string billId);

        List<PosBillDetail> GetPosBillDetailsByTabid(string hid, string billid, string dcFlag = "");
    }
}