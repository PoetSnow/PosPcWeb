using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class PosCostItemService : CRUDService<PostCost>, IPosCostItemService
    {
        private DbHotelPmsContext _db;
        public PosCostItemService(DbHotelPmsContext db) : base(db, db.PostCosts)
        {
            _db = db;
        }


        #region 查询
        /// <summary>
        /// 获取库存物品集合根据项目id
        /// </summary>
        /// <param name="hId"></param>
        /// <param name="itemId">物品id</param>
        /// <returns></returns>
        public List<PostCost> GetListPostCostByItemId(string hId, string itemId)
        {
            return _db.PostCosts.Where(x => x.Hid == hId && x.Itemid == itemId).ToList();
        }

        /// <summary>
        /// 获取库存物品信息根据id
        /// </summary>
        /// <param name="hId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public PostCost GetPostCostInfoByKey(string hId, Guid id)
        {
            return _db.PostCosts.Where(x => x.Hid == hId && x.Id == id).FirstOrDefault();
        }

        #endregion

        protected override PostCost GetTById(string id)
        {
            return new PostCost { Id = Guid.Parse(id) };
        }

        /// <summary>
        /// 根据酒店ID，项目ID，单位ID 获取物品列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="unitId">单位ID</param>
        /// <returns></returns>
        public List<PostCost> GetListPostCostByItemId(string hid, string itemId, string unitId)
        {
            return _db.PostCosts.Where(x => x.Hid == hid && x.Itemid == itemId && x.Unitid == unitId).ToList();
        }
    }
}
