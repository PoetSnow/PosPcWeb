using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.MarketingManage
{
    public interface IRoomOwnerFeeService : ICRUDService<RoomOwnerFee>
    {
        decimal? getlastpreReadQty(string hid, string roomno, string itemid);
        bool delCurdayImport(string hid,string itemid);
    }
}
