using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_pos_list_BillDetailByBillidResult 执行后的结果集对象
    /// </summary>
    public class up_pos_list_BillDetailByBillidResult
    {
        public string Row { get; set; }

        public Int64 Id { get; set; }

        public string Hid { get; set; }

        public string MBillid { get; set; }

        public string Billid { get; set; }

        public string Itemid { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public string Unitid { get; set; }

        public string UnitCode { get; set; }

        public string UnitName { get; set; }

        public decimal? Quantity { get; set; }

        public decimal? Piece { get; set; }

        public decimal? Multiple { get; set; }

        public decimal? Price { get; set; }

        public decimal? AddPrice { get; set; }

        public decimal? Dueamount { get; set; }

        public decimal? Amount { get; set; }

        public decimal? Service { get; set; }

        public decimal? Tax { get; set; }

        public string Place { get; set; }

        public string Action { get; set; }

        public string Request { get; set; }

        public string DcFlag { get; set; }

        public bool? IsCheck { get; set; }

        public byte? Isauto { get; set; }

        public byte? Status { get; set; }

        public byte? IsProduce { get; set; }

        public bool? SP { get; set; }
        
        public bool? SD { get; set; }

        public Guid? Upid { get; set; }

        public decimal? OverSuite { get; set; }

        public string Tabid { get; set; }

        public string Keyid { get; set; }

        public decimal? Discount { get; set; }

        public bool? IsDishDisc { get; set; }

        public decimal? DiscAmount { get; set; }

        public string BatchTime { get; set; }

        public string Sale { get; set; }

        public DateTime? TransBsnsDate { get; set; }

        public string TransShuffleid { get; set; }

        public string TransShiftid { get; set; }

        public string TransUser { get; set; }

        public DateTime? TransDate { get; set; }

        public string ModiUser { get; set; }

        public DateTime? ModiDate { get; set; }

        public Guid? Settleid { get; set; }

        public string SettleTransno { get; set; }

        public string SettleTransName { get; set; }

        public DateTime? SettleBsnsDate { get; set; }

        public string SettleShuffleid { get; set; }

        public string SettleShiftId { get; set; }

        public string SettleUser { get; set; }

        public DateTime? SettleDate { get; set; }

        public decimal? PriceOri { get; set; }

        public decimal? PriceClub { get; set; }

        public decimal? DiscountClub { get; set; }

        public string Approver { get; set; }

        public string CanReason { get; set; }

        public string Memo { get; set; }

        public decimal? Grossrate { get; set; }

        public bool? isHandWrite { get; set; }

        public bool? isService { get; set; }

        public bool? isLimit { get; set; }

        public string DeptClassid { get; set; }

        public string DeptClassName { get; set; }

        public string statusString { get; set; }

        public string isHandWriteString { get; set; }

        public string isautoString { get; set; }

        public string isCheckString { get; set; }

        public string isDishDiscString { get; set; }

        public bool? isDiscount { get; set; }

        /// <summary>
        /// 项目是否可折扣
        /// </summary>
        public string isIsDiscountStringForItem { get; set; }

        /// <summary>
        /// 项目是否时价菜
        /// </summary>
        public bool? isCurrent { get; set; }

        /// <summary>
        /// 项目是否可赠送
        /// </summary>
        public bool? isLargess { get; set; }

        /// <summary>
        /// 是否套餐
        /// </summary>
        public bool? isSuite { get; set; }

        /// <summary>
        /// 是否酒席
        /// </summary>
        public bool? isFeast { get; set; }

        /// <summary>
        /// 是否自定义套餐/酒席
        /// </summary>
        public bool? isUserDefined { get; set; }

        /// <summary>
        /// 要求
        /// </summary>
        public string requestText { get; set; }

        /// <summary>
        /// 作法
        /// </summary>
        public string actionText { get; set; }

        /// <summary>
        /// （0：未传菜；1：已传菜）
        /// </summary>
        public byte? sentStatus { get; set; }

       
    }
}
