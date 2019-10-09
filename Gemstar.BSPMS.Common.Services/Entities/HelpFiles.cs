using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("HelpFiles")]
    public class HelpFiles
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar")]
        public string Title { get; set; }

        [Column(TypeName = "varchar")]
        public string AddUser { get; set; }

        public DateTime AddDate { get; set; }

        [Column(TypeName = "varchar")]
        public string UpdateUser { get; set; }

        public DateTime UpdateDate { get; set; }

        public bool CheckStatus { get; set; }

        [Column(TypeName = "varchar")]
        public string CheckUser { get; set; }

        public DateTime? CheckDate { get; set; }

        public int ReadNumber { get; set; }

        [Column(TypeName = "varchar")]
        public string MenuId { get; set; }

        [Column(TypeName = "varchar")]
        public string MenuName { get; set; }

        [Column(TypeName = "varchar")]
        public string Content { get; set; }

    }
}
