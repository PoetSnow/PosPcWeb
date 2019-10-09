using Gemstar.BSPMS.Common.Services.Entities;

namespace Gemstar.BSPMS.Common.Services
{
    public interface IBeforeLoginService
    {
        JsonResultData GetConnectionString(string hid);

        JsonResultData InsertHotelToCenterDB(CenterHotel hotel);

        JsonResultData DeleteHotelFromCenterDB(string hid);

        JsonResultData ResetUserPassword(string hid, string account, string pwd);

        JsonResultData CheckHotelUser(string hid, string account);        

        JsonResultData CopyModelHotel(CenterHotel hotel, string loginCode = "", string loginName = "", string pwd = "", int isReg = 1);
    }
}
