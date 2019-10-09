using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItem
{
    public class PosItemEditAllView
    {
        /// <summary>
        /// 选中的消费项目ID
        /// </summary>
        public string itemIds { get; set; }

        /// <summary>
        /// 选中的其他设置Id
        /// </summary>
        public string otherSelects { get; set; }


        /// <summary>
        /// 单位的值
        /// </summary>
        public string unitVal { get; set; }

        /// <summary>
        /// 单位状态（Add：添加。Update:修改）
        /// </summary>

        public string isUnitStatus { get; set;  }


        /// <summary>
        /// 大类状态（Add：添加。Update:修改）
        /// </summary>
        public string isItemClassStatus { get; set; }

        /// <summary>
        /// 项目大类的值
        /// </summary>
        public string itemClassVal { get; set; }


        /// <summary>
        /// 营业点状态（Add：添加。Update:修改）
        /// </summary>
        public string isRefeStatus { get; set; }
        /// <summary>
        /// 营业点的值
        /// </summary>
        public string refeVal { get; set; }
    }
}