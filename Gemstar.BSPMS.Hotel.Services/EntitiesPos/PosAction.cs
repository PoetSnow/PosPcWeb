using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosAction")]
    [LogCName("作法基础资料")]
    public class PosAction
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("作法分类id")]
        public string ActionTypeID { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        public string Code { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("中文名")]
        public string Cname { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("出品打印机")]
        public string ProdPrinter { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }
        
        [LogCName("标准加价")]
        public decimal? AddPrice { get; set; }
        
        [LogCName("单价倍数")]
        public decimal? Multiple { get; set; }
        
        [LogCName("输入价格")]
        public bool? IsInputPrice { get; set; }
        
        [LogCName("加价数量相关")]
        public bool? IsByQuan { get; set; }
        
        [LogCName("加价人数相关")]
        public bool? IsByGuest { get; set; }
        
        [LogCName("加价条数相关")]
        public bool? IsByPiece { get; set; }
        
        [LogCName("分单出品")]
        public bool? IsSubProd { get; set; }
        
        [LogCName("排列序号")]
        public int? SeqId { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }
        
        [LogCName("修改时间")]
        public DateTime? ModifiedDate { get; set; }

        [LogCName("状态（1：启用，51：禁用）")]
        public byte? IStatus { get; set; }
    }
}
