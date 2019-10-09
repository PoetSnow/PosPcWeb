using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosCashier
{
    public class MemberDiscountChangeModel
    {
        public string ItemID { get; set; }
        public bool IsItemClass { get; set; }
        public decimal DisCount { get; set; }
    }
}