using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("CompanyLog")]
    [LogCName("合约单位变更记录")]
    public class CompanyLog
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [LogCName("客历号")]
        [LogIgnore]
        public Guid Companyid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("时间")]
        [LogIgnore]
        public string Cdate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("变更的类型")]
        public string Type { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("更换前")]
        public string Old { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("更换后")]
        public string New { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("操作员")]
        [LogIgnore]
        public string InputUser { get; set; }

    }
}
