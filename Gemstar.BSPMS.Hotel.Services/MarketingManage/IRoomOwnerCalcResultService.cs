using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.MarketingManage
{
    public interface IRoomOwnerCalcResultService : ICRUDService<RoomOwnerCalcResult>
    {
        JsonResultData regenerateRoomOwnerCalcResult(string hid, DateTime dt);
        DataTable getRoomOwnerMonthCalc(string hid, DateTime dt);
        List<RentSituation> getRoomOwnerRentSituat(string hid, DateTime? dt, string profileid);
        string getStatusPublishCalcResult(string hid, DateTime dt);
        JsonResultData setStatusPublishCalcResult(string hid, DateTime dt,bool ispublish);
    }
}
