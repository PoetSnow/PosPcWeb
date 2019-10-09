using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("Res")]
    [LogCName("订单主表")]
    public class Res
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("订单id")]
        public string Resid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("订单名/团体名")]
        [LogAnywayWhenEdit]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("预订号")]
        public string Resno { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("外部预订号")]
        public string ResNoExt { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("订房人")]
        public string ResCustName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("订房人手机")]
        public string ResCustMobile { get; set; }

        [LogCName("预订时间")]
        public DateTime? ResTime { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("预订操作员")]
        public string ResUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("市场分类")]
        public string Marketid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客人来源")]
        public string Sourceid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("价格体系")]
        public string RateCode { get; set; }

        [LogCName("合约单位")]
        public Guid? Cttid { get; set; }

        [LogCName("团体散客标志")]
        public byte? IsGroup { get; set; }
        
        [LogCName("客情保密")]
        public bool IsCustemSecret { get; set; }
        
        [LogCName("隐藏房价")]
        public bool IsHidePrice { get; set; }

        [LogCName("订单日期")]
        public DateTime? CDate { get; set; }

        [LogCName("订单来源")]
        public byte? OrderFrom { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("外部预订人")]
        public string ResUserExt { get; set; }

        [Column(TypeName = "varchar")]
        [LogIgnore]
        [LogCName("合约单位业务员")]
        public string CttSales { get; set; }


    }
}
