using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Web.Models;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosInvoiceItem
{
    public class InvoiceItemEditViewModel : BaseEditViewModel
    {
        public string Id { get; set; }


        [Display(Name = "代码")]
        [Required(ErrorMessage = "请输入项目代码")]
        public string Code { get; set; }

        [Display(Name = "项目名称")]
        [Required(ErrorMessage = "请输入项目名称")]
        public string Name { get; set; }

        [Display(Name = "发票项目代码")]
        [Required(ErrorMessage = "请输入发票项目代码")]
        public string Name2 { get; set; }

        [Display(Name = "发票项目名称")]
        [Required(ErrorMessage = "请输入发票项目名称")]
        public string Name3 { get; set; }

        [Display(Name = "税率")]
        // [Required(ErrorMessage = "请输入税率")]
        [RegularExpression(@"([1-9]?\d|100)$", ErrorMessage ="输入的数字不合法")]
        public string Name4 { get; set; }


        public string TypeCode { get; set; }


        public string TypeName { get; set; }

        public int Pk { get; set; }

        public EntityStatus Status { get; set; }


        /// <summary>
        /// 用来存储（服务费，抹零 消费余额）
        /// </summary>
        [Display(Name = "特殊项目")]
        public string Remark { get; set; }

        [Display(Name = "特殊项目")]
        public string[] Remarks
        {
            get { return string.IsNullOrEmpty(Remark) ? new string[] { } : Remark.Split(','); }
            set { Remarks = value; }
        }

        [Display(Name = "单位")]
        public string ZoneCode { get; set; }
    }
}