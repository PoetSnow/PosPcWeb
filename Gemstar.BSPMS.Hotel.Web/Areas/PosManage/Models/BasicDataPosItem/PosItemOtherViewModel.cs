using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemMultiClass;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemPrice;
using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemRefe;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItem
{
    /// <summary>
    /// 增加单位价格/项目大类/营业厅
    /// </summary>
    public class PosItemOtherViewModel
    {
        public PosItemOtherViewModel()
        {
            this.PosItemPrice = new PosItemPriceAddViewModel();
            this.PosItemMultiClass = new PosItemMultiClassAddViewModel();
            this.PosItemRefe = new PosItemRefeAddViewModel();
        }

        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public string Id { get; set; }

        [Display(Name = "项目代码")]
        public string Code { get; set; }

        [Display(Name = "中文名称")]
        public string Cname { get; set; }

        [Display(Name = "单位价格")]
        public PosItemPriceAddViewModel PosItemPrice { get; set; }

        [Display(Name = "项目大类")]
        public PosItemMultiClassAddViewModel PosItemMultiClass { get; set; }

        [Display(Name = "营业厅")]
        public PosItemRefeAddViewModel PosItemRefe { get; set; }
    }
}