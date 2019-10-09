using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ResFolioLog")]
    public class ResFolioLog
    {
        [LogIgnore]
        [Key]
        public Guid Id { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [LogIgnore]
        public DateTime CDate { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string CUser { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Ip { get; set; }

        [LogIgnore]
        public Guid Transid { get; set; }

        /// <summary>
        /// 0转账，1账务作废与恢复，2水电燃气度数
        /// </summary>
        [LogIgnore]
        public byte XType { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Value1 { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Value2 { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Other1 { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Other2 { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Describle { get; set; }

    }
}
