using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Services.MarketingManage
{
   public interface IRoomHoldService: ICRUDService<RoomHold>
    {
        /// <summary>
        /// 获取指定酒店的预留房信息
        /// </summary>
        /// <param name="hid">酒店编号</param>
        /// <returns></returns>
        List<RoomHold> GetRoomHold(string hid,string channelid,string roomtype,int year,int month);
        SelectList GetChannel(string hid);
        SelectList GetRoomType(string hid);
        string UpdateRoomHold(string begintime, string endtime, string strarr, string channelid, string roomtype,string hid);
        /// <summary>
        /// 查询酒店的渠道可售房设置和可用信息
        /// </summary>
        /// <param name="hid">酒店编号</param>
        /// <param name="channelId">渠道id，为空表示全部</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="days">查询天数</param>
        /// <returns>酒店的渠道可售房设置和可用信息</returns>
        List<UpQueryRoomHoldInfosResultForshow> QueryRoomHoldInfos(string hid, string channelId, DateTime beginDate, int days);
    }
}
