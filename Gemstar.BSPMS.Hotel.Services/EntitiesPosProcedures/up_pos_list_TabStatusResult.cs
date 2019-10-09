using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_list_TabStatusResult
    {
        public string Hid { get; set; }
        
        public string Tabid { get; set; }

        public string TabNo { get; set; }
        
        public string TabName { get; set; }
        
        public string Billid { get; set; }

        public int? IGuest { get; set; }
        
        public string Refeid { get; set; }
        
        public string RefeCode { get; set; }
        
        public string RefeName { get; set; }
        
        public string Tabtypeid { get; set; }
        
        public string TabTypeCode { get; set; }
        
        public string TabTypeName { get; set; }
        
        public byte? TabStatus { get; set; }
        
        public string OpTabid { get; set; }
        
        public string GuestName { get; set; }
        
        public int? OpenGuest { get; set; }
        
        public DateTime? OpenRecord { get; set; }
        
        public string ResTabid { get; set; }
        
        public byte? TabResStatus { get; set; }
        
        public DateTime? ArrDate { get; set; }
        
        public string Module { get; set; }
        
        public int? Seqid { get; set; }

        public bool? IsShowTableproperty { get; set; }

        public string FloorShowData { get; set; }

        /// <summary>
        /// 锁台餐台ID
        /// </summary>
        public string LockTabId { get; set; }
    }
}
