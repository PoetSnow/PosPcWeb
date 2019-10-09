using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    /// <summary>
    /// 订单发票信息
    /// </summary>
    public class ResInvoiceInfo
    {
        [Display(Name = "主键ID")]
        public Guid? Id { get; set; }

        /// <summary>
        /// 0 false：普通发票， 1 true：增值税专用发票
        /// </summary>
        [Display(Name = "发票类型")]
        public bool? InvoiceType { get; set; }

        [Display(Name = "发票抬头")]
        public string TaxName { get; set; }

        [Display(Name = "税务登记号")]
        public string TaxNo { get; set; }

        [Display(Name = "地址和电话")]
        public string TaxAddTel { get; set; }

        [Display(Name = "开户银行和账号")]
        public string TaxBankAccount { get; set; }

        /// <summary>
        /// 原始订单发票信息的json字符串
        /// </summary>
        public string OriginResInvoiceInfoJsonData { get; set; }
    }
}
