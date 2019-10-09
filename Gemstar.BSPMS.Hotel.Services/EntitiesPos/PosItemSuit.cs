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

    [Table("posItemSuit")]
    [LogCName("pos套餐酒席")]
    public class PosItemSuit
    {
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("套餐酒席ID")]
        public string ItemId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("套餐酒席代码")]
        public string ItemCode { get; set; }
        
        [LogCName("级数")]
        public int? IGrade { get; set; }

        [LogCName("自选")]
        public bool IsAuto { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("明细ID")]
        public string ItemId2 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("明细代码")]
        public string ItemCode2 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("明细名称")]
        public string ItemName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单位ID")]
        public string Unitid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单位代码")]
        public string UnitCode { get; set; }
       
        [LogCName("数量")]
        public decimal? Quantity { get; set; }

        [LogCName("单价")]
        public decimal? Price { get; set; }

        [LogCName("分摊金额")]
        public decimal? AddPrice { get; set; }

        [LogCName("金额")]
        public decimal? Amount { get; set; }

        [LogCName("是否分摊金额")]
        public bool IsPrice { get; set; }

        [LogCName("是否参与自动组合套餐")]
        public bool IsBuild { get; set; }

        [LogCName("修改时间")]
        public DateTime? Modifieddate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }
    }
}
