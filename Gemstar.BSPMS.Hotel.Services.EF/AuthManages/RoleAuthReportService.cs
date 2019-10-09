using Gemstar.BSPMS.Hotel.Services.AuthManages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services.EF;
using System.Data.Entity;

namespace Gemstar.BSPMS.Hotel.Services.EF.AuthManages
{
    public class RoleAuthReportService : CRUDService<RoleAuthReport>, IRoleAuthReportService
    {

        public RoleAuthReportService(DbHotelPmsContext db) : base(db, db.RoleAuthReports)
        {
            _pmsContext = db;
        }

        public List<RoleAuthReport> GetAllRoleAuthReports(string hid, string roleid)
        {
            return _pmsContext.RoleAuthReports.Where(w => w.Grpid == hid && w.RoleId == Guid.Parse(roleid)).ToList();
        }

        public List<RoleAuthReport> GetReportsIsAllow(string hid, string roleid)
        {
            Guid roleId = Guid.Parse(roleid);
            return _pmsContext.RoleAuthReports.Where(w => w.Grpid == hid && w.RoleId == roleId && w.IsAllow == true).ToList();
        }

        protected override RoleAuthReport GetTById(string id)
        {
            var roleAuthReport = new RoleAuthReport { IsAllow =bool.Parse(id) };
            return roleAuthReport;
        }

        public List<V_ReportList> GetV_ReportLists(ProductType productType,string hid, string roleid)
        {
            return _pmsContext.Database.SqlQuery<V_ReportList>("exec up_v_rolereportlist @hid={0},@roleid={1},@productType={2}", hid, roleid,(int)productType).ToList();
        }

        public string ChangeRoleAuthReport(ProductType productType,string hid, string roleid, string reportcodes)
        {
            reportcodes = reportcodes.Trim(',');
            int a = _pmsContext.Database.ExecuteSqlCommand("exec up_updateroleauthreport @hid={0},@roleid={1},@reportcode={2},@productType={3}", hid, roleid, reportcodes,(int)productType); 
            return a > 0? "成功" : "失败";
        }
        
        private DbHotelPmsContext _pmsContext;
    }
}
