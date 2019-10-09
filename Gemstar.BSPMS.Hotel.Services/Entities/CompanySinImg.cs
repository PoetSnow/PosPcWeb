using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("CompanySinImg")]
    public class CompanySinImg
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar")]
        public string Hid { get; set; }

        public Guid Companyid { get; set; }

        [Column(TypeName = "varchar")]
        public string ImgAddress { get; set; }

        public string Title { get; set; }

    }
}
