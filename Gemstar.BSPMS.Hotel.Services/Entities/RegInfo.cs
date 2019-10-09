using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("RegInfo")]
    [LogCName("登记客人资料表")]
    public class RegInfo
    {
        [Column(TypeName = "varchar")]
        [LogCName("地址")]
        public string Address { get; set; }

        [LogCName("生日")]
        public DateTime? Birthday { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("车牌号")]
        public string CarNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("证件号")]
        public string Cerid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("证件类型")]
        public string CerType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("籍贯")]
        public string City { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("邮箱")]
        public string Email { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("性别")]
        public string Gender { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客人英文名")]
        public string GuestEName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客人名")]
        [LogAnywayWhenEdit]
        public string GuestName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [LogCName("流水号")]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("喜好")]
        public string Interest { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("是否主客")]
        public string IsMast { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("手机号")]
        public string Mobile { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("qq号")]
        public string Qq { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("登记单id")]
        public string Regid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("国籍")]
        public string Nation { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("身份证照片")]
        public string PhotoUrl { get; set; }
    }
}