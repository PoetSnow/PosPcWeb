using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.Percentages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EF.Percentages
{
    public class OperatorPlanService : CRUDService<PercentagesPlanOperator>, IOperatorPlanService
    {
        public OperatorPlanService(DbHotelPmsContext db) : base(db, db.PercentagesPlanOperators)
        {
            _pmsContext = db;
        }
        protected override PercentagesPlanOperator GetTById(string id)
        {
            return new PercentagesPlanOperator { PlanId = Guid.Parse(id) };
        }
        private DbHotelPmsContext _pmsContext;

        /// <summary>
        /// 是否存在重复记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="OperatorId">业务员ID</param>
        /// <param name="planDate">计划年月</param>
        /// <param name="planSource">计划内容</param>
        /// <param name="notId">不包括主键ID</param>
        /// <returns></returns>
        public bool Exists(string hid, List<Guid> OperatorIds, DateTime planDate, string planSource, Guid? notId = null)
        {
            Expression<Func<PercentagesPlanOperator, bool>> predicate = c => c.Hid == hid
            && OperatorIds.Contains(c.OperatorId)
            && c.PlanDate == planDate
            && c.PlanSource == planSource
            && (notId == null ? true : c.PlanId != notId);
            return _pmsContext.PercentagesPlanOperators.Any(predicate);
        }

    }
}
