using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Services.SystemManage
{
   public interface ICancelRuleService
    {
        /// <summary>
        ///   获取酒店下的取消政策
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        SelectList getCancelRule(string hid);
    }
}
