using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_list_BillDetailByGroupItemClassResult
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string status { get; set; }

        public decimal? sumNum { get; set; }

        public decimal? sumDueamount { get; set; }

        public decimal? sumAmount { get; set; }
        public decimal? sumTaxAmount { get; set; }

    }
}
