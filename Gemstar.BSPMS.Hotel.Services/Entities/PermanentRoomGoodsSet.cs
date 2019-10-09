using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("PermanentRoomGoodsSet")]
    public class PermanentRoomGoodsSet
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 酒店ID
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        /// <summary>
        /// 长包房设置表 主键ID
        /// </summary>
        [Column(TypeName = "varchar")]
        public string PermanentRoomSetId { get; set; }

        /// <summary>
        /// 物品ID
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Itemid { get; set; }

        /// <summary>
        /// 赔偿金额
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 借用状态（1借用，2归还）
        /// </summary>
        [Column(TypeName = "varchar")]
        public string BorrowType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }
    }
}
