using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Common.Services.Entities
{
    [Table("CityMaster")]
    public class CityMaster
    {
        [Key]
        [Column(TypeName = "char")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Column(TypeName = "char")]
        public string ProvinceCode { get; set; }

        [Column(TypeName = "varchar")]
        public string ProvinceName { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        [Column(TypeName = "varchar")]
        public string Zipcode { get; set; }

    }
}
