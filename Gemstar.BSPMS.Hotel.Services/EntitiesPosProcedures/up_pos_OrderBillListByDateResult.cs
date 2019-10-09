﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 预订账单列表up_pos_OrderBillListByDate 存储过程查询结果
    /// </summary>
   public class up_pos_OrderBillListByDateResult
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

 
        public string OpenMemo { get; set; }

       
        public string CashMemo { get; set; }

       
        public byte? TabFlag { get; set; }

        
        public string Memo { get; set; }

       
        public Int64? Accid { get; set; }

       
        public decimal? TaxAmt { get; set; }

     
        public decimal? ServiceAmt { get; set; }

     
        public decimal? LimitBalance { get; set; }

    
        public decimal? BlotAmt { get; set; }

       
        public decimal? LargessAmt { get; set; }

       
        public decimal? PayAmt { get; set; }

        public decimal? InvoiceAmt { get; set; }

       
        public DateTime? LastRecord { get; set; }

     
        public DateTime? PrintRecord { get; set; }

      
        public string OpenWxid { get; set; }

     
        public DateTime? OrderDate { get; set; }

   
        public DateTime? OrderOperDate { get; set; }

  
        public string OrderUser { get; set; }


        public DateTime? OrderExpired { get; set; }

        public string StatusText { get; set; }
    }
}
