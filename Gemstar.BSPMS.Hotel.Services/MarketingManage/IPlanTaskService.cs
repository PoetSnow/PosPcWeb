using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.MarketingManage
{
    public interface IPlanTaskService : ICRUDService<PlanTask>
    {
        /// <summary>
        /// 增加计划任务
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="ptype">任务类型</param>
        /// <param name="pdate">日期</param>
        /// <param name="amount">收入</param>
        /// <returns></returns>
        JsonResultData setPlanTask(string hid, string ptype, string begindate, string enddate, string amount);

    }
}
