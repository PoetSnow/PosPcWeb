using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("ServicesOperator")]
    [LogCName("售后工程师")]
    public class ServicesOperator
    {

        [Key]
        [LogIgnore]
        public Guid ID { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("姓名")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("密码")]
        public string Pwd { get; set; }

        [LogCName("状态")]
        public EntityStatus Status { get; set; }

        [LogCName("手机号")]
        public string MobilePhone { get; set; }

        [LogCName("是否超级用户")]
        public bool? IsSuperAdmin { get; set; }

        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("微信Openid")]
        public string LoginOpenid { get; set; }
    }
}