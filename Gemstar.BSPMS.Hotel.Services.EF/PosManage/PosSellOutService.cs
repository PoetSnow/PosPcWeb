using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class PosSellOutService : CRUDService<PosSellout>, IPosSellOutService
    {
        private DbHotelPmsContext _db;
        public PosSellOutService(DbHotelPmsContext db) : base(db, db.PosSellouts)
        {
            _db = db;
        }
        protected override PosSellout GetTById(string id)
        {
            return new PosSellout { Id = new Guid(id) };
        }

        /// <summary>
        /// 根据沽清表的数据查询出消费项目
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <returns></returns>
        public List<up_list_positemBySellOutResult> GetItemListBySellOut(string hid, string module, string refeId)
        {
            //获取沽清表中的数据集合
            var list = _db.Database.SqlQuery<up_list_positemBySellOutResult>("exec  up_list_positemBySellOut @hid=@hid,@Module=@Module",
                  new SqlParameter("@hid", hid),
                  new SqlParameter("@Module", module),
                  new SqlParameter("@refeId", refeId)
                ).ToList();
            return list;

        }

        /// <summary>
        /// 根据酒店ID，项目ID，单位ID，与营业点ID判断沽清表是否有数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="unitId">单位ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <returns></returns>
        public bool IsExists(string hid, string itemId)
        {
            return _db.PosSellouts.Any(m => m.Hid == hid && m.ItemId == itemId);
        }


        /// <summary>
        /// 根据酒店ID，项目ID获取数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <returns></returns>
        public PosSellout GetPosSelloutByItemId(string hid, string itemId)
        {
            return _db.PosSellouts.Where(m => m.Hid == hid && m.ItemId == itemId ).FirstOrDefault();
        }

        /// <summary>
        /// 根据酒店ID，项目ID，单位ID，与营业点Id获取数据
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="unitId">单位ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <returns></returns>
        public List<PosSellout> GetPosSelloutListByItemId(string hid, string itemId)
        {
            return _db.PosSellouts.Where(m => m.Hid == hid && m.ItemId == itemId).ToList();
        }
    }
}
