using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EF.MarketingManage
{
    public class PlanTaskService: CRUDService<PlanTask>, IPlanTaskService
    {
        private DbHotelPmsContext _db;
        public PlanTaskService(DbHotelPmsContext db) : base(db, db.PlanTasks)
        {
            _db = db;
        }

        public JsonResultData setPlanTask(string hid, string ptype, string begindate, string enddate, string amount)
        {
            try
            {
                string[] arr = amount.Trim(',').Split(',');
                int a = _db.Database.ExecuteSqlCommand("exec up_plantaskinput @h99hid={0},@t00任务类型={1},@begindate={2},@endDate={3},@q1={4},@q2={5},@q3={6},@q4={7},@q5={8},@q6={9},@q7={10}", hid, ptype, DateTime.Parse(begindate), DateTime.Parse(enddate), arr[0], arr[1], arr[2], arr[3], arr[4], arr[5], arr[6]);

               // int i = _db.Database.ExecuteSqlCommand("exec up_plantaskinput @h99hid={0},@t00任务类型={1},@t00年月={2},@t00收入={3}", hid, ptype, pdate, amount);
                return JsonResultData.Successed("保存成功");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }

        }

        protected override PlanTask GetTById(string id)
        {
            return new PlanTask() { id =Guid.Parse(id) };
        }
    }
}
