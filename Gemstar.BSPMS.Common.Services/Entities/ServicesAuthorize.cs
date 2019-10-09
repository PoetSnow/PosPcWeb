using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    /// <summary>
    /// 酒店授权售后服务记录表
    /// </summary>
    [Table("ServicesAuthorize")]
    [LogCName("授权记录")]
    public class ServicesAuthorize
    {
        [Key]
        [LogIgnore]
        public Guid ID { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("集团id")]
        public string GrpId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店id")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("授权码")]
        public string AuthCode { get; set; }

        [LogCName("生效时间")]
        public DateTime BeginDate { get; set; }

        [LogCName("失效时间")]
        public DateTime EndDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("授权人代码")]
        public string UserCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("授权人姓名")]
        public string UserName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("服务工程师代码")]
        public string LastServiceCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("服务工程师姓名")]
        public string LastServiceName { get; set; }

        [LogCName("服务时间")]
        public DateTime? LastServiceDate { get; set; }

        [LogCName("服务次数")]
        public int? ServiceTimes { get; set; }

    }
}
