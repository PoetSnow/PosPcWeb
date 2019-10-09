using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosMscroll")]
    [LogCName("手机界面滚动菜式")]
    public class PosMScroll
    {
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒店hid")]
        public string Hid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("项目id")]
        public string Itemid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("项目代码")]
        public string ItemCode { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("项目名称")]
        public string ItemName { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("单位id")]
        public string Unitid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("单位名称")]
        public string UnitName { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("图片文件")]
        public string FileName { get; set; }
        
        [LogCName("排列序号")]
        public int? OrderBy { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("创建人")]
        public string Creator { get; set; }
        
        [LogCName("创建时间")]
        public DateTime? Createdate { get; set; }
    }
}