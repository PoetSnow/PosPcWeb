using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("AdSet")]
    public class AdSet
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "varchar")]
        public string Position { get; set; }

        [Column(TypeName = "varchar")]
        public string PicLink { get; set; }

        [Column(TypeName = "varchar")]
        public string Link { get; set; }

        public int Seqid { get; set; }

        [Column(TypeName = "varchar")]
        public string Domain { get; set; }

        [Column(TypeName = "varchar")]
        public string Bucket { get; set; }

        [Column(TypeName = "varchar")]
        public string Access_key { get; set; }

        [Column(TypeName = "varchar")]
        public string Secret_key { get; set; }
    }
}
