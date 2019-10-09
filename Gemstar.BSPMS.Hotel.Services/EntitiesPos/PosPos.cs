using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosPos")]
    [LogCName("pos收银点")]
    public class PosPos
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("收银点id")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店id")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        public string Code { get; set; }        

        [Column(TypeName = "varchar")]
        [LogCName("中文名称")]
        public string Name { get; set; }

        [LogCName("排序号")]
        public int? Seqid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("英文名称")]
        public string Ename { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("当前市别")]
        public string ShiftId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("模块代码")]
        public string Module { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("当前营业日")]
        public DateTime? Business { get; set; }

        [LogCName("内部编码")]
        public string CodeIn { get; set; }

        [LogCName("云Pos属性")]
        public string PosMode { get; set; }

        [LogCName("结转设置")]
        public PosBusinessEnd? IsBusinessend { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("最早结转时间")]
        public string BusinessTime { get; set; }

        [LogCName("状态（1：启用，51：禁用）")]
        public byte? IStatus { get; set; }

        [LogCName("是否启用扫码点餐（1：启用，0：禁用）")]
        public bool? IsBrushOrder { get; set; }
    }
}
