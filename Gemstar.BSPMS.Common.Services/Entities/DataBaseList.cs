using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("dbList")]
    public class DataBaseList
    {
        /// <summary>
        /// 主键值
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// 数名称
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// 服务器IP
        /// </summary>
        [Column(TypeName = "varchar")]
        public string DbServer { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        [Column(TypeName = "varchar")]
        public string DbName { get; set; }

        /// <summary>
        /// 登录账号
        /// </summary>
        [Column(TypeName = "varchar")]
        public string LogId { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [Column(TypeName = "varchar")]
        public string LogPwd { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public Guid VersionId { get; set; }

        /// <summary>
        /// 外网ip
        /// </summary>
        [Column(TypeName = "varchar")]
        public string IntIp { get; set; }
    }
}
