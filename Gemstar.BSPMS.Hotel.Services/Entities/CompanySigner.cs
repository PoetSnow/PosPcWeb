using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("CompanySigner")]
    [LogCName("签单人")]
    public class CompanySigner
    {
        [Column(TypeName = "varchar")]
        [LogIgnore]
        public string Hid { get; set; }

        [Key]
        [LogCName("签单人id")]
        public Guid Id { get; set; }

        [LogRefrenceName(Sql = "SELECT code+'，合约单位名称：'+name FROM company WHERE id = {0}")]
        [LogCName("合约单位代码")]
        [LogAnywayWhenEdit]
        public Guid CompanyId { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("姓名")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("称谓")]
        public string Position { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("手机")]
        public string Mobile { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("角色")]
        public string Rolename { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("职位")]
        public string Job { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("电话")]
        public string Telephone { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("微信号")]
        public string Weixin { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("邮箱")]
        public string Email { get; set; }
        
        [LogCName("类型")]
        public CompanySignType Signtype { get; set; }

        [LogCName("创建时间")]
        public DateTime? Cdate { get; set; }

        [LogIgnore]
        [LogCName("是否导入的")]        
        public byte? IsImport { get; set; }
    }
}