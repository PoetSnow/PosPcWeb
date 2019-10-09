using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItem
{
    /// <summary>
    /// 批量修改使用
    /// </summary>
    public class PosItemRefeEditAll
    {
        /// <summary>
        /// 营业点Id
        /// </summary>
        public string refeId { get; set; }

     
        /// <summary>
        /// 出品打印机
        /// </summary>
        public string ProdPrinter { get; set; }

        /// <summary>
        /// 市别
        /// </summary>
       
        public string Shuffleid { get; set; }

      
        /// <summary>
        /// 传菜打印机
        /// </summary>
        public string SentPrtNo { get; set; }
    }
}