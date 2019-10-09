using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.ItemScoreManage
{
    public class ItemScoreAddViewModel
    {
        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入登录代码")]
        public string Code { get; set; }

        [Display(Name = "名称")]
        [Required(ErrorMessage = "请输入名称")]
        public string Name { get; set; }

        [Display(Name = "文字介绍")]
        public string Remark { get; set; }

        [Display(Name = "排序号")]
        public int? Seqid { get; set; }

        [Display(Name = "图片")]
        public string PicAdd { get; set; }

        [Display(Name = "适用分店")]
        public string BelongHotel { get; set; }
    }
}