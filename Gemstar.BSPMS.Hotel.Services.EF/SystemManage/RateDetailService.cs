using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF
{
    public class RateDetailService : CRUDService<RateDetail>, IRateDetailService
    {
        public RateDetailService(DbHotelPmsContext db) : base(db, db.RateDetails)
        {
            _pmsContext = db;
        }

        protected override RateDetail GetTById(string id)
        {
            return new RateDetail { Id = Guid.Parse(id) };
        }

        /// <summary>
        /// 获取价格明细信息
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public List<RateDetail> GetRateDetail(string hid)
        {
            string datetime = DateTime.Now.ToString("yyyy-MM-dd");
            string sql = "select a.hid,a.rateid,a.id,a.roomTypeid,a.rateDate,rate,cancelid = b.name + case when b.code = '03' then  isnull(convert(varchar(10) ,a.cancelHours ),'')  else  ''  end,a.cancelHours,guaranteeid = c.name + case when b.code = '03' then isnull(convert(varchar(10), a.guaranteeTime), '')  else  ''  end,a.guaranteeTime,a.isClose from ratedetail a left join v_codeListPub b on a.cancelid = b.code and b.typeCode = '14' left join v_codeListPub c on a.guaranteeid = c.code and c.typeCode = '13' where hid = {0} and RateDate={1} ";
            var list = _pmsContext.Database.SqlQuery<RateDetail>(sql, hid, datetime).ToList();
            return list;
        }

        /// 获取指定时间段，指定房型，指定价格码的明细价格，主要用于预订时显示价格
        /// 注意：此方法返回的价格不包含结束日期的那一天，保持与预抵预离一致。如果需要结束日期那天的价格，请将日期加一天后再传
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="rateId">价格码</param>
        /// <param name="roomTypeId">房型id</param>
        /// <param name="beginDate">开始日期</param>
        /// <param name="endDate">结束日期，返回的价格不包含此日期</param>
        /// <returns>满足条件的明细价格信息</returns>
        public List<RateDetail> GetRateDetail(string hid, string rateId, string roomTypeId, DateTime beginDate, DateTime endDate)
        {
            return _pmsContext.RateDetails.Where(w => w.Hid == hid && w.Rateid == rateId && w.RoomTypeid == roomTypeId && w.RateDate >= beginDate && w.RateDate < endDate).ToList();
        }

        public List<RateDetail> GetRateDetailbyRateid(string hid, string rateid, string roomtypeid, string year, string month)
        {
            string sql = "select a.hid,a.rateid,a.id,a.roomTypeid,a.rateDate,rate,cancelid = b.name ,a.cancelHours,guaranteeid = c.name ,a.guaranteeTime,a.isClose from ratedetail a left join v_codeListPub b on a.cancelid = b.code and b.typeCode = '14' left join v_codeListPub c on a.guaranteeid = c.code and c.typeCode = '13' where hid = {0} and rateid={1} and roomtypeid={2} and Year(rateDate) = {3} and month(rateDate)={4}";
            var list = _pmsContext.Database.SqlQuery<RateDetail>(sql, hid, rateid, roomtypeid, year, month).ToList();
            return list;
        }
        /// <summary>
        /// 没使用的方法
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="rate"></param>
        /// <param name="cancelid"></param>
        /// <param name="guaranteeid"></param>
        /// <param name="ratecode"></param>
        /// <param name="roomtype"></param>
        /// <returns></returns>
        public int AddRateDetail(string hid, string rate, string cancelid, string guaranteeid, string ratecode, string roomtype)
        {
            string datetime = DateTime.Now.ToShortDateString();
            return _pmsContext.Database.ExecuteSqlCommand("exec up_add_rateDetail @rate={0},@cancelid={1},@guaranteeid={2},@hid={3},@rateDate={4},@rateid={5},@roomtypeid={6}", rate, cancelid, guaranteeid, hid, datetime, ratecode, roomtype);
        }

        /// <summary>
        /// 修改价格明细
        /// </summary>
        /// <param name="begintime"></param>
        /// <param name="endtime"></param>
        /// <param name="strarr">周一到周日的价格</param>
        /// <param name="rateid">价格码id</param>
        /// <param name="roomtype">房型</param>
        /// <param name="hid"></param>
        /// <returns></returns>
        public string UpdateRateDetail(string begintime, string endtime, string strarr, string rateid, string roomtype, string hid)
        {
            string[] arr = strarr.Trim(',').Split(','); int a = 0;
            try
            {
                a = _pmsContext.Database.ExecuteSqlCommand("exec up_setRateDetail @hid={0},@rateid={1},@begindate={2},@endDate={3},@roomtypeid={4},@q1={5},@q2={6},@q3={7},@q4={8},@q5={9},@q6={10},@q7={11}", hid, rateid, DateTime.Parse(begintime), DateTime.Parse(endtime), roomtype, arr[0], arr[1], arr[2], arr[3], arr[4], arr[5], arr[6]);
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }
            updateRefCodeRate(hid, rateid);
            return a > 0 ? "成功" : "失败";
        }

        /// <summary>
        /// 修改政策信息
        /// </summary>
        /// <param name="begintime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        /// <param name="rateid">价格码id</param>
        /// <param name="roomtype">房型</param>
        /// <param name="cancelid">取消政策id</param>
        /// <param name="cancelhours">取消小时数</param>
        /// <param name="guaranteeid">担保政策id</param>
        /// <param name="guaranteeTime">分时时间</param>
        /// <param name="hid"></param>
        /// <returns></returns>
        public string UpdateRateDetail(string begintime, string endtime, string rateid, string roomtype, string cancelid, string cancelhours, string guaranteeid, string guaranteeTime, string hid, int isClose)
        {
            int a = _pmsContext.Database.ExecuteSqlCommand("exec up_setRateDetail @hid={0},@rateid={1},@begindate={2},@endDate={3},@roomtypeid={4},@cancelid={5},@cancelhours={6},@guaranteeid={7},@guaranteeTime={8},@isClose={9}", hid, rateid, DateTime.Parse(begintime), DateTime.Parse(endtime), roomtype, cancelid, (cancelhours == "" ? "0" : cancelhours), guaranteeid, guaranteeTime, isClose);
            updateRefCodeRate(hid, rateid);
            return a > 0 ? "成功" : "失败";
        }
        /// <summary>
        /// 同步引用此价格的价格代码的价格明细
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="rateid"></param>
        public void updateRefCodeRate(string hid, string rateid)
        {
            var list = _pmsContext.Rates.Where(w => w.RefRateid == rateid && w.PriceMode == "2").ToList();
            for (int i = 0; i < list.Count(); i++)
            {
                _pmsContext.Database.ExecuteSqlCommand("exec up_updateRateToRefcode @hid={0},@ratecode={1},@refratecode={2},@addMode={3},@addamount={4}", hid, list[i].Id, rateid, list[i].AddMode, (list[i].AddMode == true ? list[i].AddRate : list[i].AddAmount) == null ? 0 : (list[i].AddMode == true ? list[i].AddRate : list[i].AddAmount));
            }

        }

        private DbHotelPmsContext _pmsContext;

    }
}