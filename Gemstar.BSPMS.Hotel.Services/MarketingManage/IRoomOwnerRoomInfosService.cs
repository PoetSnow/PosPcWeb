using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.MarketingManage
{
   public interface IRoomOwnerRoomInfosService:  ICRUDService<RoomOwnerRoomInfos>
    {   /// <summary>
        /// 模糊查询指定酒店内的业务员资料
        /// </summary>
        /// <param name="hid">指定酒店ID</param>
        /// <param name="nameOrMobile">业务员姓名或手机号</param>
        /// <param name="notName">不包括的业务员姓名</param>
        /// <returns></returns>
        List<RoomOwnerRoomInfos> List(string hid);

        List<RoomOwnerRoomInfos> getOwnerRoomList(string hid,Guid profile);

        RoomOwnerRoomInfos getOwnernamebyRoomId(string hid, string roomId);
        /// <summary>
        /// 根据房号得到房间编号
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="roomno"></param>
        /// <returns></returns>
        string getRoomIdbyRoomno(string hid, string roomno);
        Guid getprofileidByRoomno(string hid, string roomno);
        List<RoomOwnerRoomInfos> getOwnerRoomListbycalcTypeId(Guid calcTypeId);
    }
}
