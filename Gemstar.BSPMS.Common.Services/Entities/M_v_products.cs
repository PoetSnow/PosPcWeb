using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities {
    [Table("M_v_products")]
    public class M_v_products {
        [Key]
        [Column(TypeName = "varchar")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        public string Domain { get; set; }

        [Column(TypeName = "varchar")]
        public string Domain2 { get; set; }

        [Column(TypeName = "varchar")]
        public string SysName { get; set; }

        [Column(TypeName = "varchar")]
        public string SysShortName { get; set; }

        /// <summary>
        /// 是否在线系统，1为在线系统，0为线下系统
        /// </summary>
        public int? IsOnline { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int? SeqId { get; set; }

    }
}
