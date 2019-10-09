using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("BasicDataResortControl")]
    [LogCName("集团分发型基础资料的分发控制")]
    public class BasicDataResortControl
    {
        [LogIgnore]
        [Key]
        [LogCName("主键值")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("集团id")]
        public string Grpid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("基础资料代码")]
        public string BasicDataCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("基础资料名称")]
        public string BasicDataName { get; set; }

        [LogCName("分店可以增加")]
        public bool ResortCanAdd { get; set; }
        
        [LogCName("分店可以修改")]
        public bool ResortCanUpdate { get; set; }
        
        [LogCName("分店可以禁用")]
        public bool ResortCanDisable { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("数据分发类型")]
        public string DataCopyType { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("选中分店")]
        public string SelectedHids { get; set; }

    }
}