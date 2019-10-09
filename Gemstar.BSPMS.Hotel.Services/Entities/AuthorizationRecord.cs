using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    /// <summary>
    /// 授权记录表
    /// </summary>
    [Table("AuthorizationRecord")]
    public class AuthorizationRecord
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public Guid Id { get; set; }
        /// <summary>
        /// 酒店ID
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }
        /// <summary>
        /// 授权类型（1：客情调价授权；2：客账减免授权；3：客账冲销授权；4：房租加收修改授权）
        /// </summary>
        public byte Type { get; set; }
        /// <summary>
        /// 授权模式（1：登录授权；2：微信授权）
        /// </summary>
        public byte Mode { get; set; }
        /// <summary>
        /// 授权内容 详细内容
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Content { get; set; }
        /// <summary>
        /// 原因 请求授权原因
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Reason { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Describle { get; set; }
        /// <summary>
        /// 外部关联号（具体业务表主键ID，例如：resDetail.regid）
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Keys { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public Guid CreateUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 撤销时间
        /// </summary>
        public DateTime? RevokeDate { get; set; }

        /// <summary>
        /// 授权人
        /// </summary>
        public Guid AuthUserId { get; set; }
        /// <summary>
        /// 授权状态（0：默认值；1：成功；2：失败）
        /// </summary>
        public byte AuthStatus { get; set; }
        /// <summary>
        /// 授权时间
        /// </summary>
        public DateTime? AuthDate { get; set; }
    }
}
