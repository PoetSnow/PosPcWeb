using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Linq;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    /// <summary>
    /// pos餐台类型服务实现
    /// </summary>
    public class PosTabServiceService : CRUDService<EntitiesPos.PosTabService>, IPosTabServiceService
    {
        private DbHotelPmsContext _db;

        public PosTabServiceService(DbHotelPmsContext db) : base(db, db.PosTabServices)
        {
            _db = db;
        }

        protected override EntitiesPos.PosTabService GetTById(string id)
        {
            return new EntitiesPos.PosTabService { Id = new Guid(id) };
        }

        /// <summary>
        /// 判断指定的代码或者名称的服务费政策是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="module">模块</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabTypeid">餐台类型id</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的服务费政策了，false：没有相同的</returns>
        public bool IsExists(string hid, string module, string refeid, string tabTypeid, string customerTypeid, byte? iTagperiod, string startTime, string endTime)
        {
            var list = _db.PosTabServices.Where(w => w.Hid == hid && (w.Module == module || w.Module == null) && (w.Refeid == refeid || w.Refeid == null) && (w.TabTypeid == tabTypeid || w.TabTypeid == null) && (w.CustomerTypeid == customerTypeid || w.CustomerTypeid == null) && w.ITagperiod == iTagperiod).ToList();
            if (list == null || list.Count == 0)
            {
                return false;
            }
            else
            {
                string today = DateTime.Today.ToString("yyyy-MM-dd");

                startTime = today + " " + startTime;
                endTime = today + " " + endTime;

                //结束时间时候是否为次日
                if (Convert.ToDateTime(startTime) >= Convert.ToDateTime(endTime))
                {
                    endTime = Convert.ToDateTime(endTime).AddDays(1).ToString();
                }

                //循环比较时间范围
                foreach (var temp in list)
                {
                    temp.StartTime = today + " " + temp.StartTime;
                    temp.EndTime = today + " " + temp.EndTime;

                    //结束时间时候是否为次日
                    if (Convert.ToDateTime(temp.StartTime) >= Convert.ToDateTime(temp.EndTime))
                    {
                        if (Convert.ToDateTime(temp.StartTime) >= Convert.ToDateTime(startTime) && Convert.ToDateTime(startTime) >= Convert.ToDateTime(endTime))
                        {
                            startTime = Convert.ToDateTime(startTime).AddDays(1).ToString();
                        }
                        else if (Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.StartTime) && Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.EndTime))
                        {
                            startTime = Convert.ToDateTime(startTime).AddDays(1).ToString();
                        }

                        temp.EndTime = Convert.ToDateTime(temp.EndTime).AddDays(1).ToString();
                    }

                    if (Convert.ToDateTime(temp.StartTime) <= Convert.ToDateTime(startTime) && Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.EndTime))
                    {
                        return true;
                    }
                    else if (Convert.ToDateTime(temp.StartTime) <= Convert.ToDateTime(endTime) && Convert.ToDateTime(endTime) <= Convert.ToDateTime(temp.EndTime))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 判断指定的代码或者名称的服务费政策是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="module">模块</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabTypeid">餐台类型id</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="exceptId">要排队的服务费政策id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的服务费政策了，false：没有相同的</returns>
        public bool IsExists(string hid, string module, string refeid, string tabTypeid, string customerTypeid, byte? iTagperiod, string startTime, string endTime, Guid? exceptId)
        {
            var list = _db.PosTabServices.Where(w => w.Hid == hid && (w.Module == module || w.Module == null) && (w.Refeid == refeid || w.Refeid == null) && (w.TabTypeid == tabTypeid || w.TabTypeid == null) && (w.CustomerTypeid == customerTypeid || w.CustomerTypeid == null) && w.ITagperiod == iTagperiod && w.Id != exceptId).ToList();
            if (list == null || list.Count == 0)
            {
                return false;
            }
            else
            {
                string today = DateTime.Today.ToString("yyyy-MM-dd");

                startTime = today + " " + startTime;
                endTime = today + " " + endTime;

                //结束时间时候是否为次日
                if (Convert.ToDateTime(startTime) >= Convert.ToDateTime(endTime))
                {
                    endTime = Convert.ToDateTime(endTime).AddDays(1).ToString();
                }

                //循环比较时间范围
                foreach (var temp in list)
                {
                    temp.StartTime = today + " " + temp.StartTime;
                    temp.EndTime = today + " " + temp.EndTime;

                    //结束时间时候是否为次日
                    if (Convert.ToDateTime(temp.StartTime) >= Convert.ToDateTime(temp.EndTime))
                    {
                        if (Convert.ToDateTime(temp.StartTime) >= Convert.ToDateTime(startTime) && Convert.ToDateTime(startTime) >= Convert.ToDateTime(endTime))
                        {
                            startTime = Convert.ToDateTime(startTime).AddDays(1).ToString();
                        }
                        else if (Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.StartTime) && Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.EndTime))
                        {
                            startTime = Convert.ToDateTime(startTime).AddDays(1).ToString();
                        }

                        temp.EndTime = Convert.ToDateTime(temp.EndTime).AddDays(1).ToString();
                    }

                    if (Convert.ToDateTime(temp.StartTime) <= Convert.ToDateTime(startTime) && Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.EndTime))
                    {
                        return true;
                    }
                    else if (Convert.ToDateTime(temp.StartTime) <= Convert.ToDateTime(endTime) && Convert.ToDateTime(endTime) <= Convert.ToDateTime(temp.EndTime))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 根据酒店、模块、营业点、餐台类型等获取服务费政策
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="module">模块</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabTypeid">餐台类型id</param>
        /// <param name="customerTypeid">客人类型id</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="openTime">开台时间</param>
        /// <returns></returns>
        public EntitiesPos.PosTabService GetPosTabService(string hid, string module, string refeid, string tabTypeid, string customerTypeid, byte? iTagperiod, DateTime openTime)
        {
            var list = _db.PosTabServices.Where(w => w.Hid == hid && (w.Module == module || w.Module == null) && (w.Refeid == refeid || w.Refeid == null) && (w.TabTypeid == tabTypeid || w.TabTypeid == null) && (w.CustomerTypeid == customerTypeid || w.CustomerTypeid == null) && w.ITagperiod == iTagperiod).ToList();
            if (list != null && list.Count > 0)
            {
                string today = DateTime.Today.ToString("yyyy-MM-dd");

                //循环比较时间范围
                foreach (var temp in list)
                {
                    temp.StartTime = today + " " + temp.StartTime;
                    temp.EndTime = today + " " + temp.EndTime;

                    //结束时间时候是否为次日
                    if (Convert.ToDateTime(temp.StartTime) >= Convert.ToDateTime(temp.EndTime))
                    {
                        temp.StartTime = Convert.ToDateTime(temp.StartTime).AddDays(-1).ToString();
                    }

                    if (Convert.ToDateTime(temp.StartTime) <= Convert.ToDateTime(openTime) && Convert.ToDateTime(openTime) <= Convert.ToDateTime(temp.EndTime))
                    {
                        return temp;
                    }
                }
            }

            return new EntitiesPos.PosTabService();
        }
    }
}