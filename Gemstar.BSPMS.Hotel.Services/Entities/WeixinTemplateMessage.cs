using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    /// <summary>
    /// 微信模板消息表（对应中央数据库库pmsMaster.weixinTemplateMessage；[SendStatus]模板消息请求结果，指模板消息发送到微信服务器；[SendFinishStatus]模板消息发送结果，指模板消息发送到微信用户；）
    /// </summary>
    [Table("WeixinTemplateMessage")]
    public class WeixinTemplateMessage
    {
        /// <summary>
        /// 主键ID（等于pmsMaster.weixinTemplateMessage.id）
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 酒店ID
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }
        /// <summary>
        /// 外键ID（等于具体业务表主键ID，例如：authorizationRecord.id）
        /// </summary>
        public Guid Keyid { get; set; }
        /// <summary>
        /// 发送结果状态
        /// </summary>
        [Column(TypeName = "varchar")]
        public string SendStatus { get; set; }
        /// <summary>
        /// 发送结果内容
        /// </summary>
        [Column(TypeName = "varchar")]
        public string SendMsg { get; set; }
        /// <summary>
        /// 发送结果时间
        /// </summary>
        public DateTime? SendDate { get; set; }
        /// <summary>
        /// 是否送达状态
        /// </summary>
        [Column(TypeName = "varchar")]
        public string SendFinishStatus { get; set; }
        /// <summary>
        /// 是否送达内容
        /// </summary>
        [Column(TypeName = "varchar")]
        public string SendFinishMsg { get; set; }
        /// <summary>
        /// 是否送达时间
        /// </summary>
        public DateTime? SendFinishDate { get; set; }
    }
}
