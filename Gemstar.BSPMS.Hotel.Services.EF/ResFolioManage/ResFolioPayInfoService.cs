using System;
using System.Linq;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.ResFolioManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.ResFolioManage
{
    public class ResFolioPayInfoService : CRUDService<ResFolioPayInfo>, IResFolioPayInfoService
    {
        public ResFolioPayInfoService(DbHotelPmsContext db) : base(db, db.ResFolioPayInfos)
        {
            _pmsContext = db;
        }

        protected override ResFolioPayInfo GetTById(string id)
        {
            return new ResFolioPayInfo
            {
                Id = Convert.ToInt32(id)
            };
        }

        /// <summary>
        /// 获取指定酒店的指定id的待支付记录
        /// </summary>
        /// <param name="id">待支付id</param>
        /// <param name="hid">酒店id</param>
        /// <returns>如果存在，则为存在的对象，不存在则为null</returns>
        public ResFolioPayInfo GetHotelPayInfo(int id, string hid)
        {
            return _pmsContext.ResFolioPayInfos.SingleOrDefault(w=>w.Hid == hid && w.Id == id);
        }


        public ResFolioPayInfo GetHotelPayInfo(string hid,Func<ResFolioPayInfo,bool> wherefunc)
        {
            return _pmsContext.ResFolioPayInfos.Where(u=>u.Hid == hid).FirstOrDefault(wherefunc);
        }


        private DbHotelPmsContext _pmsContext;
    }
}
