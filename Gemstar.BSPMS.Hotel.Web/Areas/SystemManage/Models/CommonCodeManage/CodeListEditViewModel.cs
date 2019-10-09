using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Web.Models;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.CommonCodeManage
{
    public class CodeListEditViewModel: BaseEditViewModel
    {
        [Key]
        [Display(Name = "Pk")] 
        public int Pk { get; set; }

        [Display(Name = "酒店编号")]
        public string Hid { get; set; }

        [Display(Name = "Id")] 
        public string Id { get; set; }

        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入代码")]
        public string Code { get; set; }

        [Display(Name = "名称")]
        [Required(ErrorMessage = "请输入名称")]
        public string Name { get; set; }

        [Display(Name = "名称2")]
        public string Name2 { get; set; }

        [Display(Name = "名称3")]
        public string Name3 { get; set; }

        [Display(Name = "名称4")]
        public string Name4 { get; set; }

        [Display(Name = "排序号")]
        public int? Seqid { get; set; }
    }
}
