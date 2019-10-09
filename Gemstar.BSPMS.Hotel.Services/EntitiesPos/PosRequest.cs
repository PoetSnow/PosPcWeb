using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosRequest")]
    [LogCName("要求定义")]
    public class PosRequest
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        public string Code { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("名称")]
        public string Cname { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

        [LogIgnore]
        [LogCName("要求操作")]
        public int? ITagOperator { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("前提要求")]
        public string ReQuest { get; set; }

        [LogIgnore]
        [LogCName("联单打印")]
        public int? IsCombine { get; set; }

        [LogIgnore]
        [LogCName("要求属性")]
        public int? IsTagProperty { get; set; }

        [LogIgnore]
        [LogCName("出品状态")]
        public int? IsProduce { get; set; }

        [LogIgnore]
        [LogCName("要求时间")]
        public int? IMinute { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("所属营业点")]
        public string Refeid { get; set; }

        [LogIgnore]
        [LogCName("是否微信显示")]
        public bool? IShowWx { get; set; }

        [LogIgnore]
        [LogCName("排列序号")]
        public int? Seqid { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("背景图片")]
        public string Bmp { get; set; }

        [LogIgnore]
        [LogCName("修改时间")]
        public DateTime? ModifiedDate { get; set; }

        [LogIgnore]
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }



        [LogCName("状态（1：启用，51：禁用）")]
        public byte? IStatus { get; set; }
    }
}
