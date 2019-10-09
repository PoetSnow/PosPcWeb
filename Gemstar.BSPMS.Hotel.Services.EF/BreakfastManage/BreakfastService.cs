using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.BreakfastManage;
using Gemstar.BSPMS.Common.Tools;
using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Services.NotifyManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.BreakfastManage
{
    /// <summary>
    /// 电子早餐服务的ef实现类
    /// </summary>
    public class BreakfastService: IBreakfastService
    {
        private DbHotelPmsContext _pmsContext;

        public BreakfastService(DbHotelPmsContext pmsContext)
        {
            _pmsContext = pmsContext;
        }

        /// <summary>
        /// 获取今天的早餐记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <returns></returns>
        public List<ResFolioBreakfastInfo> Today(string hid)
        {
            DateTime nowDate = DateTime.Now.Date;
            DateTime beginDate = nowDate;
            DateTime endDate = nowDate.AddDays(1).AddSeconds(-1);
            return _pmsContext.ResFolioBreakfastInfos.Where(c => c.Hid == hid && c.CDate >= beginDate && c.CDate <= endDate).OrderByDescending(c => c.CDate).AsNoTracking().ToList();
        }

        /// <summary>
        /// 刷卡吃早餐
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardNoOrRoomNo">门锁卡卡号</param>
        /// <returns></returns>
        public JsonResultData ToHaveBreakfastByCardNo(string hid, string cardNo)
        {
            return ToHaveBreakfast(hid, 1, cardNo);
        }

        /// <summary>
        /// 刷卡吃早餐
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="cardNoOrRoomNo">房号</param>
        /// <returns></returns>
        public JsonResultData ToHaveBreakfastByRoomNo(string hid, string roomNo)
        {
            return ToHaveBreakfast(hid, 2, roomNo);
        }

        /// <summary>
        /// 刷卡吃早餐
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="type">类型（1卡号，2房号）</param>
        /// <param name="cardNoOrRoomNo">门锁卡卡号 或 房号</param>
        /// <returns></returns>
        private JsonResultData ToHaveBreakfast(string hid, int type, string cardNoOrRoomNo)
        {
            //验证 参数
            if (string.IsNullOrWhiteSpace(hid))
            {
                return JsonResultData.Failure("酒店ID不能为空！");
            }
            string cardNo = "";
            string roomNo = "";
            if(type == 1)
            {
                cardNo = cardNoOrRoomNo;
            }
            else if(type == 2)
            {
                roomNo = cardNoOrRoomNo;
            }
            else
            {
                return JsonResultData.Failure("类型错误（1卡号，2房号）！");
            }
            if (string.IsNullOrWhiteSpace(cardNoOrRoomNo))
            {
                return JsonResultData.Failure((type == 1 ? "卡号" : "房号") + "不能为空！");
            }
            try
            {
                var procResults = _pmsContext.Database.SqlQuery<ResFolioBreakfastInfo>("exec up_resFolio_toHaveBreakfast @hid=@hid,@cardNo=@cardNo,@roomNo=@roomNo", new SqlParameter("@hid", hid), new SqlParameter("@cardNo", cardNo), new SqlParameter("@roomNo", roomNo)).ToList();
                return JsonResultData.Successed(procResults);
            }
            catch(Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }


    }
}
