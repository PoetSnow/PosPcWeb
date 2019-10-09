using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities {
    [Table("GridColumnsSettings")]
    [LogCName("用户自定义列表显示列数据")]
    public class GridColumnsSettings {
        [Key]
        [Column(TypeName = "varchar",Order = 0)]
        [LogCName("酒店id")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar",Order =1)]
        [LogCName("列表区域")]
        public string Area { get; set; }

        [Key]
        [Column(TypeName = "varchar",Order = 2)]
        [LogCName("列表控制器")]
        public string Controller { get; set; }

        [Key]
        [Column(TypeName = "varchar",Order = 3)]
        [LogCName("列表页面")]
        public string Action { get; set; }

        [LogCName("列表中的列设置")]
        public string ColumnSettings { get; set; }

    }
}
