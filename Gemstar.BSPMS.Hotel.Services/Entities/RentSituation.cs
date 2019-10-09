using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    public class RentSituation
    {
        public string Roomno { get; set; }
        public DateTime TransBsnsdate { get; set; }
        public decimal? Rate { get; set; }
        public decimal? nights { get; set; }
    }
}
