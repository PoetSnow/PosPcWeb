using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class CouponService : CRUDService<Coupon>, ICouponService
    {
        public CouponService(DbHotelPmsContext db) : base(db, db.Coupons)
        {
            _pmsContext = db;
        }
        protected override Coupon GetTById(string id)
        {
            return new Coupon { Id = id };
        }
        private DbHotelPmsContext _pmsContext;
        public List<Coupon> GetCouponbyitemTypeid(string hid, int pk)
        {
            CodeList cl = _pmsContext.CodeLists.Where(w => w.Pk == pk && w.Hid == hid).FirstOrDefault();
            List<Coupon> list = _pmsContext.Coupons.Where(w => w.ItemTypeid == cl.Id && w.Hid == hid).ToList();
            return list;
        }
        public JsonResultData Enable(string id)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var mctype = _pmsContext.Coupons.Find(ids[i]);
                    _pmsContext.Entry(mctype).State = EntityState.Modified;
                    mctype.Status = EntityStatus.启用;
                }
                _pmsContext.AddDataChangeLogs(OpLogType.优惠券启用禁用);
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
                throw;
            }
        }

        public JsonResultData Disable(string id)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var mctype = _pmsContext.Coupons.Find(ids[i]);
                    _pmsContext.Entry(mctype).State = EntityState.Modified;
                    mctype.Status = EntityStatus.禁用;
                }
                _pmsContext.AddDataChangeLogs(OpLogType.优惠券启用禁用);
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
                throw;
            }
        }
        public List<KeyValuePair<string, string>> List(string hid)
        {
            var list = _pmsContext.Coupons.Where(c => c.Hid == hid && c.Status < EntityStatus.禁用).OrderBy(w => w.Seqid).Select(c => new { c.Id, c.Name }).ToList();
            List<KeyValuePair<string, string>> returnList = new List<KeyValuePair<string, string>>();
            if (list != null && list.Count > 0)
            {

                foreach (var item in list)
                {
                    returnList.Add(new KeyValuePair<string, string>(item.Id, item.Name));
                }
            }
            return returnList;
        }
        public bool isExistTicket(string id)
        {
            var ids = id.Split(',');
            string ticketId;
            for (int i = 0; i < ids.Length; i++)
            {
                ticketId = ids[i];
                var pro = _pmsContext.ProfileCards.FirstOrDefault( p=> p.TicketTypeid == ticketId);
                if (pro != null) return false;
            }
            return true;
        }
    }
}
