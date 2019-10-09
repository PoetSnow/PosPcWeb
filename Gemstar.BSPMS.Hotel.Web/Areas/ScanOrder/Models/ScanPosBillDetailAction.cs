using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models
{
    /// <summary>
    /// 扫码点餐作法
    /// </summary>
    public class ScanPosBillDetailAction
    {
        /// <summary>
        /// 账单明细行号
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 作法行号
        /// </summary>
        public int ActionOrderId { get; set; }

        /// <summary>
        /// 酒店代码
        /// </summary>
        public string Hid { get; set; }

        /// <summary>
        /// 作法组别
        /// </summary>
        public int? Igroupid { get; set; }

        /// <summary>
        /// 作法代码
        /// </summary>
        public string ActionNo { get; set; }

        /// <summary>
        /// 作法名称
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// 作法加价
        /// </summary>
        public decimal? AddPrice { get; set; }

        /// <summary>
        /// 作法倍数
        /// </summary>
        public decimal? Nmultiple { get; set; }

        /// <summary>
        /// 是否与数量相关
        /// </summary>
        public bool? IByQuan { get; set; }

        /// <summary>
        /// 是否与人数相关
        /// </summary>
        public bool? IByGuest { get; set; }

        /// <summary>
        /// Quan
        /// </summary>
        public decimal? Quan { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        /// 部门类别id
        /// </summary>
        public string DeptClassid { get; set; }

        /// <summary>
        /// 出品打印机
        /// </summary>
        public string PrtNo { get; set; }

        /// <summary>
        /// 修改操作员
        /// </summary>
        public string ModiUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime? ModiDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 数量相关最低数量
        /// </summary>
        public decimal? LimitQuan { get; set; }
    }
}