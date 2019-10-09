using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosMorderList")]
    [LogCName("手机界面点菜记录")]
    public class PosMorderList
    {
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店hid")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("单号")]
        public string Billid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("点菜类型")]
        public string OrderType { get; set; }

        [LogCName("点菜参数")]
        public string OrderPara { get; set; }

        [LogCName("点菜状态")]
        public byte? IStatus { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("微信id")]
        public string Wxid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("创建人")]
        public string Creator { get; set; }

        [LogCName("创建时间")]
        public DateTime? Createdate { get; set; }

        [LogCName("是否服务员点餐")]
        public bool? IWaiter { get; set; }
    }
}