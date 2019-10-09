using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosSeafoodPool
{
    public class SeaFoodPoolQueryViewModel
    {

        public SeaFoodPoolQueryViewModel()
        {
            TabId = "";
            PageIndex = 1;
            PageSize = 1;
            PageTotal = 0;

        }

        /// <summary>
        /// 餐台ID
        /// </summary>
        public string TabId { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageTotal { get; set; }
    }
}