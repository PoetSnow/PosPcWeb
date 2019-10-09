using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosReserve
{
    public class TabStatusList
    {
        public List<TimeList> TimeLists { get; set; }


        public List<up_pos_ReserveTabStatusResult> TabStatusLists { get; set; }
    }
}