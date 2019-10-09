using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosService
{
    public class PosTabOpenItemEditViewModel : BaseEditViewModel
    {
        [Display(Name = "id")]
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
        
        [Display(Name = "消费项目")]
        [Required(ErrorMessage = "请选择消费项目")]
        public string Itemid { get; set; }
        
        [Display(Name = "单位")]
        [Required(ErrorMessage = "请选择单位")]
        public string Unitid { get; set; }

        [Display(Name = "数量")]
        [Required(ErrorMessage = "请输入数量")]
        public decimal Quantity { get; set; }

        [Display(Name = "价格")]
        [Required(ErrorMessage = "请输入价格")]
        public decimal? Price { get; set; }

        [Display(Name = "数量方式")]
        public byte QuanMode { get; set; }

        [Display(Name = "收费状态")]
        [Required(ErrorMessage = "请选择收费状态")]
        public byte? IsCharge { get; set; }

        [Display(Name = "是否飞单")]
        public bool? IsProduce { get; set; }

        [Display(Name = "是否可取消")]
        public bool? IsCancel { get; set; }
        
        [Display(Name = "模块")]
        public string Module { get; set; }
        
        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? ModifiedDate { get; set; }
    }
}