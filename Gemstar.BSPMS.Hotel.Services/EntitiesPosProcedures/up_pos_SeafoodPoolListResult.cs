using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 海鲜池数据列表
    /// </summary>
    public class up_pos_SeafoodPoolListResult
    {
        public long id { get; set; }

        public string hid { get; set; }

        public string billid { get; set; }

        public string itemid { get; set; }

        public string itemCode { get; set; }

        public string itemName { get; set; }

        public decimal? oriQuan { get; set; }

        public decimal? quantity { get; set; }

        public string tabid { get; set; }

        public string tabNo { get; set; }

        public string tabName { get; set; }

        public DateTime? transDate { get; set; }

        public string unitName { get; set; }

        public decimal? piece { get; set; }

    }
}
