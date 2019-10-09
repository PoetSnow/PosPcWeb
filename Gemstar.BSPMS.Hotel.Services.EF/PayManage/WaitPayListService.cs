using System;
using System.Linq;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    public class WaitPayListService : CRUDService<WaitPayList>, IWaitPayListService
    {

        private DbHotelPmsContext dbHotelPms;

        public WaitPayListService(DbHotelPmsContext db) : base(db, db.WaitPayLists)
        {
            dbHotelPms = db;
        }

        protected override WaitPayList GetTById(string id)
        {
            return new WaitPayList
            {
                WaitPayId = Guid.Parse(id)
            };
        }

        /// <summary>
        /// 获取指定酒店指定ID的待支付记录
        /// </summary>
        /// <param name="id">待支付记录ID</param>
        /// <param name="producttype">产品类型</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        public WaitPayList GetWaitPayList(string id,string producttype,byte status)
        {
            Guid res = Guid.Empty;
            var guid = Guid.TryParse(id,out  res);
            return dbHotelPms.WaitPayLists.Where(u=>u.WaitPayId == res && u.ProductType == producttype && u.Status == status).FirstOrDefault();
        }


    }
}
