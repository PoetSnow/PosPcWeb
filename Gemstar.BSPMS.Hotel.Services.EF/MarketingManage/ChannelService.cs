using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using System.Collections.Generic;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Common.Services.EF;
using System.Data.Entity;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Entities;
using System;

namespace Gemstar.BSPMS.Hotel.Services.EF.MarketingManage
{
    public class ChannelService : CRUDService<Channel>, IChannelService
    {
        private DbHotelPmsContext _db;
        public ChannelService(DbHotelPmsContext db) : base(db, db.Channels)
        {
            _db = db;
        }

        public List<Channel> GetChannel(string hid)
        {
            return _db.Channels.Where(w => w.Hid == hid).ToList();
        }

        public List<RoomType> GetRoomType(string hid, bool isStatusEnable)
        {
            if (isStatusEnable)
            {
                return _db.RoomTypes.Where(w => w.Hid == hid && w.Status == EntityStatus.启用).OrderBy(w => w.Seqid).ToList();
            }
            else
            {
                return _db.RoomTypes.Where(w => w.Hid == hid).OrderBy(w => w.Seqid).ToList();
            }
        }

        public List<RoomType> GetRoomTypeforChanelValid(string hid)
        {
            return _db.RoomTypes.Where(w => w.Hid == hid && w.ChanelValid == true).OrderBy(w => w.Seqid).ToList();
        }

        public void RoomTypeSet(string hid)
        {
            _db.Database.ExecuteSqlCommand("execute [up_RoomStatusSet] @hid = {0} , @optype = 6 ", hid);
        }


        public void resetChannel(string hid, DbCommonContext masterDb)
        {
            //将中央库中的渠道和渠道状态同步到酒店业务库中
            var mChannels = masterDb.M_v_channelCodes.ToList();
            var hotelChannels = masterDb.HotelChannels.Where(w => w.Hid == hid).ToList();
            var channels = _db.Channels.Where(w => w.Hid == hid).ToList();
            foreach (var mc in mChannels)
            {
                var hc = hotelChannels.SingleOrDefault(w => w.ChannelCode == mc.Code);
                var c = channels.SingleOrDefault(w => w.Code == mc.Code);
                if (c == null)
                {
                    c = new Channel
                    {
                        Hid = hid,
                        Id = string.Format("{0}05{1}", hid, mc.Code),
                        Code = mc.Code,
                        Name = mc.Name,
                        isvalid = false,
                        ItfVersion = "direct"
                    };
                    if (hc != null)
                    {
                        c.isvalid = hc.Isvalid;
                    } 
                    _db.Channels.Add(c);
                }
                else
                {
                    if (hc != null)
                    {
                        c.isvalid = hc.Isvalid == null ? false : hc.Isvalid;
                        _db.Entry(c).State = EntityState.Modified;
                    }
                }
            }
            //将通用代码视图中的渠道同步到酒店渠道信息表中
            masterDb.Database.ExecuteSqlCommand("INSERT INTO hotelChannel (id,hid,channelCode,channelName,refno) SELECT NEWID(), {0}, code, name, '' FROM m_v_channelCode b WHERE code not in(SELECT channelCode FROM hotelChannel WHERE hid = {0}); UPDATE hotelChannel SET  channelName = m.name FROM hotelChannel c,m_v_channelCode m WHERE c.channelCode = m.code AND hid = {0}", hid);
            //将渠道同步到酒店业务库中的客人来源中
            var sources = _db.CodeLists.Where(w => w.Hid == hid && w.TypeCode == "05").ToList();
            foreach (var mc in mChannels)
            {
                var s = sources.SingleOrDefault(w => w.Code == mc.Code);
                if (s == null)
                {
                    s = new CodeList
                    {
                        Id = string.Format("{0}05{1}", hid, mc.Code),
                        Hid = hid,
                        Code = mc.Code,
                        Name = mc.Name,
                        TypeCode = "05",
                        TypeName = "客人来源"
                    };
                    _db.CodeLists.Add(s);
                }
            }
            _db.Database.ExecuteSqlCommand("execute[up_RoomStatusSet] @hid = {0}, @optype = 6", hid);
            _db.SaveChanges();
        }

        protected override Channel GetTById(string id)
        {
            return new Channel() { Id = id };
        }

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
        public string GetZHMappingUrl(string hid, string channelId, string userCode, bool isTest, List<M_v_channelPara> channelParas)
        {
            var urlParaCode = isTest ? "JointWisdomMappingUrlTest" : "JointWisdomMappingUrl";
            var version = _db.Database.SqlQuery<string>("SELECT itfVersion FROM dbo.channel WHERE hid = {0} AND id = {1}", hid, channelId).SingleOrDefault();
            string providerParaCode;
            if (!string.IsNullOrWhiteSpace(version) && version == "ebooking")
            {
                providerParaCode = isTest ? "JointWisdomEBKProviderCodeTest" : "JointWisdomEBKProviderCode";
            }
            else
            {
                providerParaCode = isTest ? "JointWisdomProviderCodeTest" : "JointWisdomProviderCode";
            }
            var urlPara = channelParas.SingleOrDefault(w => w.Code == urlParaCode);
            var providerPara = channelParas.SingleOrDefault(w => w.Code == providerParaCode);
            var hotelCodeInChannel = _db.Database.SqlQuery<string>("SELECT c.refno FROM channel AS c INNER JOIN m_v_channelCode AS v ON v.code = c.code WHERE c.hid = {0} AND c.id = {1} AND v.switch = 'zh'", hid, channelId).SingleOrDefault();
            if (string.IsNullOrWhiteSpace(hotelCodeInChannel))
            {
                return string.Empty;
            }
            return string.Format(urlPara.Value, providerPara.Value, hotelCodeInChannel, userCode);
        }

        public List<M_v_channelCode> getM_v_channelCodes(DbCommonContext masterDb)
        {
            var mChannels = masterDb.M_v_channelCodes.ToList();
            return mChannels;
        }
    }
}