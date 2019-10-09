using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities {

    [Table("HotelPos")]
    [LogCName("酒店智能POS设备管理")]
    public class HotelPos {
        [LogIgnore]
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("Pos设备编号")]
        public string PosId { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒店id")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店线下序列号")]
        public string SeriesNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("模块代码")]
        public string ProductCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("模块名称")]
        public string ProductName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("接口地址")]
        public string InterfaceUrl { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }
        
        [LogCName("创建日期")]
        public DateTime CDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("创建人")]
        public string CUser { get; set; }

        [LogCName("修改日期")]
        public DateTime? MDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("修改人")]
        public string MUser { get; set; }

    }
}
