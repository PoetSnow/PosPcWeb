using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models
{
    public class PaymentMethodViewModel
    {
        public long Id { get; set; }
        
        public string Cname { get; set; }

        public decimal? Amount { get; set; }

        public decimal? Rate { get; set; }

        public decimal? standardCurrency { get; set; }
    }
}