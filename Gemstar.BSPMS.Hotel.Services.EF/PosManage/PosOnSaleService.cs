using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class PosOnSaleService : CRUDService<PosOnSale>, IPosOnSaleService
    {
        private DbHotelPmsContext _db;
        public PosOnSaleService(DbHotelPmsContext db) : base(db, db.PosOnSales)
        {
            _db = db;
        }

        protected override PosOnSale GetTById(string id)
        {
            return new PosOnSale { Id = new Guid(id) };
        }

        /// <summary>
        /// 判断指定的代码或者名称的服务费政策是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="module">模块</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabTypeid">餐台类型id</param>
        /// <param name="customerTypeid">日期类型</param>
        /// <param name="itemid">消费项目id</param>
        /// <param name="unitid">单位id</param>
        /// <param name="iCmpType">收费状态</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的服务费政策了，false：没有相同的</returns>
        public bool IsExists(string hid, string module, string refeid, string tabTypeid, string customerTypeid, string itemid, string unitid, byte? iTagperiod, string startTime, string endTime)
        {
            var list = _db.PosOnSales.Where(w => w.Hid == hid && w.Module == module && (w.Refeid == refeid || string.IsNullOrEmpty(w.Refeid) || string.IsNullOrEmpty(refeid)) && (w.TabTypeid == tabTypeid || string.IsNullOrEmpty(w.TabTypeid) || string.IsNullOrEmpty(tabTypeid)) && (w.CustomerTypeid == customerTypeid || string.IsNullOrEmpty(w.CustomerTypeid) || string.IsNullOrEmpty(customerTypeid)) && w.Itemid == itemid && w.Unitid == unitid && w.ITagperiod == iTagperiod).ToList();
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
                    else if (Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.StartTime) && Convert.ToDateTime(endTime) >= Convert.ToDateTime(temp.EndTime))
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
        /// <param name="customerTypeid">日期类型</param>
        /// <param name="itemid">消费项目id</param>
        /// <param name="unitid">单位id</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="exceptId">要排队的服务费政策id，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的服务费政策了，false：没有相同的</returns>
        public bool IsExists(string hid, string module, string refeid, string tabTypeid, string customerTypeid, string itemid, string unitid, byte? iTagperiod, string startTime, string endTime, Guid? exceptId)
        {
            var list = _db.PosOnSales.Where(w => w.Hid == hid && w.Module == module && (w.Refeid == refeid || string.IsNullOrEmpty(w.Refeid) || string.IsNullOrEmpty(refeid)) && (w.TabTypeid == tabTypeid || string.IsNullOrEmpty(w.TabTypeid) || string.IsNullOrEmpty(tabTypeid)) && (w.CustomerTypeid == customerTypeid || string.IsNullOrEmpty(w.CustomerTypeid) || string.IsNullOrEmpty(customerTypeid)) && w.ITagperiod == iTagperiod && w.Id != exceptId && w.Unitid == unitid && w.Itemid == itemid).ToList();
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
                    else if (Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.StartTime) && Convert.ToDateTime(endTime) >= Convert.ToDateTime(temp.EndTime))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// 特价菜列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="unitId">单位ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="tabTypeId">餐台类型ID</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <returns></returns>
        public List<PosOnSale> GetPosOnSaleList(string hid, string module, string itemId, string unitId, string refeId, string tabTypeId, byte? iTagperiod)
        {
            return _db.PosOnSales.Where(w => w.Hid == hid && w.Module == module && w.Itemid == itemId && w.Unitid == unitId && w.Refeid == refeId && w.TabTypeid == tabTypeId && w.ITagperiod == iTagperiod).ToList();
        }



        /// <summary>
        /// 特价菜消费项目列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="itagperiod">日期类型</param>
        /// <param name="customerTypeid">客人类型ID</param>
        /// <param name="tabTypeid">餐台类型ID</param>
        /// <returns></returns>

        public List<up_pos_list_itemByPosOnSaleResult> GetItemListByPosOnSale(string hid, string refeId, string itagperiod, string customerTypeid, string tabTypeid, int pageIndex, int pageSize)
        {

            var list = _db.Database.SqlQuery<up_pos_list_itemByPosOnSaleResult>("exec up_pos_list_itemByPosOnSale @hid=@hid,@refeId=@refeId,@itagperiod=@itagperiod,@customerTypeid=@customerTypeid,@tabTypeid=@tabTypeid",
                    new SqlParameter("@hid", hid),
                    new SqlParameter("@refeId", refeId),
                    new SqlParameter("@itagperiod", itagperiod),
                    new SqlParameter("@customerTypeid", customerTypeid),
                    new SqlParameter("@tabTypeid", tabTypeid)).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return list;

        }

        /// <summary>
        /// 获取特价菜数量
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="refeId"></param>
        /// <param name="itagperiod"></param>
        /// <param name="customerTypeid"></param>
        /// <param name="tabTypeid"></param>
        /// <returns></returns>
        public int GetPosOnSaleTotal(string hid, string refeId, string itagperiod, string customerTypeid, string tabTypeid)
        {
            var result = _db.Database.SqlQuery<up_pos_list_itemByPosOnSaleResult>("exec up_pos_list_itemByPosOnSale @hid=@hid,@refeId=@refeId,@itagperiod=@itagperiod,@customerTypeid=@customerTypeid,@tabTypeid=@tabTypeid",
                new SqlParameter("@hid", hid),
                new SqlParameter("@refeId", refeId),
                new SqlParameter("@itagperiod", itagperiod),
                new SqlParameter("@customerTypeid", customerTypeid),
                new SqlParameter("@tabTypeid", tabTypeid)).Count();
            return result;
        }


        /// <summary>
        /// 获取消费项目单位列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="itagperiod">特价菜日期类型</param>
        /// <param name="customerTypeid">客人类型</param>
        /// <param name="tabTypeid">餐台类型ID</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<up_pos_list_ItemPriceByItemidResult> GetUnitByPosOnSaleItem(string hid, string itemId, string refeId, string itagperiod, string customerTypeid, string tabTypeid, int pageIndex, int pageSize)
        {
            var result = _db.Database.SqlQuery<up_pos_list_ItemPriceByItemidResult>("exec up_pos_list_unitByPosOnSale @hid=@hid,@refeId=@refeId,@itagperiod=@itagperiod,@customerTypeid=@customerTypeid,@tabTypeid=@tabTypeid,@itemID=@itemID",
               new SqlParameter("@hid", hid),
                new SqlParameter("@itemID", itemId),
               new SqlParameter("@refeId", refeId),
               new SqlParameter("@itagperiod", itagperiod),
               new SqlParameter("@customerTypeid", customerTypeid),
               new SqlParameter("@tabTypeid", tabTypeid)).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return result;
        }

        /// <summary>
        /// 特价菜单位数量
        /// </summary>
        /// <param name="hid"></param>
        /// <param name="itemId"></param>
        /// <param name="refeId"></param>
        /// <param name="itagperiod"></param>
        /// <param name="customerTypeid"></param>
        /// <param name="tabTypeid"></param>
        /// <returns></returns>
        public int GetUnitByPosOnSaleItemTotal(string hid, string itemId, string refeId, string itagperiod, string customerTypeid, string tabTypeid)
        {
            var result = _db.Database.SqlQuery<up_pos_list_ItemPriceByItemidResult>("exec up_pos_list_unitByPosOnSale @hid=@hid,@refeId=@refeId,@itagperiod=@itagperiod,@customerTypeid=@customerTypeid,@tabTypeid=@tabTypeid,@itemID=@itemID",
              new SqlParameter("@hid", hid),
               new SqlParameter("@itemID", itemId),
              new SqlParameter("@refeId", refeId),
              new SqlParameter("@itagperiod", itagperiod),
              new SqlParameter("@customerTypeid", customerTypeid),
              new SqlParameter("@tabTypeid", tabTypeid)).Count();
            return result;
        }

        /// <summary>
        /// 根据条件获取特价菜
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="refeId">营业点ID</param>
        /// <param name="itagperiod">日期类型</param>
        /// <param name="customerTypeid">客人类型</param>
        /// <param name="tabTypeid">餐台类型</param>
        /// <param name="unitId">单位ID</param>
        /// <returns></returns>
        public PosOnSale GetPosOnSaleByItemId(string hid, string itemId, string refeId, string itagperiod, string customerTypeid, string tabTypeid, string unitId)
        {
            //  var list= _db.PosOnSales.Where(m=>m.Hid==hid&&m.Refeid==refeId)
            byte? itagperiod1 = Convert.ToByte(itagperiod);
            var list = _db.PosOnSales.Where(w => w.Hid == hid && w.Module == "CY" && (w.Refeid == refeId || string.IsNullOrEmpty(w.Refeid) || string.IsNullOrEmpty(refeId)) && (w.TabTypeid == tabTypeid || string.IsNullOrEmpty(w.TabTypeid) || string.IsNullOrEmpty(tabTypeid)) && (w.CustomerTypeid == customerTypeid || string.IsNullOrEmpty(w.CustomerTypeid) || string.IsNullOrEmpty(customerTypeid)) && w.ITagperiod == itagperiod1 && w.Unitid == unitId).ToList();
            if (list != null && list.Count > 0)
            {
                string today = DateTime.Today.ToString("yyyy-MM-dd");

                var startTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                var endTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

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
                        return temp;

                    }
                    else if (Convert.ToDateTime(temp.StartTime) <= Convert.ToDateTime(endTime) && Convert.ToDateTime(endTime) <= Convert.ToDateTime(temp.EndTime))
                    {
                        return temp;
                    }
                }
            }
            return new PosOnSale();
        }


        /// <summary>
        /// 批量操作页面 根据条件获取特价菜列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="itemname">消费项目名称</param>
        /// <param name="unitname">单位名称</param>
        /// <param name="refename">营业点名称</param>
        /// <param name="tabname">餐台类型</param>
        /// <param name="customerid">客户ID</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="CmpType">计算类型</param>
        /// <param name="isUsed">是否启用</param>
        /// <returns></returns>
        public List<up_pos_list_BatchHandlePosOnSaleResult> GetBatchHandlePosOnSale(string hid,string itemname, string unitid, string refeid, string tabid, string customerid, string iTagperiod, Int16? CmpType, int? isUsed)
        {
            var sqlpara = new List<SqlParameter>()
            {
              new SqlParameter("@hid", hid),
              new SqlParameter("@itemname", itemname),
              new SqlParameter("@unitid", unitid),
              new SqlParameter("@refeid", refeid),
              new SqlParameter("@tabid", tabid),
              new SqlParameter("@customerid", customerid),
              new SqlParameter("@iTagperiod", iTagperiod)
            };

            if (CmpType == null)
                sqlpara.Add(new SqlParameter("@CmpType", DBNull.Value));
            else
                sqlpara.Add(new SqlParameter("@CmpType", CmpType));

            if (isUsed == null)
                sqlpara.Add(new SqlParameter("@isUsed", DBNull.Value));
            else
                sqlpara.Add(new SqlParameter("@isUsed", isUsed));


            var result = _db.Database.SqlQuery<up_pos_list_BatchHandlePosOnSaleResult>("exec up_pos_list_BatchHandlePosOnSale @h99hid=@hid,@itemname=@itemname,@unitid=@unitid,@refeId=@refeId,@iTagperiod=@iTagperiod,@customerid=@customerid,@tabid=@tabid,@CmpType=@CmpType,@isUsed=@isUsed", sqlpara.ToArray()).ToList();

            return result;
        }



        /// <summary>
        /// 根据酒店ID 和lambda表达式获取特价菜列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="func">lambda表达式</param>
        /// <returns></returns>
        public List<PosOnSale> GetPosOnSales(string hid, Func<PosOnSale, bool> func)
        {
            return _db.PosOnSales.Where(u => u.Hid==hid).Where(func).ToList();
        }


        /// <summary>
        /// 判断指定的代码或者名称的服务费政策是否已经存在，用于防止代码和名称重复
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="module">模块</param>
        /// <param name="refeid">营业点id</param>
        /// <param name="tabTypeid">餐台类型id</param>
        /// <param name="customerTypeid">日期类型</param>
        /// <param name="itemid">消费项目id</param>
        /// <param name="unitid">单位id</param>
        /// <param name="iTagperiod">日期类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="exceptIdlist">要排队的id集合，以避免与自己的代码和名称重复</param>
        /// <returns>true:在酒店中已经有相同代码或者相同名称的了，false：没有相同的</returns>
        public bool IsExists(string hid, string module, string refeid, string tabTypeid, string customerTypeid, string itemid, string unitid, byte? iTagperiod, string startTime, string endTime, List<Guid> exceptIdlist)
        {
            var list = _db.PosOnSales.Where(w => w.Hid == hid && w.Module == module && (w.Refeid == refeid || string.IsNullOrEmpty(w.Refeid) || string.IsNullOrEmpty(refeid)) && (w.TabTypeid == tabTypeid || string.IsNullOrEmpty(w.TabTypeid) || string.IsNullOrEmpty(tabTypeid)) && (w.CustomerTypeid == customerTypeid || string.IsNullOrEmpty(w.CustomerTypeid) || string.IsNullOrEmpty(customerTypeid)) && w.ITagperiod == iTagperiod && !exceptIdlist.Contains(w.Id)&& w.Unitid == unitid && w.Itemid == itemid).ToList();
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
                    else if (Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.StartTime) && Convert.ToDateTime(endTime) >= Convert.ToDateTime(temp.EndTime))
                    {
                        return true;
                    }
                }
            }

            return false;

        }

    }
}
