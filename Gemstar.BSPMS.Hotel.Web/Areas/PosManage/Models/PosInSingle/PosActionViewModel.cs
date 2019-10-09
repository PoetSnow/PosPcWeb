using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class PosActionViewModel
    {
        public PosActionViewModel()
        {
            PageIndex = 1;
            PageSize = 1;
            PageTotal = 1;
        }

        [Display(Name = "作法类型")]
        public string ActionTypeId { get; set; }

        [Display(Name = "当前页")]
        public int PageIndex { get; set; }

        [Display(Name = "每页记录数")]
        public int PageSize { get; set; }

        [Display(Name = "总记录数")]
        public int PageTotal { get; set; }

        [Display(Name = "主账单ID")]
        public string mBillid { get; set; }

        [Display(Name = "账单明细ID")]
        public int mId { get; set; }

        [Display(Name = "分组ID")]
        public int igroupid { get; set; }
    }
}