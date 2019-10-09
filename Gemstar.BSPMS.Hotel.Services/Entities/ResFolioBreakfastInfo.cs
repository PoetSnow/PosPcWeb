using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ResFolioBreakfastInfo")]
    public class ResFolioBreakfastInfo
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        public string Regid { get; set; }

        [Column(TypeName = "varchar")]
        public string RoomNo { get; set; }

        [Column(TypeName = "varchar")]
        public string CardNo { get; set; }

        [Column(TypeName = "varchar")]
        public string GuestName { get; set; }

        public int BbfCount { get; set; }

        public int BbfIndex { get; set; }

        public Guid? Transid { get; set; }

        public byte Status { get; set; }

        [Column(TypeName = "varchar")]
        public string Remark { get; set; }

        public DateTime CDate { get; set; }

    }
}
