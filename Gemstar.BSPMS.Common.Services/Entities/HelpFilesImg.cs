using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{

    [Table("HelpFilesImg")]
    public class HelpFilesImg
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar")]
        public string ImgAddress { get; set; }

        [Column(TypeName = "varchar")]
        public string Title { get; set; }

        public int? HelpId { get; set; }

    }
}
