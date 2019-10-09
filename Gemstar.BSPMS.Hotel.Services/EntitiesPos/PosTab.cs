using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosTab")]
    [LogCName("餐台")]
    public class PosTab
    {
        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("台号")]
        public string TabNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("中文名")]
        public string Cname { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("英文名")]
        public string Ename { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("所属营业点id")]
        public string Refeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("所属餐台类型id")]
        public string TabTypeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("餐台类型代码")]
        public string TabTypeCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("餐台类型名称")]
        public string TabTypeName { get; set; }

        [LogCName("最大座位数")]
        public int? MaxSeat { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("模块")]
        public string Module { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("台状态")]
        public string StatNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("IP地址")]
        public string Ipaddress { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("继电器号")]
        public string RelayNo { get; set; }

        [LogCName("赠送限额")]
        public decimal? LargessLimit { get; set; }

        [LogCName("是否微信上预订")]
        public bool? IsWxUsed { get; set; }

        [LogCName("微信服务费率")]
        public decimal? ServiceRate { get; set; }

        [LogCName("微信茶位费")]
        public decimal? TeaPrice { get; set; }

        [LogCName("临时台标志")]
        public byte? Istagtemp { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("网络打印机")]
        public string NetPrinter { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("对应传菜部打印机")]
        public string ProdPrinter { get; set; }

        [LogCName("排列序号")]
        public int? Seqid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("修改时间")]
        public DateTime? ModifiedDate { get; set; }

        [LogCName("状态（1：启用，51：禁用）")]
        public byte? IStatus { get; set; }

        [LogCName("扫码点餐开台类型（1：同账单开台；2：分账单开台）")]
        public byte? IOpenType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("对应的出品部门")]
        public string DeptDepart { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("对应的出品部门")]
        public string DepartName { get; set; }
    }
}
