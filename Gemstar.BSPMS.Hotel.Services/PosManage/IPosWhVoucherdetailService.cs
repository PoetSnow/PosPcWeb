using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.PosManage
{
    public interface IPosWhVoucherdetailService : ICRUDService<WhVoucherdetail>
    {
        List<WhVoucherdetail> GetList(string hid, string billId);

        int Del(string hid, string id);
    }
}
