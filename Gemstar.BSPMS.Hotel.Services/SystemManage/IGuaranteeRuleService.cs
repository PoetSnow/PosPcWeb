using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Services.SystemManage
{
   public interface IGuaranteeRuleService
    {
        /// <summary>
        /// 获取酒店的担保政策
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        SelectList getGuaranteeRulelist(string hid);
    }
}
