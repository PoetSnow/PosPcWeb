using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Percentages
{
    public interface ISalesmanPlanService : ICRUDService<PercentagesPlanSalesman>
    {
        /// <summary>
        /// 是否存在重复记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="salesmanId">业务员ID</param>
        /// <param name="planDate">计划年月</param>
        /// <param name="planSource">计划内容</param>
        /// <param name="notId">不包括主键ID</param>
        /// <returns></returns>
        bool Exists(string hid, List<Guid> salesmanId, DateTime planDate, string planSource, Guid? notId = null);
    }
}
