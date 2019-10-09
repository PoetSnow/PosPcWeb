using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF.MarketingManage;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MarketingManage;
using Gemstar.BSPMS.Hotel.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
namespace Gemstar.BSPMS.Hotel.Web.Areas.Weixin.Controllers
{
    /// <summary>
    /// 业主查询
    /// </summary>
    [NotAuth]
    public class RoomOwnerMonthCalcController : BaseWeixinController
    {
        // GET: MarketingManage/RoomOwnerMonthCalc 
        public ActionResult Index(string hid, string profileid, string openid)
        {
            ViewBag.hid = hid;
            ViewBag.profileid = profileid;
            ViewBag.openId = openid;
            var hotelDb = GetHotelDb(hid);
            PmsHotel hotel = hotelDb.PmsHotels.Where(w => w.Hid == hid).FirstOrDefault();
            ViewBag.hotelname = hotel.Name;

            var isShowOwnerRoomCalendar = hotelDb.PmsParas.Where(w => w.Hid == hid && w.Code == "isShowOwnerRoomCalendar").FirstOrDefault();
            if (isShowOwnerRoomCalendar != null && isShowOwnerRoomCalendar.Value=="1")
            {
                ViewBag.isShowOwnerRoomCalendar = true;
            }
            else
            {
                ViewBag.isShowOwnerRoomCalendar = false;
            }
            return View();
        }

        /// <summary>
        /// 月度分成
        /// </summary>
        /// <param name="dtime"></param>
        /// <returns></returns> 
        public ActionResult MonthCalc(string hid, string profileid, DateTime? dtime = null)
        {
            // var queryService = GetService<ICommonQueryService>();
            Dictionary<string, string> s = new Dictionary<string, string>();
            s.Add("@h99hid", hid);
            if (dtime == null)
            { dtime = DateTime.Parse("1920-01-01"); }
            s.Add("@年月", dtime.ToString());
            s.Add("@profileid", profileid);
            var hotelDb = GetHotelDb(hid);
            CommonQueryService cs = new CommonQueryService(hotelDb);
            DataTable dt = cs.ExecuteQuery("up_list_RoomOwnerMonthCalc", s);
            ViewBag.monthcalc = dt;
            List<string> sr = new List<string>();
            foreach (DataRow r in dt.Rows)
            {
                if (!sr.Contains(r["roomno"]))
                {
                    sr.Add(r["roomno"].ToString());
                }
            }
            ViewBag.roomnos = sr;
            ViewBag.dtime = dtime;
            return PartialView("_MonthCalc");
        }
        /// <summary>
        /// 房租情况
        /// </summary>
        /// <param name="dtime"></param>
        /// <returns></returns> 
        public ActionResult RentSituation(string hid, string profileid, DateTime? dtime = null)
        {
            if (dtime == null)
            { dtime = DateTime.Parse("1920-01-01"); }
            var hotelDb = GetHotelDb(hid);
            RoomOwnerCalcResultService cs = new RoomOwnerCalcResultService(hotelDb);
            List<RentSituation> rs = cs.getRoomOwnerRentSituat(hid, dtime, profileid);
            List<string> sr = new List<string>();
            foreach (var r in rs)
            {
                if (!sr.Contains(r.Roomno))
                {
                    sr.Add(r.Roomno);
                }
            }
            ViewBag.roomnos = sr;
            return Json(JsonResultData.Successed(rs), JsonRequestBehavior.AllowGet);
        }
    }
}