using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class PosBillDetailAddViewModel
    {
        [Display(Name = "账单id")]
        public string MBillid { get; set; }

        [Display(Name = "业务单id")]
        public string Billid { get; set; }

        [Display(Name = "消费项目id")]
        public string Itemid { get; set; }

        [Display(Name = "消费项目代码")]
        public string ItemCode { get; set; }

        [Display(Name = "消费项目名称")]
        public string ItemName { get; set; }

        [Display(Name = "是否打折")]
        public bool? IsDiscount { get; set; }

        [Display(Name = "是否收服务费")]
        public bool? IsService { get; set; }

        [Display(Name = "单位id")]
        public string Unitid { get; set; }

        [Display(Name = "单位代码")]
        public string UnitCode { get; set; }

        [Display(Name = "单位名称")]
        public string UnitName { get; set; }

        [Display(Name = "数量")]
        public decimal? Quantity { get; set; }

        [Display(Name = "称重条只")]
        public decimal? Piece { get; set; }

        [Display(Name = "扣钝倍数")]
        public decimal? Multiple { get; set; }

        [Display(Name = "单价")]
        public decimal? Price { get; set; }

        [Display(Name = "作法加价")]
        public decimal? AddPrice { get; set; }

        [Display(Name = "客位")]
        public string Place { get; set; }

        [Display(Name = "作法")]
        public string Actions { get; set; }

        [Display(Name = "要求")]
        public string Request { get; set; }

        [Display(Name = "餐台id")]
        public string Tabid { get; set; }

        [Display(Name = "锁牌号")]
        public string Keyid { get; set; }

        [Display(Name = "原价")]
        public decimal? PriceOri { get; set; }

        [Display(Name = "会员价")]
        public decimal? PriceClub { get; set; }

        [Display(Name = "会员折扣")]
        public decimal? DiscountClub { get; set; }

        [Display(Name = "批准人")]
        public string Approver { get; set; }

        [Display(Name = "取消原因")]
        public string CanReason { get; set; }

        [Display(Name = "备注")]
        public string Memo { get; set; }

        /// <summary>
        /// 特价菜日期属性
        /// </summary>
        public string itagperiod { get; set; }

        [Display(Name = "所属套餐")]
        public Guid? Upid { get; set; }


        /// <summary>
        /// 称重属性(A:入单,B:海鲜池)
        /// </summary>
        public string WeighFlag { get; set; }


        /// <summary>
        /// 原称重
        /// </summary>
        public decimal? OriQuan { get; set; }

        /// <summary>
        /// 是否称重( 如果海鲜池称重了则更新为1，默认为0)
        /// </summary>
        public bool? IsWeight { get; set; }

        /// <summary>
        /// 称重用来记录主键ID
        /// </summary>
        public string mId { get; set; }

        /// <summary>
        /// 餐台号
        /// </summary>
        public string tabNo { get; set; }

        /// <summary>
        /// 餐台名
        /// </summary>
        public string tabName { get; set; }

        /// <summary>
        /// 原条只
        /// </summary>
        public decimal? OriPiece { get; set; }


        /// <summary>
        /// 是否时价菜
        /// </summary>
        public bool? IsCurrent { get; set; }


    }
}