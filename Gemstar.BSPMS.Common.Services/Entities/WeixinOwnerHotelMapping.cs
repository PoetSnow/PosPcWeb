using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("WeixinOwnerHotelMapping")]
    [LogCName("业主微信映射关系")]
    public class WeixinOwnerHotelMapping
    {
        [LogIgnore]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [LogCName("映射id")]
        public int MappingId { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒店id")]
        public string Hid { get; set; }
        
        [LogCName("业主id")]
        public Guid ProfileId { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("微信openid")]
        public string OwnerWxOpenId { get; set; }
        
        [LogCName("创建时间")]
        public DateTime Cdate { get; set; }

    }
}