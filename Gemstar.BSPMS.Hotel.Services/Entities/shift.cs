using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("Shift")]
    [LogCName("班次")]
    public class Shift : IBasicDataCopyEntity, IEntityEnable
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        [LogIgnore]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("班次id")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("班次代码")]
        [BasicDataUpdate(UpdateName = "班次代码")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("班次名")]
        [LogAnywayWhenEdit]
        [BasicDataUpdate(UpdateName = "班次名")]
        public string ShiftName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("开始时间")]
        [BasicDataUpdate(UpdateName = "开始时间")]
        public string BeginTime { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("结束时间")]
        [BasicDataUpdate(UpdateName = "结束时间")]
        public string EndTime { get; set; }

        [LogCName("班次登录状态")]
        public ShiftLoginStatus LoginStatus { get; set; }

        [LogCName("状态")]
        public EntityStatus Status { get; set; }

        [LogCName("顺序")]
        [BasicDataUpdate(UpdateName = "排序号")]
        public int? Seqid { get; set; } 

        [Column(TypeName = "varchar")]
        [LogCName("数据来源")]
        public string DataSource { get; set; }
        /// <summary>
        /// 数据分发id
        /// </summary>
        [LogCName("数据分发id")]
        public string DataCopyId { get; set; }
    }
}
