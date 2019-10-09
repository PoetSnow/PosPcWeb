using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.SystemManage.Models.ItemManage
{
    public class ItemAddViewModel
    {
        public ItemAddViewModel()
        {
            Status = EntityStatus.启用;
        }
        [Required(ErrorMessage = "请输入项目代码")]
        [Display(Name = "项目代码")]
        [Column(TypeName = "varchar")]
        public string Code { get; set; }

        [Required(ErrorMessage = "请输入项目名称")]
        [Display(Name = "项目名称")]
        [Column(TypeName = "varchar")]
        public string Name { get; set; }

        [Display(Name = "别名")]
        [Column(TypeName = "varchar")]
        public string Alias { get; set; }

        [Display(Name = "类别代码")]
        [Column(TypeName = "varchar")]
        public string ItemTypeid { get; set; }

        [Display(Name = "类别名称")]
        [Column(TypeName = "varchar")]
        public string ItemTypeName { get; set; }

        [Display(Name = "含税单价")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "请输入增值税率")]
        [Display(Name = "增值税率")]
        public decimal Taxrate { get; set; }

        [Display(Name = "统计间夜数")]
        public decimal? Nights { get; set; }

        [Display(Name = "是否可手工录入")]
        public bool? IsInput { get; set; }

        [Display(Name = "是否录入数量")]
        public bool? IsQuantity { get; set; }

        [Display(Name = "是否找回")]
        public bool? IsRetun { get; set; }

        [Display(Name = "是否可充值")]
        public bool? IsCharge { get; set; }

        [Display(Name = "状态")]
        public EntityStatus Status { get; set; }

        [Display(Name = "付款还是消费")]
        [Column(TypeName = "char")]
        public string DcFlag { get; set; }

        [Display(Name = "统计分类")]
        [Column(TypeName = "varchar")]
        public string StaType { get; set; }

        [Display(Name = "处理方式")]
        [Column(TypeName = "varchar")]
        public string Action { get; set; }

        [Display(Name = "发票项目")]
        [Column(TypeName = "char")]
        public string InvoiceItemid { get; set; }
        
        [Display(Name = "排序号")]
        public int? Seqid { get; set; }

        [Display(Name = "汇率")]
        public decimal? Rate { get; set; }

        [Display(Name = "是否可积分")]
        public bool? Notscore { get; set; }

        [Display(Name = "是否业主费用")]
        public bool IsOwnerFee { get; set; }

        [Display(Name = "是否计入业主房租")]
        public bool IsOwnerAmount { get; set; }

        [Display(Name = "业主属性")]
        public string OwnerProperty { get; set; }

    }
}
