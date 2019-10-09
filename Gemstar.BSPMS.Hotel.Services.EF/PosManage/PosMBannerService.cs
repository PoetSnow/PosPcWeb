using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// Pos扫码点餐Banner
    /// </summary>
    public class PosMBannerService : CRUDService<PosMBanner>, IPosMBannerService
    {
        private DbHotelPmsContext _db;

        public PosMBannerService(DbHotelPmsContext db) : base(db, db.PosMBanners)
        {
            _db = db;
        }

        protected override PosMBanner GetTById(string id)
        {
            return new PosMBanner { Id = Guid.Parse(id) };
        }

        /// <summary>
        /// 判断指定Id的Banner是否已经存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsExists(string hid, string id)
        {
            return _db.PosAcTypes.Any(w => w.Hid == hid && id.Contains(w.Id));
        }

        /// <summary>
        /// 获取Banner列表
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public List<PosMBanner> GetMBannerList(string hid)
        {
            return _db.PosMBanners.Where(w => w.Hid == hid).ToList();
        }
    }
}