using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("PermanentRoomFixedCostSet")]
    public class PermanentRoomFixedCostSet
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        public Guid PermanentRoomSetId { get; set; }

        [Column(TypeName = "varchar")]
        public string Itemid { get; set; }

        public decimal Amount { get; set; }

        /// <summary>
        /// 类型（1加收，2包费）
        /// </summary>
        public byte? Type { get; set; }

    }
}