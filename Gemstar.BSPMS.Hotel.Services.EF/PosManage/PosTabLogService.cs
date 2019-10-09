using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos锁台服务实现
    /// </summary>
    public class PosTabLogService : CRUDService<PosTabLog>, IPosTabLogService
    {
        private DbHotelPmsContext _db;
        public PosTabLogService(DbHotelPmsContext db) : base(db, db.PosTabLogs)
        {
            _db = db;
        }

        protected override PosTabLog GetTById(string id)
        {
            return new PosTabLog { Id = new Guid(id) };
        }

        /// <summary>
        /// 判断指定的餐台id或者台号的锁台是否已经存在，用于防止餐台id和台号重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台id</param>
        /// <param name="tabNo">台号</param>
        /// <returns>true:在酒店中已经有相同餐台id或者相同台号的锁台了，false：没有相同的</returns>
        public bool IsExists(string hid, string refeid, string tabid, string tabNo)
        {
            return _db.PosTabLogs.Any(w => w.Hid == hid && w.Refeid == refeid && (w.Tabid == tabid || w.TabNo == tabNo));
        }

        /// <summary>
        /// 获取指定酒店、营业点、餐台id或台号下的锁定信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台id</param>
        /// <param name="tabNo">台号</param>
        /// <returns>锁台信息</returns>
        public PosTabLog GetPosTabLogByTab(string hid, string refeid, string tabid, string tabNo)
        {
            return _db.PosTabLogs.Where(w => w.Hid == hid && w.Refeid == refeid && (w.Tabid == tabid || w.TabNo == tabNo)).FirstOrDefault();
        }

        /// <summary>
        /// 获取指定酒店、营业点、餐台id或台号下的锁定信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabid">餐台id</param>
        /// <param name="tabNo">台号</param>
        /// <returns>锁台信息</returns>
        public List<PosTabLog> GetPosTabLogListByTab(string hid, string refeid, string tabid, string tabNo)
        {
            return _db.PosTabLogs.Where(w => w.Hid == hid && (w.Refeid == refeid || refeid == "") && (w.Tabid == tabid || w.TabNo == tabNo)).ToList();
        }

        /// <summary>
        /// 根据指定酒店、营业点、计算机和操作员获取锁定信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="computer">计算机</param>
        /// <param name="transUser">操作员</param>
        /// <returns></returns>
        public List<PosTabLog> GetPosTabLogListByUser(string hid, string refeid, string computer, string transUser)
        {
            return _db.PosTabLogs.Where(w => w.Hid == hid && (w.Refeid == refeid) && w.Computer == computer && w.TransUser == transUser).ToList();
        }

        /// <summary>
        /// 根据酒店ID与营业点ID获取
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeid"></param>
        /// <returns></returns>
        public List<up_pos_listTabLogResult> GetPosTabLogListByRefeId(string hid, string refeid)
        {
            return _db.Database.SqlQuery<up_pos_listTabLogResult>("exec up_pos_listTabLog @hid=@hid,@refeId=@refeId", new SqlParameter("@hid", hid), new SqlParameter("@refeId", refeid)).ToList();


        }

        /// <summary>
        /// 根据账单ID获取锁台记录
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        public PosTabLog GetPosTabLogByBillId(string hid, string billId)
        {
            return _db.PosTabLogs.Where(w => w.Hid == hid && w.Billid==billId).FirstOrDefault();
        }
    }
}
