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
    /// pos营业厅服务实现
    /// </summary>
    public class PosItemRefeService : CRUDService<PosItemRefe>, IPosItemRefeService
    {
        private DbHotelPmsContext _db;
        public PosItemRefeService(DbHotelPmsContext db) : base(db, db.PosItemRefes)
        {
            _db = db;
        }
        protected override PosItemRefe GetTById(string id)
        {
            return new PosItemRefe { Id = new Guid(id) };
        }
        /// <summary>
        /// 判断指定的代码或者名称的营业厅是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="refe">营业厅</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的营业厅了，false：没有相同的</returns>
        public bool IsExists(string hid, string itemId, string refe)
        {
            return _db.PosItemRefes.Any(w => w.Hid == hid && w.Itemid == itemId && w.Refeid == refe);
        }
        /// <summary>
        /// 判断指定的代码或者名称的营业厅是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目id</param>
        /// <param name="refe">营业厅</param>
        /// <param name="exceptId">要排队的营业厅id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的营业厅了，false：没有相同的</returns>
        public bool IsExists(string hid, string itemId, string refe, Guid exceptId)
        {
            return _db.PosItemRefes.Any(w => w.Hid == hid && w.Id != exceptId && w.Itemid == itemId && w.Refeid == refe);
        }
        /// <summary>
        /// 获取指定酒店和模块下的营业厅列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">模块代码</param>
        /// <returns>指定酒店和模块下的营业厅列表</returns>
        public List<PosItemRefe> GetPosItemClassByModule(string hid, string itemId)
        {
            return _db.PosItemRefes.Where(w => w.Hid == hid && w.Itemid == itemId).ToList();
        }
        /// <summary>
        /// 获取指定酒店和项目下的对应营业点列表
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="itemId">项目Id</param>
        /// <returns>指定酒店和项目下的对应营业点列表</returns>
        public List<up_pos_list_ItemRefeByItemidResult> GetPosItemRefeByItemId(string hid, string itemId)
        {
            var results = _db.Database.SqlQuery<up_pos_list_ItemRefeByItemidResult>("exec up_pos_list_ItemRefeByItemid @hid=@hid,@itemID=@itemID", new SqlParameter("@hid", hid), new SqlParameter("@itemID", itemId)).ToList();
            return results;
        }

        /// <summary>
        /// 获取酒店和项目下的对应营业点列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">项目Id</param>
        /// <returns>酒店和项目下的对应营业点列表</returns>
        public List<PosItemRefe> GetPosItemRefeForCopy(string hid, string itemId)
        {
            return _db.PosItemRefes.Where(w => w.Hid == hid && w.Itemid == itemId).ToList();
        }

        public PosItemRefe GetPosItemRefeByEditAll(string hid, string itemId, string refeId)
        {
            return _db.PosItemRefes.Where(w => w.Hid == hid && w.Itemid == itemId && w.Refeid == refeId).FirstOrDefault();
        }
    }
}
