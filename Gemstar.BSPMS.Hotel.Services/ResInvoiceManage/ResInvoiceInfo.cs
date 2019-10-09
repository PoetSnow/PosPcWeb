using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.ResInvoiceManage
{
    /// <summary>
    /// 发票界面中左侧显示的发票信息模型
    /// </summary>
    public class ResInvoiceInfo
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Display(Name = "主键ID")]
        public Guid Id { get; set; }

        /// <summary>
        /// 发票的关联类型。 0:和订单号相关  1:和会员账务相关  2:和合约单位账务相关
        /// </summary>
        [Display(Name = "业务来源")]
        public byte? Reftype { get; set; }
        /// <summary>
        /// 业务来源名称
        /// </summary>
        [Display(Name = "业务来源")]
        public string ReftypeName { get; set; }

        /// <summary>
        /// 订单Id
        /// </summary>
        [Display(Name = "订单Id")]
        public string Resid { get; set; }

        /// <summary>
        /// 会员账务Id
        /// </summary>
        [Display(Name = "会员账务Id")]
        public Guid? ProfileCaid { get; set; }

        /// <summary>
        /// 合约单位账务Id
        /// </summary>
        [Display(Name = "合约单位账务Id")]
        public Guid? Companycaid { get; set; }

        /// <summary>
        /// 发票类型。0：普通发票， 1：增值税专用发票
        /// </summary>
        [Display(Name = "发票类型")]
        public byte? TaxType { get; set; }
        /// <summary>
        /// 发票类型名称
        /// </summary>
        [Display(Name = "发票类型")]
        public string TaxTypeName { get; set; }

        /// <summary>
        /// 发票打印类型。0：纸质发票， 1：电子发票
        /// </summary>
        [Display(Name = "发票打印类型")]
        public byte? PrintType { get; set; }

        /// <summary>
        /// 发票号码
        /// </summary>
        [Required(ErrorMessage = "请输入发票号码")]
        [Display(Name = "发票号码")]
        public string No { get; set; }

        /// <summary>
        /// 发票抬头
        /// </summary>
        [Display(Name = "发票抬头")]
        public string TaxName { get; set; }

        /// <summary>
        /// 税务登记号
        /// </summary>
        [Display(Name = "税务登记号")]
        public string TaxNo { get; set; }

        /// <summary>
        /// 发票地址和电话
        /// </summary>
        [Display(Name = "发票地址和电话")]
        public string TaxAddTel { get; set; }

        /// <summary>
        /// 发票银行和账号
        /// </summary>
        [Display(Name = "发票银行和账号")]
        public string TaxBankAccount { get; set; }

        /// <summary>
        /// 开票营业日
        /// </summary>
        [Display(Name = "开票营业日")]
        public DateTime? BsnsDate { get; set; }

        /// <summary>
        /// 开票时间
        /// </summary>
        [Display(Name = "开票时间")]
        public DateTime? CDate { get; set; }

        /// <summary>
        /// 开票人姓名
        /// </summary>
        [Display(Name = "开票人姓名")]
        public string InputUser { get; set; }

        /// <summary>
        /// 税务发票代码
        /// </summary>
        [Display(Name = "税务发票代码")]
        public string InvoiceCode { get; set; }

        /// <summary>
        /// 税务发票号
        /// </summary>
        [Display(Name = "税务发票号")]
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 税务发票批号
        /// </summary>
        [Display(Name = "税务发票批号")]
        public string InvoiceSeq { get; set; }

        /// <summary>
        /// 原税务发票代码。冲红时用，后面是数字0
        /// </summary>
        [Display(Name = "原税务发票代码")]
        public string InvoiceCode0 { get; set; }

        /// <summary>
        /// 原税务发票号
        /// </summary>
        [Display(Name = "原税务发票号")]
        public string InvoiceNo0 { get; set; }

        /// <summary>
        /// 红字发票信息
        /// </summary>
        [Display(Name = "红字发票信息")]
        public string RedInfo { get; set; }

        /// <summary>
        /// 是否冲销。0 false:正常发票    1 true：冲红发票
        /// </summary>
        [Display(Name = "是否冲销")]
        public bool? Isread { get; set; }

        /// <summary>
        /// 是否作废。0 false:正常   1 true：作废
        /// </summary>
        [Display(Name = "是否作废")]
        public bool? IsCancel { get; set; }

        /// <summary>
        /// 发票金额 此字段只用来展示
        /// </summary>
        [Display(Name = "发票金额")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 源信息
        /// </summary>
        public string OriginInvoiceJsonData { get; set; }

        /// <summary>
        /// 发票界面中右侧显示的发票项目信息模型列表
        /// </summary>
        public List<ResInvoiceDetailInfo> InvoiceDetails { get; set; }
    }
}
