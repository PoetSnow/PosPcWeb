using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class PosReserveService : CRUDService<PosBill>, IPosReserveService
    {
        private DbHotelPmsContext _db;

        public PosReserveService(DbHotelPmsContext db) : base(db, db.PosBills)
        {
            _db = db;
        }

        protected override PosBill GetTById(string id)
        {
            return new PosBill { Billid = id };
        }

        /// <summary>
        /// 获取预定餐台状态
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="tabTypeId">餐台类型</param>
        /// <param name="TimeList">时间段（营业点市别的最早时间到最晚时间）</param>
        /// <param name="Business">营业日</param>
        /// <returns></returns>
        public List<up_pos_ReserveTabStatusResult> GetReserveTabStatus(string hid, string refeId, string tabTypeId, string TimeList, DateTime Business, string Flag)
        {
            var result = _db.Database.SqlQuery<up_pos_ReserveTabStatusResult>("exec up_pos_ReserveTabStatus @hid=@hid,@refeId=@refeId,@tabTypeId=@tabTypeId,@TimeList=@TimeList,@Business=@Business,@Flag=@Flag",
                new SqlParameter("@hid", hid),
                new SqlParameter("@refeId", refeId),
                new SqlParameter("@tabTypeId", tabTypeId),
                new SqlParameter("@TimeList", TimeList),
                new SqlParameter("@Business", Business),
                new SqlParameter("@Flag", Flag)).ToList();
            return result;
        }



        /// <summary>
        /// 统计餐台类型该时间段可用的餐台数量
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="module">模块</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="Business">时间</param>
        /// <returns></returns>
        public List<up_pos_ReserveTabTypeListResult> GetReserveTabTypeInfo(string hid, string module, string refeId, DateTime ReserveDate)
        {
            var result = _db.Database.SqlQuery<up_pos_ReserveTabTypeListResult>("exec up_pos_ReserveTabTypeList @hid=@hid,@module=@module,@refeId=@refeId,@ReserveDate=@ReserveDate",
               new SqlParameter("@hid", hid),
               new SqlParameter("@module", module),
               new SqlParameter("@refeId", refeId),
               new SqlParameter("@ReserveDate", ReserveDate)).ToList();
            return result;
        }

        /// <summary>
        /// 根据餐台类型获取该时段所有餐台信息
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="module">模块</param>
        /// <param name="refeId">营业点Id</param>
        /// <param name="ReserveDate">预抵日期</param>
        /// <param name="tabTypeId">餐台类型Id</param>
        /// <returns></returns>
        public List<PosTab> GetTabListByTabTypeId(string hid, string module, string refeId, DateTime ReserveDate, string tabTypeId)
        {
            var result = _db.Database.SqlQuery<PosTab>("exec up_pos_ReserveTabListForTabType @hid=@hid,@module=@module,@refeId=@refeId,@ReserveDate=@ReserveDate,@tabTypeId=@tabTypeId",
              new SqlParameter("@hid", hid),
              new SqlParameter("@module", module),
              new SqlParameter("@refeId", refeId),
              new SqlParameter("@ReserveDate", ReserveDate),
               new SqlParameter("@tabTypeId", tabTypeId)).ToList();
            return result;
        }

        /// <summary>
        /// 获取餐台下所有预定账单
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="tabId">餐台Id</param>
        /// <returns></returns>
        public List<PosBill> GetOrderBillByTabId(string hid, string tabId)
        {
            var result = _db.PosBills.Where(w => w.Hid == hid && w.Tabid == tabId && w.IsOrder == true && w.Status == (byte)PosBillStatus.预订 && w.OrderDate >= DateTime.Now).ToList();
            return result;
        }

        /// <summary>
        /// 根据日期获取所有的预订账单
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="tabId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<PosBill> GetOrderBillByTabId(string hid, string tabId, DateTime date)
        {
            var result = _db.PosBills.Where(w => w.Hid == hid && w.Tabid == tabId && w.IsOrder == true && w.Status == (byte)PosBillStatus.预订 && DbFunctions.TruncateTime(w.OrderDate) == date).ToList();
            return result;
        }


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
        public List<up_pos_OrderBillListByDateResult> GetOrderBillList(string hid, string refeId, DateTime business, string tabTypeId, string Flag, string status)
        {
            var result = _db.Database.SqlQuery<up_pos_OrderBillListByDateResult>("exec up_pos_OrderBillListByDate @hid=@hid,@refeId=@refeId,@tabTypeId=@tabTypeId,@status=@status,@Business=@Business,@Flag=@Flag",
                new SqlParameter("@hid", hid),
                new SqlParameter("@refeId", refeId),
                new SqlParameter("@tabTypeId", tabTypeId),
                new SqlParameter("@status", status),
                new SqlParameter("@Business", business),
                new SqlParameter("@Flag", Flag)).ToList();
            return result;
        }

        /// <summary>
        /// 获取预订账单信息
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="billId"></param>
        /// <returns></returns>
        public PosBill GetBillOrder(string hid, string billId)
        {
            var result = _db.PosBills.Where(w => w.Hid == hid && w.IsOrder == true && w.Billid == billId).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 获取预定账单信息
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="tabId">餐台Id</param>
        /// <param name="business">预订日期</param>
        /// <param name="refeId">营业点Id</param>
        /// <returns></returns>
        public PosBill GetBillOrder(string hid, string tabId, DateTime stime,DateTime eTime, string refeId)
        {
            var result = _db.PosBills.Where(w => w.Hid == hid && w.IsOrder == true && w.Tabid == tabId && w.Refeid == refeId && w.Status == (byte)PosBillStatus.预订
                                && w.OrderDate > stime && w.OrderDate <= eTime).OrderBy(w => w.OrderDate).FirstOrDefault();
            return result;

        }


    }
}
