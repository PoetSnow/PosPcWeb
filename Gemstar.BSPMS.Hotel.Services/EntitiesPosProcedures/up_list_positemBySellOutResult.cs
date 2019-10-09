using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_list_positemBySellOutResult
    {
       
        public Guid Id { get; set; }


      
        public string Hid { get; set; }

       
        public string RefeId { get; set; }

      
        public string ItemId { get; set; }

        
        public string ItemCode { get; set; }

        
        public string ItemName { get; set; }

      
        public string UnitId { get; set; }

      
        public byte SellStatus { get; set; }

      
        public string Module { get; set; }

     
        public string Remark { get; set; }

        
        public string TransUser { get; set; }

     
        public DateTime? CreateDate { get; set; }

       
        public string ModiUser { get; set; }

       
        public DateTime? ModiDate { get; set; }

        public  string unitName { get; set; }

        public string StatusText { get; set; }
    }
}
