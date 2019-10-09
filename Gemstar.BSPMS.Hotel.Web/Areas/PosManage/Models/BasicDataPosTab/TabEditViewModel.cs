using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosTab
{
    public class TabEditViewModel : BaseEditViewModel
    {
        [Display(Name = "id")]
        [Required(ErrorMessage = "请输入Id")]
        public string Id { get; set; }

        [Display(Name = "台号")]
        [Required(ErrorMessage = "请输入台号")]
        public string TabNo { get; set; }

        [Display(Name = "中文名称")]
        [Required(ErrorMessage = "请输入中文名")]
        public string Cname { get; set; }

        [Display(Name = "英文名称")]
        public string Ename { get; set; }

        [Display(Name = "营业点")]
        public string Refeid { get; set; }

        [Display(Name = "餐台类型")]
        public string TabTypeid { get; set; }

        [Display(Name = "模块")]
        public string Module { get; set; }

        [Display(Name = "餐台状态")]
        public string Statno { get; set; }

        [Display(Name = "IP地址")]
        public string Ipaddress { get; set; }

        [Display(Name = "继电器号")]
        public string RelayNo { get; set; }

        [Display(Name = "赠送限额")]
        public decimal? LargessLimit { get; set; }

        [Display(Name = "是否移动端预订")]
        public bool? IsWxUsed { get; set; }

        //[Display(Name = "微信服务费率")]
        //public decimal? ServiceRate { get; set; }

        //[Display(Name = "微信茶位费")]
        //public decimal? TeaPrice { get; set; }

        [Display(Name = "临时台标志")]
        public byte? Istagtemp { get; set; }

        [Display(Name = "网络打印机")]
        public string NetPrinter { get; set; }

        [Display(Name = "对应传菜部打印机")]
        public string ProdPrinter { get; set; }

        [Display(Name = "对应传菜部打印机")]
        public string[] ProdPrinters
        {
            get { return string.IsNullOrEmpty(ProdPrinter) ? new string[] { } : (string[])GetSeparateSubString(ProdPrinter, 3).ToArray(typeof(string)); }
            set { ProdPrinters = value; }
        }

        [Display(Name = "排列序号")]
        public int? Seqid { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public DateTime? ModifiedDate { get; set; }

        [Display(Name = "餐台类型代码")]
        public string TabTypeCode { get; set; }

        [Display(Name = "餐台类型名称")]
        public string TabTypeName { get; set; }

        [Display(Name = "扫码点餐开台类型")]
        public byte? IOpenType { get; set; }


        public int? MaxSeat { get; set; }

        [Display(Name = "对应的出品部门")]
        public string DeptDepart { get; set; }

        [Display(Name = "对应的出品部门")]
        public string DepartName { get; set; }

       
    }
}