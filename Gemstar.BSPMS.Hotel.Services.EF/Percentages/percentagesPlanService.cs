using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.Percentages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Gemstar.BSPMS.Hotel.Services.EF.Percentages
{
    public class percentagesPlanService : CRUDService<percentagesPlan>, IpercentagesPlanService
    {
        private DbHotelPmsContext _pmsContext;

        public percentagesPlanService(DbHotelPmsContext db) : base(db, db.percentagesPlans)
        {
            _pmsContext = db;
        }

        public int setPercentagesPlanSalesmans(string hid, Guid? salesid, string year, string plansource, decimal?[] parr)
        {
            int i=_pmsContext.Database.ExecuteSqlCommand("exec up_Add_percentagesPlanSalesmans @h99hid={0},@m19业务员={1},@t00年份={2},@m36计划内容={3},@p1={4},@p2={5},@p3={6},@p4={7},@p5={8},@p6={9},@p7={10},@p8={11},@p9={12},@p10={13},@p11={14},@p12={15}",hid,salesid,year,plansource, parr[0], parr[1], parr[2], parr[3], parr[4], parr[5], parr[6], parr[7], parr[8], parr[9], parr[10], parr[11]);
            return i; 
        }
        public int setPercentagesPlanOperators(string hid, Guid? Operatorid, string year, string plansource, decimal?[] parr)
        {
            int i = _pmsContext.Database.ExecuteSqlCommand("exec up_Add_percentagesPlanOperators @h99hid={0},@m32操作员={1},@t00年份={2},@m39计划内容={3},@p1={4},@p2={5},@p3={6},@p4={7},@p5={8},@p6={9},@p7={10},@p8={11},@p9={12},@p10={13},@p11={14},@p12={15}", hid, Operatorid, year, plansource, parr[0], parr[1], parr[2], parr[3], parr[4], parr[5], parr[6], parr[7], parr[8], parr[9], parr[10], parr[11]);
            return i;
        }
        protected override percentagesPlan GetTById(string id)
        {
            return new percentagesPlan { Id = Guid.Parse(id) };
        }
    }
}
