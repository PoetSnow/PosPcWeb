using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_getPrePayInfoResult
    {
        public string GuestName { get; set; }

        public string Mobile { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? BalanceAmount { get; set; }
        public string Business { get; set; }
        public string PosName { get; set; }
        public string ShiftName { get; set; }
        public string HandBillNo { get; set; }
        public string UseDesc { get; set; }
        public string Remark { get; set; }
        public string UsedDate { get; set; }
    }
}
