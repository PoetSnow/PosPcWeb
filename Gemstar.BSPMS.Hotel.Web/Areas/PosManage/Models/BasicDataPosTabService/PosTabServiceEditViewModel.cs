using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosService
{
    public class PosTabServiceEditViewModel : BaseEditViewModel
    {
        [Display(Name = "Id")]
        [Required(ErrorMessage = "请输入Id")]
        public Guid Id { get; set; }

        [Display(Name = "营业点")]
        public string Refeid { get; set; }

        [Display(Name = "餐台类型")]
        public string TabTypeid { get; set; }

        [Display(Name = "客人类型")]
        public string CustomerTypeid { get; set; }

        [Display(Name = "日期类型")]
        [Required(ErrorMessage = "请选择日期类型")]
        public byte? ITagperiod { get; set; }

        [Display(Name = "开始时间")]
        [Required(ErrorMessage = "请输入开始时间")]
        public string StartTime { get; set; }

        [Display(Name = "结束时间")]
        [Required(ErrorMessage = "请输入结束时间")]
        public string EndTime { get; set; }

        [Display(Name = "服务费率")]
        public decimal? Servicerate { get; set; }

        [Display(Name = "默认折扣")]
        public decimal? Discount { get; set; }

        [Display(Name = "最低消费")]
        public decimal? NLimit { get; set; }

        [Display(Name = "最低消费计法")]
        public byte? IsByPerson { get; set; }

        [Display(Name = "最低消费时长")]
        public int? LimitTime { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? ModifiedDate { get; set; }
    }
}