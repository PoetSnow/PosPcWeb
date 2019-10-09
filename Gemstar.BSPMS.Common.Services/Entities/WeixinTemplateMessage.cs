using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    /// <summary>
    /// 微信模板消息表（对应业务库pms.weixinTemplateMessage；[Id + Hid + Type]确定业务库唯一记录；[Openid + Msgid]确定唯一记录；）
    /// </summary>
    [Table("WeixinTemplateMessage")]
    public class WeixinTemplateMessage
    {
        /// <summary>
        /// 主键ID（等于pms.weixinTemplateMessage.id）
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 酒店ID
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }
        /// <summary>
        /// 模板消息类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权；4：营业简报推送）
        /// </summary>
        public byte Type { get; set; }
        /// <summary>
        /// 微信用户标识Openid
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Openid { get; set; }
        /// <summary>
        /// 微信模板消息ID
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Msgid { get; set; }
    }
}
