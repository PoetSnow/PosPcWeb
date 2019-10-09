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
    [Table("PosOnSale")]
    [LogCName("特价菜(放在营业政策时，与开台项目、服务费政策在一起，另外要考虑做一个批量的功能。)")]
    public class PosOnSale
    {

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("客人类型id")]
        public string CustomerTypeid { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("营业点id")]
        public string Refeid { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("餐台类型id")]
        public string TabTypeid { get; set; }


        [LogCName("日期类型")]
        public byte? ITagperiod { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("开始时间")]
        public string StartTime { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("结束时间")]
        public string EndTime { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("项目id")]
        public string Itemid { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("单位")]
        public string Unitid { get; set; }

        [LogCName("价格")]
        public decimal? Price { get; set; }

        [LogIgnore]
        [LogCName("折扣率")]
        public decimal? Discount { get; set; }

        [LogCName("是否计最低消费")]
        public bool? IsLimit { get; set; }

        [LogCName("是否计服务费")]
        public bool? IsService { get; set; }

        [LogCName("是否打折")]
        public bool? IsDiscount { get; set; }

        [LogCName("计算类型")]
        public byte? ICmpType { get; set; }

        [LogCName("是否启用")]
        public bool? IsUsed { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("修改时间")]
        public DateTime? ModifiedDate { get; set; }


        [LogCName("大类ID")]
        public string ItemClassid { get; set; }

        [LogCName("类型")]
        public byte?  iType { get; set; }


        [LogCName("开始日期")]
        public DateTime? sDate { get; set; }

        [LogCName("结束日期")]
        public DateTime? eDate { get; set; }

    }
}
