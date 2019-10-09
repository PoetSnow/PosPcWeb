using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos账单明细服务实现
    /// </summary>
    public class PosBillDetailService : CRUDService<PosBillDetail>, IPosBillDetailService, IPayActionXmlOperate
    {
        private DbHotelPmsContext _db;

        public PosBillDetailService(DbHotelPmsContext db) : base(db, db.PosBillDetails)
        {
            _db = db;
        }

        protected override PosBillDetail GetTById(string id)
        {
            return new PosBillDetail { Id = Convert.ToInt64(id) };
        }

        /// <summary>
        /// 根据酒店、账单ID、消费项目id判断当前明细是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="itemid">消费项目ID</param>
        /// <returns></returns>
        public bool IsExists(string hid, string billid, string itemid)
        {
            return _db.PosBillDetails.Any(a => a.Hid == hid && a.Billid == billid && a.Itemid == itemid && a.Status < 50);
        }

        /// <summary>
        /// 根据酒店、账单ID、消费项目id判断当前明细是否存在
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="itemid">消费项目ID</param>
        /// <param name="itemName">消费项目名称</param>
        /// <returns></returns>
        public bool IsExists(string hid, string billid, string itemid, string itemName)
        {
            return _db.PosBillDetails.Any(a => a.Hid == hid && a.Billid == billid && a.Itemid == itemid && a.ItemName == itemName && a.Status < 50);
        }

        /// <summary>
        /// 根据酒店和账单ID获取账单明细
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <param name="dcFlag">付款标记（D：消费；C：付款）</param>
        /// <returns></returns>
        public List<up_pos_list_BillDetailByBillidResult> GetUpBillDetailByBillid(string hid, string billid, string dcFlag = "")
        {
            return _db.Database.SqlQuery<up_pos_list_BillDetailByBillidResult>("exec up_pos_list_BillDetailByBillid @hid=@hid,@billid=@billid,@dcFlag=@dcFlag"
                , new SqlParameter("@hid", hid), new SqlParameter("@billid", billid), new SqlParameter("@dcFlag", dcFlag)).ToList();
        }

        /// <summary>
        /// 根据酒店和账单ID获取账单收银明细
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <param name="dcFlag">付款标记（D：消费；C：付款）</param>
        /// <returns></returns>
        public List<up_pos_list_BillDetailByCashierResult> GetUpBillDetailByCashier(string hid, string billid, string dcFlag = "")
        {
            return _db.Database.SqlQuery<up_pos_list_BillDetailByCashierResult>("exec up_pos_list_BillDetailByCashier @hid=@hid,@billid=@billid,@dcFlag=@dcFlag"
                , new SqlParameter("@hid", hid), new SqlParameter("@billid", billid), new SqlParameter("@dcFlag", dcFlag)).ToList();
        }

        /// <summary>
        /// 根据酒店和账单明细ID获取应付金额
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <returns></returns>
        public decimal GetBillDetailAmountSumByBillid(string hid, string billid)
        {
            return _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billid && w.DcFlag == "D" && w.Status == (byte)PosBillDetailStatus.正常 && w.SD == false && w.IsCheck == false).Sum(s => s.Amount) ?? 0;
        }

        /// <summary>
        /// 根据酒店、账单ID、付款标识获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="dcFlag">付款标记（D：消费；C：付款）</param>
        /// <returns></returns>
        public List<PosBillDetail> GetBillDetailByDcFlag(string hid, string billid, string dcFlag = "")
        {
            return _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billid && (w.DcFlag == dcFlag || dcFlag == "") && w.Status < 50).ToList();
        }

        /// <summary>
        /// 根据酒店、账单、结账交易号查询账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="settleTransno">结账交易号</param>
        /// <returns></returns>
        public PosBillDetail GetBillDetailBySettleTransno(string hid, string billid, string settleTransno)
        {
            return _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billid && w.SettleTransno == settleTransno).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店、账单、套餐ID获取套餐明细
        /// </summary>
        /// <param name="hid">酒店</param>
        /// <param name="billid">账单</param>
        /// <param name="upid">套餐ID</param>
        /// <returns></returns>
        public List<PosBillDetail> GetBillDetailByUpid(string hid, string billid, Guid? upid)
        {
            return _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billid && w.Upid == upid).ToList();
        }

        /// <summary>
        /// 根据酒店、账单、消费项目获取账单明细
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <param name="itemid">消费项目id</param>
        /// <returns></returns>
        public PosBillDetail GetBillDetailByBillid(string hid, string billid, string itemid)
        {
            return _db.PosBillDetails.Where(a => a.Hid == hid && a.Billid == billid && a.Itemid == itemid && a.Status < 50).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店、账单、消费项目获取账单明细
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <param name="itemid">消费项目id</param>
        /// <param name="itemName">消费项目id</param>
        /// <returns></returns>
        public PosBillDetail GetBillDetailByBillid(string hid, string billid, string itemid, string itemName)
        {
            return _db.PosBillDetails.Where(a => a.Hid == hid && a.Billid == billid && a.Itemid == itemid && a.ItemName == itemName && a.Status < 50).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店结账ID获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单id</param>
        /// <param name="settleid">结账ID</param>
        /// <returns></returns>
        public PosBillDetail GetEntity(string hid, string billid, Guid? settleid)
        {
            return _db.PosBillDetails.Where(a => a.Hid == hid && a.Billid == billid && a.Settleid == settleid).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店结账ID获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单id</param>
        /// <param name="isauto">自动标志</param>
        /// <returns></returns>
        public PosBillDetail GetEntity(string hid, string billid, byte? isauto)
        {
            return _db.PosBillDetails.Where(a => a.Hid == hid && a.Billid == billid && a.Isauto == isauto).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店明细ID获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">明细ID</param>
        /// <returns></returns>
        public PosBillDetail GetEntity(string hid, long id)
        {
            return _db.PosBillDetails.Where(a => a.Hid == hid && a.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店、账单ID、付款标识、计费状态获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="dcFlag">付款标记（D：消费；C：付款）</param>
        /// <param name = "status" >计费状态</ param >
        /// <returns></returns>
        public List<PosBillDetail> GetBillDetailByBillid(string hid, string billid, string dcFlag, byte status)
        {
            return _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billid && w.DcFlag == dcFlag && w.Status == status).ToList();
        }

        /// <summary>
        /// 根据酒店、账单ID、付款标识、计费状态获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="dcFlag">付款标记（D：消费；C：付款）</param>
        /// <param name = "status" >计费状态</ param >
        /// <returns></returns>
        public List<PosBillDetail> GetBillDetailByBillidAndStatus(string hid, string billid, string dcFlag, byte status)
        {
            return _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billid && w.DcFlag == dcFlag && w.Status < status).ToList();
        }

        /// <summary>
        /// 根据酒店、账单ID、付款标识获取账单明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="dcFlag">付款标识</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public List<PosBillDetail> GetBillDetailByBillid(string hid, string billid, string dcFlag, int pageIndex, int pageSize)
        {
            var list = _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billid && w.DcFlag == dcFlag).OrderBy(o => o.Billid).ToList();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 根据酒店和账单查询付款明细
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <returns></returns>
        public List<up_pos_list_BillDetailForPaymentResult> GetBillDetailForPaymentByBillid(string hid, string billid)
        {
            return _db.Database.SqlQuery<up_pos_list_BillDetailForPaymentResult>("exec up_pos_list_BillDetailForPayment @hid=@hid,@billid=@billid"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@billid", billid)
                ).ToList();
        }

        /// <summary>
        /// 根据酒店和账单查询付款金额合计
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <returns></returns>
        public up_pos_cmp_PaymentTotalResult GetBillDetailForPaymentTotalByBillid(string hid, string billid)
        {
            return _db.Database.SqlQuery<up_pos_cmp_PaymentTotalResult>("exec up_pos_cmp_PaymentTotal @hid=@hid,@billid=@billid"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@billid", billid)
                ).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店、账单和付款标记获取新的流水号
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="dcFlag">付款标记（D：消费；C：付款）</param>
        /// <returns></returns>
        public string GetNewBatchTimeByBillid(string hid, string billid, string dcFlag)
        {
            int batchTime = 01;

            var posBillDetails = _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billid && w.DcFlag == dcFlag)
                .OrderByDescending(o => o.BatchTime).FirstOrDefault();
            if (posBillDetails != null && posBillDetails.BatchTime != null)
            {
                batchTime = Convert.ToInt32(posBillDetails.BatchTime) + 1;
            }

            return batchTime.ToString("D2");
        }

        /// <summary>
        /// 根据酒店id、单号、主单号统计账单明细数据
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">单号</param>
        /// <param name="mBillid">主单号</param>
        public void StatisticsBillDetail(string hid, string billid, string mBillid)
        {
            _db.Database.ExecuteSqlCommand("exec up_pos_cmp_amount @hid=@hid,@billid=@billid,@mBillid=@mBillid"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@billid", billid)
                , new SqlParameter("@mBillid", mBillid)
                );
        }

        /// <summary>
        /// 根据酒店id、单号、主单号获取账单明细统计数据
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">单号</param>
        /// <param name="mBillid">主单号</param>
        /// <param name="isave">1：点菜，2：落单</param>
        public up_pos_cmp_billDetailStatisticsResult GetBillDetailStatistics(string hid, string billid, string mBillid, int isave)
        {
            return _db.Database.SqlQuery<up_pos_cmp_billDetailStatisticsResult>("exec up_pos_cmp_billDetailStatistics @hid=@hid,@billid=@billid,@mBillid=@mBillid,@isave=@isave"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@billid", billid)
                , new SqlParameter("@mBillid", mBillid)
                , new SqlParameter("@isave", isave)
                ).FirstOrDefault();
        }

        /// <summary>
        /// 获取尾数处理后的金额
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        public decimal GetAmountByBillTailProcessing(string hid, string refeid, decimal amount)
        {
            var result = _db.Database.SqlQuery<decimal?>("exec up_pos_cmp_billTailProcessing @hid=@hid,@refeid=@refeid,@amount=@amount"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@refeid", refeid)
                , new SqlParameter("@amount", amount)
                ).First();
            return result == null ? amount : result.Value;
        }

        /// <summary>
        /// 捷信达的外部接口
        /// </summary>
        /// <param name="grpid">酒店id</param>
        /// <param name="channelCode">渠道代码</param>
        /// <param name="xmlStr">业务xml</param>
        /// <returns></returns>
        public string RealOperate(string grpid, string channelCode, string xmlStr)
        {
            //调用存储过程来执行业务处理
            var sendId = _db.Database.SqlQuery<decimal>("exec up_crsRealOperate @grpid=@grpid,@channelCode=@channelCode,@xmlText=@xmlText"
                , new SqlParameter("@grpid", grpid)
                , new SqlParameter("@channelCode", "")
                , new SqlParameter("@xmlText", System.Data.SqlDbType.Text) { Value = xmlStr }
                ).Single();

            //取出需要返回的详细xml信息
            return _db.Database.SqlQuery<string>("select transxml from crsSendInfo where sendid=@sendid"
                , new SqlParameter("@sendid", sendId)
                ).Single();
        }

        public List<up_pos_list_BillDetailByGroupItemClassResult> GetBillDetailGroupItemClass(string hid, string billId, string Flag, string dcFlag = "")
        {
            var result = _db.Database.SqlQuery<up_pos_list_BillDetailByGroupItemClassResult>("exec up_pos_list_BillDetailByGroupItemClass @hid=@hid,@billId=@billId,@Flag=@Flag,@DcFlag=@DcFlag"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@billId", billId)
                , new SqlParameter("@Flag", Flag)
                , new SqlParameter("@DcFlag", dcFlag)
                );
            return result.ToList();
        }

        /// <summary>
        /// 根据酒店、账单ID、消费项目id判断当前明细是否存在落单的项目
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="itemid">项目ID</param>
        /// <returns></returns>
        public bool IsExistsForLD(string hid, string billid, string itemid)
        {
            return _db.PosBillDetails.Any(a => a.Hid == hid && a.Billid == billid && a.Itemid == itemid && a.Status < 50 && a.Status == 4 && a.SD != true);
        }

        /// <summary>
        /// 根据酒店、账单ID、消费项目id判断当前明细是否存在落单的项目
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="itemid">项目ID</param>
        ///  <param name="itemName">项目名称</param>
        /// <returns></returns>
        public bool IsExistsForLD(string hid, string billid, string itemid, string itemName)
        {
            return _db.PosBillDetails.Any(a => a.Hid == hid && a.Billid == billid && a.Itemid == itemid && a.ItemName == itemName && a.Status < 50 && string.IsNullOrEmpty(a.BatchTime));
        }

        /// <summary>
        /// 根据酒店、账单、项目、单位获取没有落单的账单明细
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <param name="itemid">消费项目id</param>
        /// <returns></returns>
        public PosBillDetail GetBillDetailByBillidForLD(string hid, string billid, string itemid)
        {
            return _db.PosBillDetails.Where(a => a.Hid == hid && a.Billid == billid && a.Itemid == itemid && a.Status < 50 && a.Status == 4 && a.SD != true).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店、账单、项目、单位获取没有落单的账单明细
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <param name="itemid">消费项目id</param>
        /// <param name="itemName">消费项目名称</param>
        /// <returns></returns>
        public PosBillDetail GetBillDetailByBillidForLD(string hid, string billid, string itemid, string itemName)
        {
            return _db.PosBillDetails.Where(a => a.Hid == hid && a.Billid == billid && a.Itemid == itemid && a.ItemName == itemName && a.Status < 50 && string.IsNullOrEmpty(a.BatchTime)).FirstOrDefault();
        }

        public List<up_pos_BillDetailResult> GetBillDetailByDcFlagForPosInSing(string hid, string billid, string dcFlag = "")
        {
            return _db.Database.SqlQuery<up_pos_BillDetailResult>("exec up_pos_BillDetailList @hid=@hid,@billId=@billId,@DcFlag=@DcFlag"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@billId", billid)
                , new SqlParameter("@DcFlag", dcFlag)
                ).ToList();
        }

        /// <summary>
        /// 根据酒店id、单号、主单号获取账单明细统计数据
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">单号</param>
        /// <param name="mBillid">主单号</param>
        public List<up_pos_print_billDetailResult> GetBillDetailByPrint(string hid, string billid, string mBillid)
        {
            return _db.Database.SqlQuery<up_pos_print_billDetailResult>("exec up_pos_print_billDetail @h99hid=@h99hid, @h99billid = @h99billid, @h99mBillid = @h99mBillid"
                , new SqlParameter("@h99hid", hid)
                , new SqlParameter("@h99billid", billid)
                , new SqlParameter("@h99mBillid", mBillid)
                ).ToList();
        }

        /// <summary>
        /// 根据酒店id、单号、主单号获取账单明细统计数据
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">单号</param>
        /// <param name="mBillid">主单号</param>
        public List<up_pos_print_billOrderResult> GetBillOrderByPrint(string hid, string billid, string mBillid)
        {
            return _db.Database.SqlQuery<up_pos_print_billOrderResult>("exec up_pos_print_billOrder @h99hid=@h99hid, @h99billid = @h99billid, @h99mBillid = @h99mBillid"
                , new SqlParameter("@h99hid", hid)
                , new SqlParameter("@h99billid", billid)
                , new SqlParameter("@h99mBillid", mBillid)
                ).ToList();
        }

        /// <summary>
        /// 重算折扣
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billid"></param>
        /// <param name="mBillid"></param>
        public void UpdateBillDetailDisc(string hid, string billid, string mBillid)
        {
            _db.Database.ExecuteSqlCommand("exec up_pos_cmp_discount @hid=@hid,@billid=@billid,@mBillid=@mBillid"
               , new SqlParameter("@hid", hid)
               , new SqlParameter("@billid", billid)
               , new SqlParameter("@mBillid", mBillid)
               );
        }

        public DataSet GetDataSetByPayPrint(string sql, params SqlParameter[] para)
        {
            List<string> sql2 = new List<string>();
            for (int i = 0; i < para.Length; i++)
            {
                SqlParameter p = para[i];
                sql2.Add(p.ParameterName);
            }
            DataSet sd = new DataSet();

            SqlConnection conn = _db.Database.Connection as SqlConnection;

            using (SqlCommand cmd = new SqlCommand(sql + " " + string.Join(",", sql2.ToArray()), conn))
            {
                cmd.Parameters.AddRange(para);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(sd);
            }

            return sd;
        }

        /// <summary>
        /// 查询转菜数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="tabid">当前餐台ID</param>
        /// <param name="billDetailList">换台之后的账单明细ID集合</param>
        /// <returns></returns>
        public List<up_pos_changeItemListResult> GetDataSetByChangeItem(string hid, string tabid, string billDetailList)
        {
            return _db.Database.SqlQuery<up_pos_changeItemListResult>("exec up_pos_changeItemList @hid=@hid,@oldTabId=@oldTabId,@billDetailList=@billDetailList"
                           , new SqlParameter("@hid", hid)
                           , new SqlParameter("@oldTabId", tabid)
                           , new SqlParameter("@billDetailList", billDetailList)
                           ).ToList();
        }

        /// <summary>
        /// 查询转台数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="tabid">当前餐台ID</param>
        /// <param name="billId">账单ID</param>
        /// <returns></returns>
        public List<up_pos_ChangeTabBillDetailListResult> GetDataSetByChangeTab(string hid, string tabid, string billId)
        {
            return _db.Database.SqlQuery<up_pos_ChangeTabBillDetailListResult>("exec up_pos_ChangeTabBillDetailList @hid=@hid,@oldTabId=@oldTabId,@BillID=@BillID"
                           , new SqlParameter("@hid", hid)
                           , new SqlParameter("@oldTabId", tabid)
                           , new SqlParameter("@BillID", billId)
                           ).ToList();
        }

        /// <summary>
        /// 获取特价菜
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="itemid">项目ID</param>
        /// <param name="Isauto"></param>
        /// <returns></returns>
        public PosBillDetail GetBillDetailByBillidForLD(string hid, string billid, string itemid, PosBillDetailIsauto Isauto)
        {
            return _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billid && w.Itemid == itemid && w.Status < 50 && w.Status == 4 && w.SD != true && w.Isauto == (byte)Isauto).FirstOrDefault();
        }

        /// <summary>
        /// 非特价菜
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <param name="itemid">项目ID</param>
        /// <param name="Isauto"></param>
        /// <returns></returns>
        public PosBillDetail GetBillDetailByBillidForLDByTJC(string hid, string billid, string itemid, PosBillDetailIsauto Isauto)
        {
            return _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billid && w.Itemid == itemid && w.Status < 50 && w.Status == 4 && w.SD != true && w.Isauto != (byte)Isauto).FirstOrDefault();
        }
        /// <summary>
        /// 获取已付金额
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billid"></param>
        /// <returns></returns>
        public decimal GetIsPayAmount(string hid, string billid)
        {
            return _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billid && w.DcFlag == "C" && w.Status == (byte)PosBillDetailStatus.正常 && w.IsCheck == true).Sum(s => s.Amount) ?? 0;
        }




        /// <summary>
        /// 扫码点草重新计算金额
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billId">账单ID</param>
        public void CmpBillAmountByScanOrder(string hid, string billId)
        {
            //折扣
            _db.Database.ExecuteSqlCommand("exec up_pos_cmp_discount @hid=@hid,@billid=@billid,@mBillid=@mBillid"
               , new SqlParameter("@hid", hid)
               , new SqlParameter("@billid", billId)
               , new SqlParameter("@mBillid", billId)
               );
            //作法加价
            _db.Database.ExecuteSqlCommand("exec up_pos_cmp_actionamt @hid=@hid,@billid=@billid,@mBillid=@mBillid"
             , new SqlParameter("@hid", hid)
             , new SqlParameter("@billid", billId)
             , new SqlParameter("@mBillid", billId)
             );
            //处理最低消费
            _db.Database.ExecuteSqlCommand("exec up_pos_cmp_limit @hid=@hid,@billid=@billid,@mBillid=@mBillid"
             , new SqlParameter("@hid", hid)
             , new SqlParameter("@billid", billId)
             , new SqlParameter("@mBillid", billId)
             );
            //重新计算服务费
            _db.Database.ExecuteSqlCommand("exec up_pos_cmp_service @hid=@hid,@billid=@billid,@mBillid=@mBillid"
             , new SqlParameter("@hid", hid)
             , new SqlParameter("@billid", billId)
             , new SqlParameter("@mBillid", billId)
             );
            //金额写入到账单主表
            _db.Database.ExecuteSqlCommand("exec up_pos_cmp_billAmoount @hid=@hid,@billid=@billid,@mBillid=@mBillid"
           , new SqlParameter("@hid", hid)
           , new SqlParameter("@billid", billId)
           , new SqlParameter("@mBillid", billId)
           );
        }

        /// <summary>
        /// 扫码点餐处理出品记录
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billId">账单ID</param>
        public void CmpProducelist(string hid, string billId)
        {
            //出品
            _db.Database.ExecuteSqlCommand("exec up_pos_cmp_producelist @hid=@hid,@billid=@billid,@mBillid=@mBillid"
           , new SqlParameter("@hid", hid)
           , new SqlParameter("@billid", billId)
           , new SqlParameter("@mBillid", billId)
           );
        }

        /// <summary>
        /// 判断当前套餐明细是否已经添加了此消费项目
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billid">账单ID</param>
        /// <param name="upid">所属套餐</param>
        /// <param name="itemid">消费项目</param>
        /// <returns></returns>
        public bool IsExistsSuiteItem(string hid, string billid, Guid? upid, string itemid)
        {
            return _db.PosBillDetails.Any(w => w.Hid == hid && w.Billid == billid && w.Upid == upid && w.Itemid == itemid && w.Status < (byte)PosBillDetailStatus.不加回库取消);

        }

        /// <summary>
        /// 平摊不计收入金额到各个明细的公司帐字段中
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">单据id</param>
        /// <param name="Amount">不计入收入所支付的金额</param>
        /// <param name="rate">汇率</param>
        public void AverageOutComeAmount(string hid, string billid, decimal Amount, decimal rate)
        {
            try
            {
                //拿到消费明细列表
                var posbilldetaillist = GetBillDetailByDcFlag(hid, billid);
                //消费总额
                var sumbillamount = decimal.Parse(posbilldetaillist.Where(t => t.DcFlag == "D" && t.Status <= 50).Sum(m => m.Amount).ToString());
                //计算当前明细金额所占单据总额的比例
                var percent = Math.Round((Amount * rate), 2) / (sumbillamount == 0 ? 1 : sumbillamount);
                decimal sumoutComeAmount = 0;
                decimal oldoutComeAmount = 0;
                foreach (var item in posbilldetaillist.Where(t => t.DcFlag == "D" && t.Status <= 50).ToList())
                {
                    oldoutComeAmount = 0;
                    //分得不计入收入所支付的金额
                    var getOutComeAmount = item.Amount == null ? 0 : decimal.Parse(item.Amount.ToString()) * percent;
                    //存入数据
                    var updateobj = _db.PosBillDetails.Find(item.Id);
                    oldoutComeAmount = updateobj.outComeAmount == null ? 0 : decimal.Parse(updateobj.outComeAmount.ToString());
                    updateobj.outComeAmount = oldoutComeAmount + Math.Round(getOutComeAmount, 2);
                    sumoutComeAmount = sumoutComeAmount + Math.Round(getOutComeAmount, 2);
                    _db.Entry(updateobj).State = EntityState.Modified;
                }
                _db.SaveChanges();
                if (sumoutComeAmount < Math.Round((Amount * rate), 2))
                {
                    //随便拿一条明细，补差额
                    oldoutComeAmount = 0;
                    var itemid = posbilldetaillist.Where(t => t.DcFlag == "D" && t.Status <= 50).ToList().FirstOrDefault().Id;
                    var updateobj = _db.PosBillDetails.Find(itemid);
                    oldoutComeAmount = updateobj.outComeAmount == null ? 0 : decimal.Parse(updateobj.outComeAmount.ToString());
                    updateobj.outComeAmount = oldoutComeAmount + (Amount - sumoutComeAmount);
                    _db.Entry(updateobj).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                _db.AddDataChangeLogs(OpLogType.Pos账单消费明细修改);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新传菜状态
        /// </summary>
        /// <param name="detailid">明细id</param>
        /// <param name="sentStatus">（0：未传菜；1：已传菜）</param>
        public void UpdatePosBillDetailStatus(string detailid, byte sentStatus)
        {
            try
            {
                var updateobj = _db.PosBillDetails.Find(long.Parse(detailid));
                updateobj.sentStatus = sentStatus;
                _db.Entry(updateobj).State = EntityState.Modified;
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 根据餐台id查找最新的单据明细
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="tabid"></param>
        /// <param name="dcFlag"></param>
        /// <returns></returns>
        public List<PosBillDetail> GetPosBillDetailsByTabid(string hid, string billid, string dcFlag="")
        {
            try
            {                
                return _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billid && (w.DcFlag == dcFlag || dcFlag == "")).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string GetMaxAcBillNo(string hid, string billId)
        {

            var list = _db.PosBillDetails.Where(w => w.Hid == hid && w.Billid == billId);

            List<int> listBillNo = new List<int>();
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item.Acbillno))
                {
                    listBillNo.Add(Convert.ToInt32(item.Acbillno));
                }
            }
            if (listBillNo.Count <= 0)
            {
                return "1";
            }
            else
            {
                return (listBillNo.Max() + 1).ToString();
            }


        }
    }
}


