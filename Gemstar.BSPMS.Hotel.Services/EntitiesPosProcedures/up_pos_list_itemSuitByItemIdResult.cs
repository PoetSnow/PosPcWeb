using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_list_itemSuitByItemIdResult
    {

        public Guid Id { get; set; }


        public string Hid { get; set; }



        public string ItemId { get; set; }


        public string ItemCode { get; set; }


        public int? IGrade { get; set; }

        public bool IsAuto { get; set; }


        public string ItemId2 { get; set; }


        public string ItemCode2 { get; set; }


        public string ItemName { get; set; }


        public string Unitid { get; set; }


        public string UnitCode { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? Price { get; set; }

        public decimal? AddPrice { get; set; }


        public decimal? Amount { get; set; }


        public bool IsPrice { get; set; }


        public bool IsBuild { get; set; }


        public DateTime? Modifieddate { get; set; }


        public string Remark { get; set; }

        public string UnitName { get; set; }
    }
}
