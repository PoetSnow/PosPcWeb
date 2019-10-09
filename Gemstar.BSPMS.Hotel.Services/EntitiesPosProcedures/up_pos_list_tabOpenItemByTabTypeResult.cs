using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    ///  存储过程 up_pos_list_tabOpenItemByTabType 执行后的结果集对象
    /// </summary>
    public class up_pos_list_tabOpenItemByTabTypeResult
    {
        public Guid Id { get; set; }

        public string Hid { get; set; }

        public string CustomerTypeid { get; set; }

        public string Refeid { get; set; }

        public string TabTypeid { get; set; }

        public byte? ITagperiod { get; set; }
        
        public string StartTime { get; set; }
        
        public string EndTime { get; set; }
        
        public string Itemid { get; set; }
        
        public string Unitid { get; set; }
        
        public decimal Quantity { get; set; }
        
        public decimal? Price { get; set; }
        
        public byte QuanMode { get; set; }
        
        public byte? IsCharge { get; set; }
        
        public bool? IsProduce { get; set; }
        
        public bool? IsCancel { get; set; }
        
        public string Module { get; set; }
       
        public string Remark { get; set; }
        
        public DateTime? ModifiedDate { get; set; }

        public string itemCode { get; set; }

        public string itemName { get; set; }

        public string unitCode { get; set; }

        public string unitName { get; set; }
    }
}
