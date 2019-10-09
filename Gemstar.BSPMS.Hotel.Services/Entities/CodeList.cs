using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("CodeList")]
    [LogCName("通用代码")]
    public class CodeList : IBasicDataCopyEntity, IEntityEnable
    {
        [Key]
        [Column(TypeName = "int")]
        [LogCName("主键值")]
        [LogIgnore]
        public int Pk { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店id")]
        [LogIgnore]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("代码类型")]
        public string TypeCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("类型名称")]
        public string TypeName { get; set; }

        
        [Column(TypeName = "varchar")]
        [LogCName("代码id")]
        [LogAnywayWhenEdit]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名称")]
        [BasicDataUpdate(UpdateName = "名称")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名称2")]
        [BasicDataUpdate(UpdateName = "名称2")]
        public string Name2 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名称3")]
        [BasicDataUpdate(UpdateName = "名称3")]
        public string Name3 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名称4")]
        [BasicDataUpdate(UpdateName = "名称4")]
        public string Name4 { get; set; }

        [LogCName("状态")]
        public EntityStatus Status { get; set; }

        [LogCName("顺序")]
        [BasicDataUpdate(UpdateName = "排序号")]
        public int? Seqid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("拼音码")]
        [LogIgnore]
        public string Py { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("数据来源")]
        public string DataSource { get; set; }
        /// <summary>
        /// 数据分发id
        /// </summary>
        [LogCName("数据分发id")]
        public string DataCopyId { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }


        [Column(TypeName = "varchar")]
        [LogCName("单位")]
        public string ZoneCode { get; set; }
    }
}
