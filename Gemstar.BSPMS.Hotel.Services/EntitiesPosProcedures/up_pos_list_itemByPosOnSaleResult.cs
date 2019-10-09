using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 特价菜消费项目返回结果
    /// </summary>
    public class up_pos_list_itemByPosOnSaleResult
    {
        public Guid id { get; set; }
        public string itemId { get; set; }
        public string code { get; set; }

        public string Cname { get; set; }

        public decimal? price { get; set; }

        public decimal? discount { get; set; }

        public bool? isLimit { get; set; }

        public bool? isService { get; set; }

        public bool? isDiscount { get; set; }

        public byte? iCmpType { get; set; }

        public bool? isshowprice { get; set; }

        public bool? isshowcode { get; set; }


        public string unitid { get; set; }

        public string unitName { get; set; }

        public string itagperiod { get; set; }
    }
}
