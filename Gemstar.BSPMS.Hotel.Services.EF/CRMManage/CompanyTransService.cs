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
    public class CompanyTransService : CRUDService<CompanyTrans>, ICompanyTransService
    {
        public CompanyTransService(DbHotelPmsContext db) : base(db, db.CompanyTranses)
        {
            _pmsContext = db;
        }
        protected override CompanyTrans GetTById(string id)
        {
            return new CompanyTrans { Id = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;
    }
}
