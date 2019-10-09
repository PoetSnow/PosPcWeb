using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Common.Tools;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Gemstar.BSPMS.Common.Services.EF
{
    public class MasterService : IMasterService
    {
        DbCommonContext _db;
        public MasterService(DbCommonContext db)
        {
            _db = db;
        }

        public IQueryable<Province> GetProvince()
        {
            return _db.Provinces;
        }

        public Province GetProvince(string provinceCode)
        {
            return _db.Provinces.Find(provinceCode);
        }

        public List<CityMaster> GetCity()
        {
            return _db.Database.SqlQuery<CityMaster>("select * from v_city").ToList();
        }

        public List<CityMaster> GetCity(string provinceCode, string cityCode = null)
        {
            string sql = "select * from v_city where provinceCode=@provinceCode";
            if (!string.IsNullOrWhiteSpace(cityCode))
            {
                sql += " and code=@code";
                return _db.Database.SqlQuery<CityMaster>(sql, new System.Data.SqlClient.SqlParameter("@provinceCode", provinceCode), new System.Data.SqlClient.SqlParameter("@code", cityCode)).ToList();
            }
            return _db.Database.SqlQuery<CityMaster>(sql, new System.Data.SqlClient.SqlParameter("@provinceCode", provinceCode)).ToList();
        }

        public IList<StarLevel> GetStarLevel()
        {
            var sql = "select * from uv_starLevel";
            var result = ADOHelper.ExecSql(sql, _db.Database.Connection.ConnectionString).ToList<StarLevel>();
            return result;
        }

        public IQueryable<AdSet> GetAdSet(string position)
        {
            return _db.AdSets.Where(c => c.Position == position).OrderBy(c => c.Seqid);
        }

        public List<M_v_channelCode> GetM_v_channelCode()
        {
            return _db.Database.SqlQuery<M_v_channelCode>("select * from m_V_channelCode").ToList(); 
        }

        public int updateHotelChannel(string hid, string code, string refno)
        {
            refno = (refno == null ? "" : refno);
            return _db.Database.ExecuteSqlCommand("update hotelChannel set refno={0} where hid={1} and channelcode={2}", refno, hid, code);
        }

        public string GetHotelOnlineLockType(string hid)
        {
            if (string.IsNullOrWhiteSpace(hid)) { return null; }
            return _db.Hotels.Where(c => c.Hid == hid).Select(c => c.OnlineLockType).FirstOrDefault();
        }

        public string GetSysParaValue(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) { return null; }
            return _db.SysParas.Where(c => c.Code == code).Select(c => c.Value).FirstOrDefault();
        }
        public string GetHotelItemAction(string hid)
        {
            if (string.IsNullOrWhiteSpace(hid)) { return null; }
            return _db.Hotels.Where(c => c.Hid == hid).Select(c => c.ItemAction).FirstOrDefault();
        }

    }
}
