using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    public class ResDetailPlanInfo
    {
        /// <summary>
        /// 登记号ID
        /// </summary>
        public string Regid { get; set; }
        
        /// <summary>
        /// 日期
        /// </summary>
        public string Ratedate { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 源价格（价格代码对应的价格）
        /// </summary>
        public decimal? OriginPrice { get; set; }
    }
}
