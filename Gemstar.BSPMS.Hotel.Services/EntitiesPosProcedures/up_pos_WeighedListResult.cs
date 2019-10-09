using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_WeighedListResult
    {
        public string billid { get; set; }
        public string billNo { get; set; }

        public string tabid { get; set; }

        public string tabNo { get; set; }

        public long? id { get; set; }

        public string itemCode { get; set; }

        public string itemName { get; set; }

        public decimal? price { get; set; }

        public decimal? quantity { get; set; }

        public decimal? dueamount { get; set; }

        public decimal? amount { get; set; }

        public string unitCode { get; set; }

        public string unitName { get; set; }

        public decimal? piece { get; set; }

        public decimal? oriQuan { get; set; }
    }
}
