using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("BookingNotes")]
    [LogCName("预订须知")]
    public class BookingNotes: IBasicDataCopyEntity
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        [LogIgnore]
        public string Hid { get; set; }

        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogAnywayWhenEdit]
        [LogCName("名称")]
        [BasicDataUpdate(UpdateName ="名称")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("预订须知内容")]
        [BasicDataUpdate(UpdateName = "内容")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("数据来源")]
        public string DataSource { get; set; }
        /// <summary>
        /// 数据分发id
        /// </summary>
        [LogCName("数据分发id")]
        public Guid? DataCopyId { get; set; }
    }
}
