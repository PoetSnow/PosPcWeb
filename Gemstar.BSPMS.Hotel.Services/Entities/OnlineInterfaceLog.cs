using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    /// <summary>
    /// 线上接口日志
    /// </summary>
    [Table("OnlineInterfaceLog")]
    public class OnlineInterfaceLog
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// 接口类型
        /// </summary>
        [Column(TypeName = "varchar")]
        public string TypeCode { get; set; }
        /// <summary>
        /// 接口代码
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Code { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CDate { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Url { get; set; }
        /// <summary>
        /// 发送内容
        /// </summary>
        [Column(TypeName = "varchar")]
        public string SendContent { get; set; }
        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime? SendDate { get; set; }
        /// <summary>
        /// 回复内容
        /// </summary>
        [Column(TypeName = "varchar")]
        public string ReceiveContent { get; set; }
        /// <summary>
        /// 回复时间
        /// </summary>
        public DateTime? ReceiveDate { get; set; }
        /// <summary>
        /// 酒店ID
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Regid { get; set; }

    }
}
