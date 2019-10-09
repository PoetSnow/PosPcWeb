using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.CRMManage
{
    public class CommpanyBlanceInfo
    {
        public string RateCode { get; set; }
        public string RateName { get; set; }

        public int? LimitAmount { get; set; }

        public decimal? Balance { get; set; }

        public DateTime? ValidDate { get; set; }
    }
}
