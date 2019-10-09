using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services
{
   public interface IGuestService : ICRUDService<Guest>
    {
        /// <summary>
        /// 获取指定酒店下的记账项目列表
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        List<Guest> GetGuest(string hid);

        /// <summary>
        /// 获取指定酒店下的记账项目列表
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="guestName"></param>
        /// <returns></returns>
        List<Guest> GetGuest(string hid, string guestName);

        /// <summary>
        /// 根据证件号获取熟客
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cerType">证件类型</param>
        /// <param name="cerId">证件号</param>
        /// <returns></returns>
        Guest GetGuestByCerId(string hid, string cerType, string cerId);

        /// <summary>
        /// 获取客历消费记录
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="guestid"></param>
        /// <returns></returns>
        List<UpQueryGuestTrans> GetGuestTrans(string hid, string guestid);
    }
}
