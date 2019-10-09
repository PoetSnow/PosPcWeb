using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class PosBillCostService : CRUDService<PosBillCost>, IPosBillCostService
    {
        private DbHotelPmsContext _db;

        public PosBillCostService(DbHotelPmsContext db) : base(db, db.PosBillCosts)
        {
            _db = db;
        }

        protected override PosBillCost GetTById(string id)
        {
            return new PosBillCost { Id = new Guid(id) };
        }

        /// <summary>
        /// 获取物品数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="billId">账单明细ID</param>
        /// <param name="costItemid">物品Id</param>
        /// <returns></returns>
        public PosBillCost GetBillCost(string hid, string module, long billId, string costItemid)
        {
            return _db.PosBillCosts.Where(w => w.Hid == hid && w.PostSysName == module && w.BillID == billId && w.CostItemid == costItemid ).FirstOrDefault();
        }

        /// <summary>
        /// 获取仓库耗用数据列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="billId">账单明细ID</param>
        /// <returns></returns>
        public List<PosBillCost> GetBillCostList(string hid, string module, long billId)
        {
            return _db.PosBillCosts.Where(w => w.Hid == hid && w.PostSysName == module && w.BillID == billId).ToList();
        }

        /// <summary>
        /// 获取当前营业日已消耗的库存数量
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="billBsnsDate">营业日</param>
        /// <param name="whCode">二级仓库</param>
        /// <param name="costItemid">物品ID</param>
        /// <returns></returns>
        public decimal? GetBillCostSumQuantity(string hid, string module, DateTime? billBsnsDate, string whCode, string costItemid)
        {
            var result = _db.PosBillCosts.Where(w => w.Hid == hid && w.PostSysName == module && w.BillBsnsDate == billBsnsDate && w.WhCode == whCode && w.CostItemid == costItemid).Sum(w => w.Quantity);
            return result;
        }

        public decimal? GetBillCostSumQuantity(string hid, string module, DateTime? billBsnsDate, string whCode, string costItemid, long billId)
        {
            var result = _db.PosBillCosts.Where(w => w.Hid == hid && w.PostSysName == module && w.BillBsnsDate == billBsnsDate && w.WhCode == whCode && w.CostItemid == costItemid && w.BillID != billId).Sum(w => w.Quantity);
            return result;
        }


        public List<PosBillCost> GetPosBillCostByProc(string hid, string posid, DateTime? Business, string Module)
        {
            var result = _db.Database.SqlQuery<PosBillCost>("exec up_pos_ExpendItemByBusiDate @hid=@hid,@posId=@posId,@Business=@Business,@Module=@Module"
                , new SqlParameter("@hid", hid)
                 , new SqlParameter("@posId", posid)
                , new SqlParameter("@Business", Business)
                , new SqlParameter("@Module", Module)).ToList();
            return result;
        }
    }
}
