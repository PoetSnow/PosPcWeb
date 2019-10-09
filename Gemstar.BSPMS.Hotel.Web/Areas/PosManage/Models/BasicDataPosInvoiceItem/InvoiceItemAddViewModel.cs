using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosInvoiceItem
{
    public class InvoiceItemAddViewModel
    {
        private string typeCode = "66";

        private string typeName = "云Pos发票开票项目";


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
        [RegularExpression(@"([1-9]?\d|100)$", ErrorMessage = "输入的数字不合法")]
        public string Name4 { get; set; }


        public string TypeCode
        {
            get { return typeCode; }
            set { }
        }


        public string TypeName
        {
            get { return typeName; }
            set { }
        }

        /// <summary>
        /// 用来存储（服务费，抹零 消费余额）
        /// </summary>
        [Display(Name = "特殊项目")]
        public  string  Remark { get; set; }

        [Display(Name = "单位")]
        public string ZoneCode { get; set; }
    }
}