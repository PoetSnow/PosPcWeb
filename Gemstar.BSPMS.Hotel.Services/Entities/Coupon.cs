using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("Coupon")]
    public class Coupon
    {
        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        public string ItemTypeid { get; set; }

        [Column(TypeName = "varchar")]
        public string ItemTypeName { get; set; }

        public decimal? CouponMoney { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Column(TypeName = "varchar")]
        public string RoomTypeids { get; set; }

        public string Remark { get; set; }

        public EntityStatus? Status { get; set; }

        public int? Seqid { get; set; }

        public int? ValidDate { get; set; }

        public int? IssueCount { get; set; }
    }
}
