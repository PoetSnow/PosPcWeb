using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos餐台状态服务实现
    /// </summary>
    public class PosTabStatusService : CRUDService<PosTabStatus>, IPosTabStatusService
    {
        private DbHotelPmsContext _db;

        public PosTabStatusService(DbHotelPmsContext db) : base(db, db.PosTabStatuss)
        {
            _db = db;
        }

        protected override PosTabStatus GetTById(string id)
        {
            return new PosTabStatus { Tabid = id };
        }

        /// <summary>
        /// 判断指定的代码或者名称的餐台状态是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">餐台状态代码</param>
        /// <param name="name">餐台状态名称</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的餐台状态了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name)
        {
            return _db.PosTabStatuss.Any(w => w.Hid == hid && (w.TabNo == code || w.TabName == name));
        }

        /// <summary>
        /// 判断指定的代码或者名称的餐台状态是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">餐台状态代码</param>
        /// <param name="name">餐台状态名称</param>
        /// <param name="exceptId">要排队的餐台状态id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的餐台状态了，false：没有相同的</returns>
        public bool IsExists(string hid, string code, string name, string exceptId)
        {
            return _db.PosTabStatuss.Any(w => w.Hid == hid && w.Tabid != exceptId && (w.TabNo == code || w.TabName == name));
        }

        /// <summary>
        /// 根据查询条件获取指定酒店下的餐台状态列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">台号或台名</param>
        /// <param name="refeid">营业点id、代码或名称</param>
        /// <param name="tabtypeid">餐台类型id、代码或名称</param>
        /// <param name="tabStatus">餐台状态</param>
        /// <returns></returns>
        public int GetPosTabStatusTotal(string hid, string code, string refeid, string tabtypeid, byte? tabStatus)
        {
            string[] refeids = refeid.Split(',');
            var result = _db.PosTabStatuss.Count(w => w.Hid == hid && (refeids.Contains(w.Refeid) || refeid == "") && (w.TabStatus == tabStatus || tabStatus == null) && (w.TabNo == code || w.TabNo.Contains(code)) && (w.Tabtypeid == tabtypeid || tabtypeid == ""));
            return result;
        }

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
        public List<PosTabStatus> GetPosTabStatus(string hid, string code, string refeid, string tabtypeid, byte? tabStatus, int pageIndex, int pageSize)
        {
            string[] refeids = refeid.Split(',');
            var result = _db.PosTabStatuss.Where(w => w.Hid == hid && (refeids.Contains(w.Refeid) || refeid == "") && (w.TabStatus == tabStatus || tabStatus == null) && (w.TabNo == code || w.TabNo.Contains(code)) && (w.Tabtypeid == tabtypeid || tabtypeid == "")).OrderBy(o => o.Seqid).ToList();
            return result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 根据酒店、营业点、餐台获取餐台状态
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台id</param>
        /// <returns></returns>
        public PosTabStatus GetPosTabStatus(string hid, string refeid, string tabid)
        {
            var tabStatus = _db.PosTabStatuss.SingleOrDefault(w => w.Hid == hid && w.Refeid == refeid && w.Tabid == tabid);
            return tabStatus;
        }

        /// <summary>
        /// 设置餐台状态
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="opType">操作代码</param>
        /// <param name="ids"></param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        public void SetTabStatus(string hid, string refeid, byte opType, string ids, string beginDate, string endDate)
        {
            _db.Database.ExecuteSqlCommand("EXEC up_pos_tabStatusSet @hid = @hid, @refeid = @refeid, @opType = @opType, @ids = @ids, @beginDate = @beginDate,@endDate = @endDate", new SqlParameter("@hid", hid), new SqlParameter("@refeid", refeid), new SqlParameter("@opType", opType), new SqlParameter("@ids", ids), new SqlParameter("@beginDate", beginDate), new SqlParameter("@endDate", endDate));
        }

        /// <summary>
        /// 根据酒店、收银点、营业点、餐台类型获取开台统计数据
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="posid">收银点id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabtypeid">餐台类型ID</param>
        /// <returns></returns>
        public up_pos_cmp_tabStatusResult GetPosTabStatusStatistics(string hid, string posid, string refeid, string tabtypeid)
        {
            return _db.Database.SqlQuery<up_pos_cmp_tabStatusResult>("EXEC up_pos_cmp_tabStatus @hid=@hid,@posid=@posid,@refeid=@refeid,@tabtypeid=@tabtypeid", new SqlParameter("@hid", hid), new SqlParameter("@posid", posid), new SqlParameter("@refeid", refeid), new SqlParameter("@tabtypeid", tabtypeid)).FirstOrDefault();
        }

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
        public List<up_pos_list_TabStatusResult> GetPosTabStatusResult(string hid, string code, string refeid, string tabtypeid, string tabStatus, int pageIndex, int pageSize)
        {
            var list = _db.Database.SqlQuery<up_pos_list_TabStatusResult>("EXEC up_pos_list_TabStatus @hid=@hid,@code=@code,@refeid=@refeid,@tabtypeid=@tabtypeid,@tabStatus=@tabStatus", new SqlParameter("@hid", hid), new SqlParameter("@code", code), new SqlParameter("@refeid", refeid), new SqlParameter("@tabtypeid", tabtypeid), new SqlParameter("@tabStatus", tabStatus ?? "")).OrderBy(o => o.Seqid).ToList();
            return list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        /// <summary>
        /// 根据查询条件获取指定酒店下的餐台状态列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="code">台号或台名</param>
        /// <param name="refeid">营业点id、代码或名称</param>
        /// <param name="tabtypeid">餐台类型id、代码或名称</param>
        /// <param name="tabStatus">餐台状态</param>
        /// <returns></returns>
        public int GetPosTabStatusResultTotal(string hid, string code, string refeid, string tabtypeid, string tabStatus)
        {
            var _count = _db.Database.SqlQuery<up_pos_list_TabStatusResult>("EXEC up_pos_list_TabStatus @hid=@hid,@code=@code,@refeid=@refeid,@tabtypeid=@tabtypeid,@tabStatus=@tabStatus", new SqlParameter("@hid", hid), new SqlParameter("@code", code), new SqlParameter("@refeid", refeid), new SqlParameter("@tabtypeid", tabtypeid), new SqlParameter("@tabStatus", tabStatus ?? "")).OrderBy(o => o.Seqid).Count();
            return _count;
        }

        /// <summary>
        /// 设置餐台预定状态
        /// </summary>
        /// <param name="hid"></param>
        public void SetTabReserveStatus(string hid)
        {
            _db.Database.ExecuteSqlCommand("exec up_pos_tabReserverStatusSet @hid=@hid",
                new SqlParameter("@hid", hid));
        }
    }
}