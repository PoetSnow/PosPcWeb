using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    /// <summary>
    /// 微信扫码登录表
    /// </summary>
    [Table("WeixinQrcodeLogin")]
    public class WeixinQrcodeLogin
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// 键ID
        /// </summary>
        public Guid Keyid { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireDate { get; set; }
        /// <summary>
        /// 状态 Gemstar.BSPMS.Common.Services.Enums.WeixinQrcodeLoginStatus
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 登录微信Openid
        /// </summary>
        [Column(TypeName = "varchar")]
        public string LoginOpenid { get; set; }
        /// <summary>
        /// 登录酒店ID
        /// </summary>
        [Column(TypeName = "varchar")]
        public string LoginHid { get; set; }
        /// <summary>
        /// 登录用户ID
        /// </summary>
        public Guid? LoginUserid { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime? LoginDate { get; set; }

        /// <summary>
        /// 登录类型 null或者1：操作员扫码登录，2：售后工程师扫码登录 3:运营后台扫码登录
        /// </summary>
        public byte? LoginType { get; set; }
    }
}
