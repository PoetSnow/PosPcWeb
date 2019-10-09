using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.ResInvoiceManage
{
    /// <summary>
    /// 发票界面中右侧显示的发票项目信息模型
    /// </summary>
    public class ResInvoiceDetailInfo
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        [Display(Name = "项目")]
        public string ItemTaxid { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        [Display(Name = "项目")]
        public string ItemTaxName { get; set; }

        /// <summary>
        /// 含税金额
        /// </summary>
        [Display(Name = "含税金额")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// 税金
        /// </summary>
        [Display(Name = "税金")]
        public decimal? AmountTax { get; set; }

        /// <summary>
        /// 不含税金额
        /// </summary>
        [Display(Name = "不含税金额")]
        public decimal? AmountNoTax { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        [Display(Name = "税率")]
        public decimal? RateTax { get; set; }
    }
}
