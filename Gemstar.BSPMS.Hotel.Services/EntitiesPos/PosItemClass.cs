using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosItemClass")]
    [LogCName("消费项目大类")]
    public class PosItemClass
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        public string Code { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("名称")]
        public string Cname { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }
        
        [LogCName("是否分类")]
        public bool? IsSubClass { get; set; }
        
        [LogCName("是否IPAD显示")]
        public bool? IsIpadShow { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("所属营业点")]
        public string Refeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("背景图片")]
        public string Bmp { get; set; }
        
        [LogCName("排列序号")]
        public int? Seqid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }
        
        [LogCName("修改时间")]
        public DateTime? ModifiedDate { get; set; }


        [LogCName("状态（1：启用，51：禁用）")]
        public byte? IStatus { get; set; }
    }
}
