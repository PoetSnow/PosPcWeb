using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.PmsParaManage
{
    public class PmsParaAddViewModel
    {
        [Required(ErrorMessage = "请输入参数代码")]
        [Display(Name = "参数代码")]  
        [Column(TypeName = "varchar")]
        public string Code { get; set; }

        [Required(ErrorMessage = "请输入参数名称")]
        [Display(Name = "参数名称")]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }

         
        [Display(Name = "参数的说明")]
        [Column(TypeName = "varchar")]
        public string Remark { get; set; }

        [Required(ErrorMessage = "请输入参数顺序")]
        [Display(Name = "顺序")]
        public int Seqid { get; set; }

        [Required(ErrorMessage = "请输入参数值")]
        [Display(Name = "参数值")]
        [Column(TypeName = "varchar")]
        public string Value { get; set; }

        [Required(ErrorMessage = "请输入默认值")]
        [Display(Name = "默认值")]
        [Column(TypeName = "varchar")]
        public string DefaultValue { get; set; }

        [Required(ErrorMessage = "请选择是否可见")]
        [Display(Name = "是否可见")]
        public byte IsVisible { get; set; }
    }
}