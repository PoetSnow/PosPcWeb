using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosCashier
{
    public class CashierViewModel
    {
        public string MBillid { get; set; }

        public string Billid { get; set; }

        public decimal? Amount { get; set; }

        public decimal? Itemid { get; set; }
        
        public int PageIndex { get; set; }
        
        public int PageSize { get; set; }
        
        public int PageTotal { get; set; }

        public List<PosItem> PayWayList { get; set; }

        public List<up_pos_list_BillDetailForPaymentResult> BillDetailList { get; set; }
    }
}