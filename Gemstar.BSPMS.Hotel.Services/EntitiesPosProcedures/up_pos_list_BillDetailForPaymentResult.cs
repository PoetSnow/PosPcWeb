using System;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_pos_list_BillDetailForPaymentResult 执行后的结果集对象
    /// </summary>
    public class up_pos_list_BillDetailForPaymentResult
    {
        public long Row { get; set; }

        public long Id { get; set; }

        public string Billid { get; set; }

        public string mBillid { get; set; }

        public string Cname { get; set; }

        public decimal? Amount { get; set; }

        public decimal? Rate { get; set; }

        public decimal? Dueamount { get; set; }

        public Guid? Settleid { get; set; }

        public string SettleTransno { get; set; }

        public string SettleTransName { get; set; }

        public DateTime? SettleBsnsDate { get; set; }

        public string SettleShuffleid { get; set; }

        public string SettleShiftId { get; set; }

        public string SettleUser { get; set; }

        public DateTime? SettleDate { get; set; }
    }
}