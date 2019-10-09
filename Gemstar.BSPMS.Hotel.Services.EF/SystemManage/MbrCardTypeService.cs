using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Common.Services.EF;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class MbrCardTypeService : CRUDService<MbrCardType>, IMbrCardTypeService
    {
        
        public MbrCardTypeService(DbHotelPmsContext db) : base(db, db.MbrCardTypes)
        {
            _pmsContext = db;
        } 
        public List<MbrCardType> GetMbrCardType(string hid)
        {
            return _pmsContext.MbrCardTypes.Where(w => w.Hid == hid).ToList();
        } 
        protected override MbrCardType GetTById(string id)
        {
            var shift = new MbrCardType { Id = id };
            return shift;
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 获取会员卡类型键值对信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public List<KeyValuePair<string, string>> List(string hid)
        {
            var list = _pmsContext.MbrCardTypes.Where(c => c.Hid == hid).OrderBy(w => w.Seqid).Select(c => new { c.Id, c.Name }).ToList();
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
        /// <summary>
        ///会员等级是否存在
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="seqit"></param>
        /// <returns></returns>
        public bool IsMbrCardTypeSeqit(string hid, int seqit,string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return _pmsContext.MbrCardTypes.AsNoTracking().Any(a => a.Hid == hid && a.Seqid == seqit);
            else
            {
                var data = _pmsContext.MbrCardTypes.AsNoTracking().FirstOrDefault(a => a.Hid == hid && a.Id == id);
                var result= _pmsContext.MbrCardTypes.AsNoTracking().FirstOrDefault(a => a.Hid == hid && a.Seqid==seqit);
                if (data != null && result != null)
                {
                    return data.Seqid != result.Seqid;
                }
                return false;
            }
        }
    }
}
