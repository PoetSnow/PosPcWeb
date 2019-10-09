using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Services.MarketingManage
{
    public interface IChannelService : ICRUDService<Channel>
    {
        /// <summary>
        /// 获取指定酒店的渠道信息
        /// </summary>
        /// <param name="hid">酒店编号</param>
        /// <returns></returns>
        List<Channel> GetChannel(string hid);
        List<RoomType> GetRoomType(string hid, bool isStatusEnable);
        List<RoomType> GetRoomTypeforChanelValid(string hid);
        void RoomTypeSet(string hid);
        //	--同步分库中的渠道信息（channel）
        void resetChannel(string hid, DbCommonContext masterDb);
        /// <summary>
        /// 获取众荟渠道下的自助匹配产品链接地址
        /// 如果指定渠道是众荟下的，则返回实际地址，否则返回空字符串
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="channelId">渠道id</param>
        /// <param name="userCode">当前登录用户代码</param>
        /// <param name="isTest">是否测试环境</param>
        /// <param name="channelParas">渠道参数列表</param>
        /// <returns>如果指定渠道是众荟下的，则返回实际地址，否则返回空字符串</returns>
        string GetZHMappingUrl(string hid, string channelId,string userCode, bool isTest,List<M_v_channelPara> channelParas);

        List<M_v_channelCode> getM_v_channelCodes(DbCommonContext masterDb);
    }
}
