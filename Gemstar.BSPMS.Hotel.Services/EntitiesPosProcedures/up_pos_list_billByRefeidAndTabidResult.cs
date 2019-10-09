using System;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_pos_list_billByRefeidAndTabidResult 执行后的结果集对象
    /// </summary>
    public class up_pos_list_billByRefeidAndTabidResult
    {
        public string Hid { get; set; }
        
        public string Billid { get; set; }
        
        public string BillNo { get; set; }
        
        public string MBillid { get; set; }
        
        public string Name { get; set; }
        
        public string Mobile { get; set; }
        
        public int? IGuest { get; set; }
        
        public DateTime? BillBsnsDate { get; set; }
        
        public string InputUser { get; set; }
        
        public DateTime? BillDate { get; set; }
        
        public DateTime? DepBsnsDate { get; set; }
        
        public string MoveUser { get; set; }
        
        public DateTime? DepDate { get; set; }
        
        public string Module { get; set; }
        
        public string Refeid { get; set; }
        
        public string Tabid { get; set; }
        
        public string TabNo { get; set; }

        public string TabName { get; set; }

        public string Keyid { get; set; }
        
        public string Invno { get; set; }
        
        public string Sale { get; set; }
        
        public string LinkNo { get; set; }
        
        public string Profileid { get; set; }
        
        public string CardNo { get; set; }
        
        public string Cttid { get; set; }
        
        public string CttName { get; set; }
        
        public string Consumer { get; set; }
        
        public string Approver { get; set; }
        
        public byte? IsForce { get; set; }
        
        public decimal? Discount { get; set; }
        
        public byte? DaType { get; set; }
        
        public decimal? DiscAmount { get; set; }
        
        public byte? Status { get; set; }
        
        public bool? IsOrder { get; set; }
        
        public bool? IsOver { get; set; }
        
        public string CustomerTypeid { get; set; }
        
        public bool? IsService { get; set; }
        
        public decimal? ServiceRate { get; set; }
        
        public bool? IsLimit { get; set; }
        
        public decimal? Limit { get; set; }
        
        public bool? IsByPerson { get; set; }
        
        public int? IHour { get; set; }
        
        public string Shiftid { get; set; }
        
        public string Shuffleid { get; set; }
        
        public int? IPrint { get; set; }
        
        public int? IPaidPrint { get; set; }
        
        public byte? IKtvStatus { get; set; }

        public byte? TabFlag { get; set; }

        public string OpenMemo { get; set; }
        
        public string CashMemo { get; set; }

        public string RefeName { get; set; }

        public bool? Isoutsell { get; set; }

        public string OpenInfo { get; set; }

        public string ShiftName { get; set; }

        public string ShuffleName { get; set; }

        public string billBsnsDateStr { get; set; }
    }
}
