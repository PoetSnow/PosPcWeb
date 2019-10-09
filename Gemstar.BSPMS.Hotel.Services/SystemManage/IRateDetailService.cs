using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;
using System;

namespace Gemstar.BSPMS.Hotel.Services
{
    public interface IRateDetailService : ICRUDService<RateDetail>
    {

        /// <summary>
        /// 获取引用价格体系明细信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        List<RateDetail> GetRateDetail(string hid);
        /// <summary>
        /// 获取指定时间段，指定房型，指定价格码的明细价格，主要用于预订时显示价格
        /// 注意：此方法返回的价格不包含结束日期的那一天，保持与预抵预离一致。如果需要结束日期那天的价格，请将日期加一天后再传
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="rateId">价格码</param>
        /// <param name="roomTypeId">房型id</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期，返回的价格不包含此日期</param>
        /// <returns>满足条件的明细价格信息</returns>
        List<RateDetail> GetRateDetail(string hid, string rateId, string roomTypeId, DateTime beginDate, DateTime endDate);
        List<RateDetail> GetRateDetailbyRateid(string hid,string rateid, string roomTypeid,string year,string month); 
        int AddRateDetail(string hid,  string rate, string cancelid, string guaranteeid,string ratecode,string roomtype); 
        string UpdateRateDetail(string begintime, string endtime, string strarr, string rateid, string roomtype, string hid);
        string UpdateRateDetail(string begintime, string endtime, string rateid, string roomtype,string cancelid,string cancelhours,string guaranteeid, string guaranteeTime, string hid,int isClose);
    }
}
