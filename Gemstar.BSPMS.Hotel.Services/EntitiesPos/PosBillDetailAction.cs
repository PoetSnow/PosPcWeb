using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPos
{
    [Table("PosBillDetailAction")]
    [LogCName("账单作法明细表")]
    public class PosBillDetailAction
    {
        [Key]
        [LogCName("id")]
        public Int64 Id { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("账单id")]
        public string MBillid { get; set; }
        
        [LogCName("明细id")]
        public Int64? Mid { get; set; }
        
        [LogCName("作法组别")]
        public int? Igroupid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("作法代码")]
        public string ActionNo { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("作法名称")]
        public string ActionName { get; set; }
        
        [LogCName("作法加价")]
        public decimal? AddPrice { get; set; }
        
        [LogCName("作法倍数")]
        public decimal? Nmultiple { get; set; }
        
        [LogCName("是否与数量相关")]
        public bool? IByQuan { get; set; }
        
        [LogCName("是否与人数相关")]
        public bool? IByGuest { get; set; }
        
        [LogCName("数量")]
        public decimal? Quan { get; set; }
        
        [LogCName("金额")]
        public decimal? Amount { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("部门类别id")]
        public string DeptClassid { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("出品打印机")]
        public string PrtNo { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("修改操作员")]
        public string ModiUser { get; set; }
        
        [LogCName("修改时间")]
        public DateTime? ModiDate { get; set; }
        
        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Memo { get; set; }

        /// <summary>
        /// 1:基础资料的做法 2：菜式做法
        /// </summary>

        [Column(TypeName = "varchar")]
        [LogCName("作法类型")]
        public string ActionType { get; set; }

    }
}