using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosSellout")]
    [LogCName("沽清表")]
    /// <summary>
    /// 沽清表
    /// </summary>
    public class PosSellout
    {
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        
        [LogCName("酒店代码")]
        public string Hid { get; set; }
        
        [LogCName("营业点")]
        public string RefeId { get; set; }

        [LogCName("营业点")]
        public string ItemId { get; set; }

        [LogCName("营业点")]
        public string ItemCode { get; set; }

        [LogCName("营业点")]
        public string ItemName { get; set; }

        [LogCName("营业点")]
        public string UnitId { get; set; }

        [LogCName("营业点")]
        public byte SellStatus { get; set; }

        [LogCName("营业点")]
        public string Module { get; set; }

        [LogCName("营业点")]
        public string Remark { get; set; }

        [LogCName("营业点")]
        public string TransUser { get; set; }

        [LogCName("营业点")]
        public DateTime? CreateDate { get; set; }

        [LogCName("营业点")]
        public string ModiUser { get; set; }

        [LogCName("营业点")]
        public DateTime? ModiDate { get; set; }
    }
}
