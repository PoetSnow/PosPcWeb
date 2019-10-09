using System;
using Gemstar.BSPMS.Common.Services.Entities;

namespace Gemstar.BSPMS.Common.Services.EF
{
    public class TryInfoService : CRUDService<TryInfo>, ITryInfoService
    {
        public TryInfoService(DbCommonContext db) : base(db,db.TryInfos)
        {
        }

        protected override TryInfo GetTById(string id)
        {
            return new TryInfo { Id = Convert.ToInt32(id) };
        }
    }
}
