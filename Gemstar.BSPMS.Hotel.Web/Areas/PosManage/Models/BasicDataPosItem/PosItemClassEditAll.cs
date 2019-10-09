using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItem
{
    /// <summary>
    /// 批量修改使用
    /// </summary>
    public class PosItemClassEditAll
    {
        /// <summary>
        /// 原项目大类ID
        /// </summary>
        public string itemClassId { get; set; }

        /// <summary>
        /// 是否分类
        /// </summary>
        public bool? IsSubClass { get; set; }


        /// <summary>
        /// 现项目大类ID
        /// </summary>
        public string newItemClassId { get; set; }
    }
}