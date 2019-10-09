using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("pmsUser")]
    [LogCName("操作员")]
    public class PmsUser
    {
        [Column(TypeName = "char")]
        [LogCName("集团id")]
        [LogIgnore]
        public string Grpid { get; set; }
        [Key]
        [LogIgnore]
        public Guid Id { get; set; }
        [LogCName("登录名")]
        [LogAnywayWhenEdit]
        [Column(TypeName = "varchar")]
        public string Code { get; set; }
        [LogCName("操作员")]
        [Column(TypeName = "varchar")]
        [LogAnywayWhenEdit]
        public string Name { get; set; }
        [Column(TypeName = "varchar")]
        [LogCName("邮箱")]
        public string Email { get; set; }
        [Column(TypeName = "varchar")]
        public string Qq { get; set; }
        [Column(TypeName = "varchar")]
        [LogCName("手机")]
        public string Mobile { get; set; }
        [Column(TypeName = "varchar")]
        [LogIgnore]
        public string Pwd { get; set; }
        [LogCName("状态")]
        public EntityStatus Status { get; set; }
        [LogIgnore]
        public DateTime? LoginDate { get; set; }

        [LogIgnore]
        public byte IsReg { get; set; }
        /// <summary>
        /// 微信标识
        /// </summary>
        [LogCName("微信标识")]
        public string WxOpenId { get; set; }

        [LogCName("备注")]
        [Display(Name = "备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 总裁驾驶舱账号
        /// </summary>
        [LogCName("总裁驾驶舱账号")]
        [Column(TypeName = "varchar")]
        public string AnalysisUserCode { get; set; }

        [LogCName("所属分店")] 
        public string Belonghotel { get; set; }

        [LogCName("卡号")]
        public string CardId { get; set; }

        [LogCName("营业点")]
        public string RefeId { get; set; }

        [LogCName("收银点")]
        public string PosId { get; set; }

        [LogCName("操作员身份")]
        public string OperatorStatus { get; set; }
    }
}
