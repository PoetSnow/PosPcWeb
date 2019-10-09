using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos账单服务实现
    /// </summary>
    public class PosBillService : CRUDService<PosBill>, IPosBillService
    {
        private DbHotelPmsContext _db;

        public PosBillService(DbHotelPmsContext db) : base(db, db.PosBills)
        {
            _db = db;
        }

        protected override PosBill GetTById(string id)
        {
            return new PosBill { Billid = id };
        }

        /// <summary>
        /// 判断指定单号的账单是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="billid">单号</param>
        /// <returns>true:在酒店中已经有相同单号或者相同客账号的账单了，false：没有相同的</returns>
        public bool IsExists(string hid, string refeid, string billid)
        {
            return _db.PosBills.Any(w => w.Hid == hid && w.Refeid == refeid && w.Billid == billid);
        }

        /// <summary>
        /// 判断指定单号的账单是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="billid">单号</param>
        /// <param name="exceptId">要排队的单号</param>
        /// <returns>true:在酒店中已经有相同单号或者相同客账号的账单了，false：没有相同的</returns>
        public bool IsExists(string hid, string refeid, string billid, string exceptId)
        {
            return _db.PosBills.Any(w => w.Hid == hid && w.Refeid == refeid && w.Billid != exceptId && w.Billid == billid);
        }

        /// <summary>
        /// 根据酒店、营业点、餐台id获取当前餐台是否为抹台
        /// </summary>
        /// <param name="hid">酒店Id</param>
        /// <param name="tabid">餐台Id</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <returns></returns>
        public List<PosBill> GetSmearList(string hid, string tabid, DateTime? billBsnsDate)
        {
            var beginTime = Convert.ToDateTime(billBsnsDate.Value.ToString("yyyy-MM-dd 00:00:00"));
            var endTime = Convert.ToDateTime(billBsnsDate.Value.ToString("yyyy-MM-dd 23:59:59"));
            return _db.PosBills.Where(w => w.Hid == hid && w.Tabid == tabid && w.Status == (byte)PosBillStatus.开台 && w.BillBsnsDate.Value >= beginTime && w.BillBsnsDate.Value <= endTime).ToList();
        }

        /// <summary>
        /// 根据酒店、营业点、营业日判断是否有未清台的账单
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeids">营业点</param>
        /// <param name="business">营业日</param>
        /// <returns></returns>
        public bool IsNotPaid(string hid, string refeids, DateTime business)
        {
            string[] refes = refeids.Split(',');
            return _db.PosBills.Any(w => w.Hid == hid && refes.Contains(w.Refeid) && w.DepBsnsDate == business && (w.Status != (byte)PosBillStatus.清台 && w.Status < 50));
        }

        /// <summary>
        /// 根据酒店、营业点、营业日生成新的流水号
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="billBsnsDate">当前营业日</param>
        /// <returns></returns>
        public PosBill GetLastBillId(string hid, string refeid, DateTime? billBsnsDate)
        {
            PosBill bill = new PosBill();

            var model = _db.Database.SqlQuery<up_pos_getidnoResult>("exec up_pos_getidno @hid=@hid,@refeid=@refeid,@business=@business", new SqlParameter("@hid", hid), new SqlParameter("@refeid", refeid), new SqlParameter("@business", billBsnsDate)).FirstOrDefault();
            if (model != null)
            {
                bill.Billid = model.BillId;
                bill.BillNo = model.BillNo;
            }
            return bill;
        }

        /// <summary>
        /// 根据酒店、账单ID获取账单
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="billid">账单ID</param>
        /// <returns></returns>
        public PosBill GetEntity(string hid, string billid)
        {
            return _db.PosBills.Where(w => w.Hid == hid && w.Billid == billid).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店、营业点、餐台获取非结账的账单信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台id</param>
        /// <param name="billid">账单id</param>
        /// <returns></returns>
        public up_pos_list_billByRefeidAndTabidResult GetPosBillByTabId(string hid, string refeid, string tabid, string billid = "")
        {
            if (string.IsNullOrWhiteSpace(billid))
            {
                return _db.Database.SqlQuery<up_pos_list_billByRefeidAndTabidResult>("exec up_pos_list_billByRefeidAndTabid @hid=@hid,@refeid=@refeid,@tabid=@tabid", new SqlParameter("@hid", hid), new SqlParameter("@refeid", refeid), new SqlParameter("@tabid", tabid)).OrderByDescending(o => o.Billid).FirstOrDefault();
            }
            else
            {
                return _db.Database.SqlQuery<up_pos_list_billByRefeidAndTabidResult>("exec up_pos_list_billByRefeidAndTabid @hid=@hid,@refeid=@refeid,@tabid=@tabid", new SqlParameter("@hid", hid), new SqlParameter("@refeid", refeid), new SqlParameter("@tabid", tabid)).Where(w => w.Billid == billid).FirstOrDefault();
            }
        }

        /// <summary>
        /// 根据酒店、营业点、餐台获取非迟付和结账的账单信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台id</param>
        /// <param name="billid">账单id</param>
        /// <returns></returns>
        public up_pos_list_billByRefeidAndTabidResult GetPosBillByTabIdForNotDelayed(string hid, string refeid, string tabid, string billid = "")
        {
            if (string.IsNullOrWhiteSpace(billid))
            {
                return _db.Database.SqlQuery<up_pos_list_billByRefeidAndTabidResult>("exec up_pos_list_billByRefeidAndTabidForNotDelayed @hid=@hid,@refeid=@refeid,@tabid=@tabid", new SqlParameter("@hid", hid), new SqlParameter("@refeid", refeid), new SqlParameter("@tabid", tabid)).OrderByDescending(o => o.BillDate).FirstOrDefault();
            }
            else
            {
                return _db.Database.SqlQuery<up_pos_list_billByRefeidAndTabidResult>("exec up_pos_list_billByRefeidAndTabidForNotDelayed @hid=@hid,@refeid=@refeid,@tabid=@tabid", new SqlParameter("@hid", hid), new SqlParameter("@refeid", refeid), new SqlParameter("@tabid", tabid)).Where(w => w.Billid == billid).FirstOrDefault();
            }
        }







        /// <summary>
        /// 根据酒店、收银点、营业日获取收银账单列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="tabNo">台号</param>
        /// <returns></returns>
        public List<up_pos_list_billByPosidResult> GetPosBillByPosid(string hid, string posid, DateTime? billBsnsDate, string tabNo)
        {
            return _db.Database.SqlQuery<up_pos_list_billByPosidResult>("exec up_pos_list_billByPosid @hid=@hid,@posid=@posid,@billBsnsDate=@billBsnsDate,@tabNo=@tabNo", new SqlParameter("@hid", hid), new SqlParameter("@posid", posid), new SqlParameter("@billBsnsDate", billBsnsDate), new SqlParameter("@tabNo", tabNo)).ToList();
        }

        /// <summary>
        /// 根据酒店、收银点、营业日获取反结账单列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="tabNo">台号</param>
        /// <returns></returns>
        public List<up_pos_list_billByPosidResult> GetPosBillReverseCheckout(string hid, string posid, DateTime? billBsnsDate, string tabNo)
        {
            return _db.Database.SqlQuery<up_pos_list_billByPosidResult>("exec up_pos_list_billReverseCheckout @hid=@hid,@posid=@posid,@billBsnsDate=@billBsnsDate,@tabNo=@tabNo", new SqlParameter("@hid", hid), new SqlParameter("@posid", posid), new SqlParameter("@billBsnsDate", billBsnsDate), new SqlParameter("@tabNo", tabNo)).ToList();
        }

        /// <summary>
        /// 根据酒店、收银点、营业日获取迟付结账列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="tabNo">台号</param>
        /// <returns></returns>
        public List<up_pos_list_billByPosidResult> GetPosBillDelayedPayment(string hid, string posid, DateTime? billBsnsDate, string tabNo)
        {
            return _db.Database.SqlQuery<up_pos_list_billByPosidResult>("exec up_pos_list_billDelayedPayment @hid=@hid,@posid=@posid,@billBsnsDate=@billBsnsDate,@tabNo=@tabNo", new SqlParameter("@hid", hid), new SqlParameter("@posid", posid), new SqlParameter("@billBsnsDate", billBsnsDate.ToString() ?? ""), new SqlParameter("@tabNo", tabNo)).ToList();
        }

        /// <summary>
        /// 根据酒店、收银点、营业日获取客账账单列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="tabNo">台号</param>
        /// <returns></returns>
        public List<up_pos_list_billByPosidResult> GetPosBillGuestQuery(string hid, string posid, DateTime? billBsnsDate, string tabNo)
        {
            return _db.Database.SqlQuery<up_pos_list_billByPosidResult>("exec up_pos_list_billGuestQuery @hid=@hid,@posid=@posid,@billBsnsDate=@billBsnsDate,@tabNo=@tabNo", new SqlParameter("@hid", hid), new SqlParameter("@posid", posid), new SqlParameter("@billBsnsDate", billBsnsDate), new SqlParameter("@tabNo", tabNo)).ToList();
        }

        /// <summary>
        /// 根据酒店、账单ID获取账单信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="billid">账单id</param>
        /// <returns>账单信息</returns>
        public up_pos_list_billByBillidResult GetPosBillByBillid(string hid, string billid)
        {
            return _db.Database.SqlQuery<up_pos_list_billByBillidResult>("exec up_pos_list_billByBillid @hid=@hid,@billid=@billid", new SqlParameter("@hid", hid), new SqlParameter("@billid", billid)).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店、营业点、登记营业日获取新的快餐台号
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="tabFlag">餐台标识(0：物理台，1：快餐台)</param>
        /// <returns>快餐台号</returns>
        public string GetTakeoutForTabid(string hid, string refeid, DateTime? billBsnsDate, byte? tabFlag)
        {
            var tab = _db.PosBills.Where(w => w.Hid == hid && w.Refeid == refeid && w.BillBsnsDate == billBsnsDate && w.TabFlag == tabFlag).OrderByDescending(o => o.BillDate).FirstOrDefault();
            if (tab == null)
            {
                return 1.ToString().PadLeft(4, '0');
            }
            else
            {
                if (tab.Tabid.Length > hid.Length && tab.Tabid.IndexOf(hid) > -1)
                {
                    tab.Tabid = tab.Tabid.Substring(hid.Length);
                }
                return (Convert.ToInt32(tab.Tabid) + 1).ToString().PadLeft(4, '0');
            }
        }

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
        public void ChangeTab(string hid, string oldbillId, string oldTabId, decimal? newServiceRate, decimal? newLimit, string ServiceRateFlag, string ItemFlag, string Newtabid)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append(" exec up_pos_ChangeTab  @hid=@hid,");
            str.Append("@oldbillId=@oldbillId,");
            str.Append("@oldTabId=@oldTabId,");
            str.Append("@Newtabid=@Newtabid,");
            str.Append("@newServiceRate=@newServiceRate,");
            str.Append("@newLimit=@newLimit,");
            str.Append("@ServiceRateFlag=@ServiceRateFlag,");
            str.Append("@ItemFlag=@ItemFlag");

            _db.Database.ExecuteSqlCommand(str.ToString(),
                    new SqlParameter("@hid", hid),
                    new SqlParameter("@oldbillId", oldbillId),
                    new SqlParameter("@oldTabId", oldTabId),
                    new SqlParameter("@Newtabid", Newtabid),
                    new SqlParameter("@newServiceRate", newServiceRate),
                    new SqlParameter("@newLimit", newLimit),
                    new SqlParameter("@ServiceRateFlag", ServiceRateFlag),
                    new SqlParameter("@ItemFlag", ItemFlag)
                    );
        }

        /// <summary>
        /// 根据酒店ID，营业点ID，营业日期获取所有的账单数据
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeId"></param>
        /// <param name="billBsnsDate"></param>
        /// <returns></returns>
        public List<PosBill> GetBillListForPosRefe(string hid, string refeId, DateTime? billBsnsDate)
        {
            return _db.PosBills.Where(m => m.Status == 1 && m.Refeid == refeId && m.Hid == hid && m.BillBsnsDate == billBsnsDate).ToList();
        }

        /// <summary>
        /// 根据酒店ID与餐台ID获取开台状态的账单列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="tabid"></param>
        /// <returns></returns>
        public List<PosBill> GetSmearListByClearTab(string hid, string tabid)
        {
            return _db.PosBills.Where(w => w.Hid == hid && w.Tabid == tabid && w.Status == (byte)PosBillStatus.开台).ToList();
        }

        /// <summary>
        /// 客账查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<up_pos_list_billByPosidResult> GetPosBillGuestQuery(QueryBillModel model)
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.Append(" exec up_pos_list_billGuestQuery  @hid=@hid,");
            str.Append("@posid=@posid,");
            str.Append("@billBsnsDate=@billBsnsDate,");
            str.Append("@tabNo=@tabNo,");
            str.Append("@ItemName=@ItemName,");
            str.Append("@MinAmount=@MinAmount,");
            str.Append("@MaxAmount=@MaxAmount,");
            str.Append("@PayMethod=@PayMethod,");
            str.Append("@RefeId=@RefeId");

            return _db.Database.SqlQuery<up_pos_list_billByPosidResult>(str.ToString(),

                  new SqlParameter("@hid", model.Hid),
                  new SqlParameter("@posid", model.PosId),
                  new SqlParameter("@billBsnsDate", model.BillBsnsDate),
                  new SqlParameter("@tabNo", model.tabNo ?? ""),
                  new SqlParameter("@ItemName", model.ItemName ?? ""),
                  new SqlParameter("@MinAmount", model.MinAmount ?? 0),
                  new SqlParameter("@MaxAmount", model.MaxAmount ?? 0),
                  new SqlParameter("@PayMethod", model.PayMethod ?? ""),
                  new SqlParameter("@RefeId", model.RefeId ?? "")
                  ).ToList();
        }


        /// <summary>
        /// 根据账单ID 获取账单信息
        /// </summary>
        /// <param name="billid">账单ID</param>
        /// <param name="hid">酒店代码</param>
        /// <returns></returns>
        public PosBill GetBillByCancalLockTab(string billid, string hid)
        {
            return _db.PosBills.AsNoTracking().ToList().Find(m => m.Billid == billid && m.Hid == hid);

        }

        //获取账单的消费项目的所有消费项目大类
        /// <summary>
        /// 根据账单获取账单消费项目详情的消费大类
        /// </summary>
        /// <param name="posBill"></param>
        /// <returns></returns>
        public List<up_pos_queryBillDetailPositemResult> GetPosItemByPosBill(string hid, PosBill posBill)
        {
            var sql = $"exec up_pos_queryBillDetailPositem '{hid}','{posBill.Billid}'";
            return _db.Database.SqlQuery<up_pos_queryBillDetailPositemResult>(sql).ToList();
        }


        /// <summary>
        /// 退出餐台删除账单数据以及修改餐台状态
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billId">账单ID</param>
        /// <param name="tabId">餐台ID</param>
        public void DeletePosBillByCCancelLockTab(string hid, string billId, string tabId)
        {
            _db.Database.ExecuteSqlCommand(" exec up_pos_deleteBillByCancelLockTab @hid=@hid,@billId=@billId,@tabId=@tabId",
                           new SqlParameter("@hid", hid),
                           new SqlParameter("@billId", billId),
                           new SqlParameter("@tabId", tabId)
                );
        }

        /// <summary>
        /// 根据酒店ID以及openID 获取账单
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="tabId"></param>
        /// <param name="openWxid"></param>
        /// <returns></returns>
        public PosBill GetPosBillByScanOrder(string hid, string tabId, string openWxid)
        {
            return _db.PosBills.Where(w => w.Hid == hid && w.Tabid == tabId && w.OpenWxid == openWxid && (w.Status == (byte)PosBillStatus.扫码点餐默认状态 || w.Status == (byte)PosBillStatus.结账)).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店ID以及餐台ID 获取账单
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="tabId"></param>
        /// <returns></returns>
        public PosBill GetPosbillByScanOrder(string hid, string tabId)
        {
            return _db.PosBills.Where(w => w.Hid == hid && w.Tabid == tabId && (w.Status == (byte)PosBillStatus.开台 || w.Status == (byte)PosBillStatus.结账)).FirstOrDefault();
        }

        /// <summary>
        /// 根据餐台id查找最新的账单
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeid"></param>
        /// <param name="tabid"></param>
        /// <returns></returns>
        public PosBill GetPosBillByTabid(string hid, string refeid, string tabid)
        {
            return _db.PosBills.Where(w => w.Hid == hid && w.Tabid == tabid && w.Refeid == refeid).OrderByDescending(w => w.Billid).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店ID，营业点ID，营业日期获取所有的账单数据
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeId"></param>
        /// <param name="billBsnsDate"></param>
        /// <returns></returns>
        public List<PosBill> GetAllBillListForPosRefe(string hid, string refeId, DateTime? billBsnsDate)
        {
            return _db.PosBills.Where(m => m.Refeid == refeId && m.Hid == hid && m.BillBsnsDate == billBsnsDate).ToList();
        }
    }
}