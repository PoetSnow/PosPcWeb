using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 扫码点餐滚动菜式
    /// </summary>
    public class up_pos_MScrollItemListResult
    {
        public string itemId { get; set; }

        public string itemCode { get; set; }

        public string itemName { get; set; }

        public decimal? Price { get; set; }

        public bool? IsDiscount { get; set; }

        public bool? IsService { get; set; }

        public string UnitId { get; set; }

        public string unitName { get; set; }

        public string UnitCode { get; set; }

        public string fileName { get; set; }

        public string SubClassid { get; set; }

        public string SubClassName { get; set; }
    }
}
