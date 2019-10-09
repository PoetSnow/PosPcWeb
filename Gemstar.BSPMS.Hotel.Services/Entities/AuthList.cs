using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("AuthList")]
    [LogCName("权限列表,每个权限对应一个功能页面")]
    public class AuthList
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("权限id")]
        public string AuthCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("父权限id")]
        public string ParentCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("权限名称")]
        [LogAnywayWhenEdit]
        public string AuthName { get; set; }

        [LogCName("排序号")]
        public int? Seqid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("页面Area")]
        public string Area { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("页面Controller")]
        public string Controller { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("页面Action")]
        public string Action { get; set; }

        [LogCName("集团可用")]
        public byte IsGroup { get; set; }

        [LogCName("集团分店可用")]
        public byte IsGroupHotel { get; set; }

        [LogCName("单店可用")]
        public byte IsHotel { get; set; }

        /// <summary>
        /// 产品掩码
        /// </summary>
        [LogCName("产品掩码")]
        public string Mask { get; set; }
    }
}
