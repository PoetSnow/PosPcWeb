using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ResLog")]
    [LogCName("订单变更表")]
    public class ResLog
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        [LogCName("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// 酒店代码
        /// </summary>
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [LogCName("操作时间")]
        public DateTime CDate { get; set; }

        /// <summary>
        /// 操作员
        /// </summary>
        [Column(TypeName = "varchar")]
        [LogCName("操作员")]
        public string CUser { get; set; }

        /// <summary>
        /// 操作ip
        /// </summary>
        [Column(TypeName = "varchar")]
        [LogCName("操作ip")]
        public string Ip { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [Column(TypeName = "varchar")]
        [LogCName("账号")]
        public string Regid { get; set; }

        /// <summary>
        /// 操作类型 0:换房  1:调价，2迟付
        /// </summary>
        [LogCName("操作类型")]
        public byte XType { get; set; }

        /// <summary>
        /// 更换前
        /// </summary>
        [Column(TypeName = "varchar")]
        [LogCName("更换前")]
        public string Value1 { get; set; }

        /// <summary>
        /// 更换后
        /// </summary>
        [Column(TypeName = "varchar")]
        [LogCName("更换后")]
        public string Value2 { get; set; }

        /// <summary>
        /// 相关属性1
        /// </summary>
        [Column(TypeName = "varchar")]
        [LogCName("相关属性1")]
        public string Other1 { get; set; }

        /// <summary>
        /// 相关属性2
        /// </summary>
        [Column(TypeName = "varchar")]
        [LogCName("相关属性2")]
        public string Other2 { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Column(TypeName = "varchar")]
        [LogCName("描述")]
        public string Describle { get; set; }

    }
}
