using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("WeixinQrcodes")]
    [LogCName("微信带参数二维码")]
    public class WeixinQrcodes
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [LogCName("场景值，主键")]
        public int SceneId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店id")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("值类型")]
        public string IdType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("值")]
        public string IdValue { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("用于换取二维码显示")]
        public string Ticket { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("二维码内容")]
        public string QrcodeContent { get; set; }

        [LogCName("创建时间")]
        public DateTime Cdate { get; set; }

        [LogCName("过期时间")]
        public DateTime EndDate { get; set; }

    }
}
