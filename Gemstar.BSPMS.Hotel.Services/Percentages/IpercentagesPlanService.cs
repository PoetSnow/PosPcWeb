using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Percentages
{
  public  interface IpercentagesPlanService : ICRUDService<percentagesPlan>
    {
        /// <summary>
        /// 批量增加销售员计划任务
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="salesid">销售员id</param>
        /// <param name="year">年份</param>
        /// <param name="plansource">提成内容</param>
        /// <param name="parr">每月任务额</param>
        /// <returns></returns>
        int setPercentagesPlanSalesmans(string hid,Guid? salesid,string year,string plansource,decimal?[] parr);
        /// <summary>
        /// 批量增加操作员计划任务
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="salesid">操作员id</param>
        /// <param name="year">年份</param>
        /// <param name="plansource">提成内容</param>
        /// <param name="parr">每月任务额</param>
        /// <returns></returns>
        int setPercentagesPlanOperators(string hid, Guid? Operatorid, string year, string plansource, decimal?[] parr);
    }
}
