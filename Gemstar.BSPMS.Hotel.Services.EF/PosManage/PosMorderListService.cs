using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos扫码点餐记录
    /// </summary>
    public class PosMorderListService : CRUDService<PosMorderList>, IPosMorderListService
    {
        private DbHotelPmsContext _db;

        public PosMorderListService(DbHotelPmsContext db) : base(db, db.PosMorderLists)
        {
            _db = db;
        }

        protected override PosMorderList GetTById(string id)
        {
            return new PosMorderList { Id = Guid.Parse(id) };
        }

        /// <summary>
        /// 根据账单ID获取点菜记录
        /// </summary>
        /// <param name="hid">酒店代码</param>
        /// <param name="billId">账单ID</param>
        /// <returns></returns>
        public List<PosMorderList> GetPosMorderListByBillId(string hid, string billId)
        {
            return _db.PosMorderLists.Where(w => w.Hid == hid && w.Billid == billId && w.IStatus == (byte)PosMorderListIStatus.落单).ToList();

        }


        /// <summary>
        /// 根据微信ID与酒店ID 获取账单信息
        /// </summary>
        /// <param name="openId">微信ID</param>
        /// <param name="hid">酒店代码</param>
        /// <returns></returns>
        public PosMorderList GetPosMorderListByOpenId(string hid, string openId)
        {
            //  string date = DateTime.Now.ToString("d");
            var year = DateTime.Now.Year;
            var month = DateTime.Now.Month;
            var day = DateTime.Now.Day;


            return _db.Database.SqlQuery<PosMorderList>("select * from PosMorderList where hid=@hid and Wxid=@Wxid and IStatus=@IStatus and " +
                "convert(varchar,YEAR(createdate))+'-'+convert(varchar,Month(createdate))+'-'+convert(varchar,Day(createdate))=@date order by  createdate desc" +
                "", new SqlParameter("hid", hid)
                , new SqlParameter("Wxid", openId)
                , new SqlParameter("IStatus", (byte)PosMorderListIStatus.落单)
                , new SqlParameter("date", year + "-" + month + "-" + day)).FirstOrDefault();
            //string sql= "Select *  from PosMorderList where hid="
            //var list = _db.PosMorderLists.Where(w => w.Hid == hid && w.Wxid == openId && w.IStatus == (byte)PosMorderListIStatus.落单).OrderBy().ToList();
            //foreach (var item in collection)
            //{

            //}
        }
    }
}