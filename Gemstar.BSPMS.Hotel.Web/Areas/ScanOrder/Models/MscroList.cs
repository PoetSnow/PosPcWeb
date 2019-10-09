using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models
{
    /// <summary>
    /// 滚动菜式
    /// </summary>
    public class MscroList
    {

        public Guid Id { get; set; }


        public string Hid { get; set; }

        public string Itemid { get; set; }


        public string ItemCode { get; set; }


        public string ItemName { get; set; }


        public string Unitid { get; set; }


        public string UnitName { get; set; }


        public string FileName { get; set; }

        public int? OrderBy { get; set; }


        public string Remark { get; set; }

        public string Creator { get; set; }


        public DateTime? Createdate { get; set; }

        public decimal? Price { get; set; }
    }
}