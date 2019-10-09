using System;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services.EF;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class PmsHotelService : CRUDService<PmsHotel>, IPmsHotelService
    { 
        public PmsHotelService(DbHotelPmsContext db) : base(db, db.PmsHotels)
        {
            _pmsContext = db;
        }
        public List<PmsHotel> GetHotelsInGroup(string grpid)
        {
            return _pmsContext.PmsHotels.Where(w => w.Grpid == grpid).OrderBy(w=>w.Seqid).ToList();
        }
        /// <summary>
        /// 获取指定集团下除集团管理公司外的所有分店列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <returns>集团下除集团管理公司外的所有分店列表</returns>
        public List<PmsHotel> GetHotelsInGroupExceptGroupHotel(string grpid)
        {
            return _pmsContext.PmsHotels.Where(w => w.Grpid == grpid && w.Hid != grpid).OrderBy(w=>w.Seqid).ToList();
        }
        /// <summary>
        /// 获取指定集团下除集团管理公司外的指定管理类型的分店列表
        /// </summary>
        /// <param name="grpid">集团id</param>
        /// <param name="manageType">管理类型代码，比如直营，加盟等对应的代码</param>
        /// <returns>集团下除集团管理公司外的所有分店列表</returns>
        public List<PmsHotel> GetHotelsInGroupExceptGroupHotel(string grpid, string manageType)
        {
            return _pmsContext.PmsHotels.Where(w => w.Grpid == grpid && w.Hid != grpid && w.ManageType == manageType).OrderBy(w => w.Seqid).ToList();
        }

        protected override PmsHotel GetTById(string id)
        {
            throw new NotImplementedException();
        }

        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 是否开启总裁驾驶舱
        /// </summary>
        /// <param name="hid">指定酒店ID</param>
        /// <returns></returns>
        public bool IsOpenAnalysis(string hid)
        {
            int count = _pmsContext.Database.SqlQuery<int>("select count(1) from pmsHotel where hid=@hid and isOpenAnalysis = 1;", new System.Data.SqlClient.SqlParameter("@hid", hid)).FirstOrDefault();
            if(count == 1)
            {
                return true;
            }
            return false;
        }
    }
}
