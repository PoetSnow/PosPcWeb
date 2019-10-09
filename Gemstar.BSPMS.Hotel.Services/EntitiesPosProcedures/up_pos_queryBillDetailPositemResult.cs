using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    //消费项目的消费项目
   public class up_pos_queryBillDetailPositemResult
    {
        //i.id ,i.Cname ,i.ItemClassid,d.batchTime,d.discount,d.billid

        public string ItemID { get; set; }
        public string Cname { get; set; }
        public string ItemClassid { get; set; }
        public string Batch { get; set; }
        public decimal Discount { get; set; }
        public decimal? Price { get; set; }
        public decimal? Amount { get; set; }
        public string Billid { get; set; }
        public string UnitID { get; set; }
        public string UnitName { get; set; }
        public decimal? Quantity { get; set; }
        public bool ItemIsCanDiscount { get; set; }




    }
}
