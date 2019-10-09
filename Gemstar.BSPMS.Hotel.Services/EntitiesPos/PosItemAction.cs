using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosItemAction")]
    [LogCName("消费项目对应作法")]
    public class PosItemAction
    {
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("项目id")]
        public string Itemid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("作法id")]
        public string Actionid { get; set; }

        [LogCName("数量相关")]
        public bool? IsByQuan { get; set; }

        [LogCName("数量相关最低数量")]
        public decimal? LimitQuan { get; set; }

        [LogCName("人数相关")]
        public bool? IsByGuest { get; set; }

        [LogCName("常用作法")]
        public bool? IsCommon { get; set; }

        [LogCName("必选作法")]
        public bool? IsNeed { get; set; }

        [LogCName("作法加价")]
        public decimal? AddPrice { get; set; }

        [LogCName("加价倍数")]
        public decimal? Multiple { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("出品打印机")]
        public string ProdPrinter { get; set; }

        [LogCName("排列序号")]
        public int? SeqID { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("修改时间")]
        public DateTime? Modified { get; set; }

        [LogCName("项目类型")]
        public byte? iType { get; set; }
    }
}