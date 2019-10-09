using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Tools;
using System;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class CompanyLogService : CRUDService<CompanyLog>, ICompanyLogService
    {
        public CompanyLogService(DbHotelPmsContext db, ICurrentInfo currentInfo) : base(db, db.CompanyLogs)
        {
            _pmsContext = db;
            _currentInfo = currentInfo;
        }
        protected override CompanyLog GetTById(string id)
        {
            return new CompanyLog { Id = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;
        private ICurrentInfo _currentInfo;

        /// <summary>
        /// 简单增加对象
        /// </summary>
        /// <param name="companyid">合约单位ID</param>
        /// <param name="type">更改类型</param>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        public void AddSimple(Guid companyid, string type, string oldValue, string newValue)
        {
            _pmsContext.CompanyLogs.Add(new CompanyLog
            {
                Cdate = DateTime.Now.ToString(),
                Hid = _currentInfo.HotelId,
                Id = Guid.NewGuid(),
                InputUser = _currentInfo.UserCode,
                Companyid = companyid,
                Type = type,
                New = newValue,
                Old = oldValue
            });
        }

    }
}
