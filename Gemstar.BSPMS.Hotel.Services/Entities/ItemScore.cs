using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ItemScore")]
    [LogCName("积分可兑换的项目")]
    public class ItemScore
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("代码")] 
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名称")]
        [LogAnywayWhenEdit]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("说明")]
        public string Remark { get; set; }

        [LogCName("排序号")]
        public int? Seqid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("图片地址")]
        public string PicAdd { get; set; }

        [LogCName("状态")]
        public EntityStatus Status { get; set; }

        [LogCName("适用分店")] 
        public string BelongHotel { get; set; }
    }
}