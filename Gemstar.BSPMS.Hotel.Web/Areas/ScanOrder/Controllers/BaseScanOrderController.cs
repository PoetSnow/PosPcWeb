using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.AuthManages;
using Gemstar.BSPMS.Hotel.Services.EF;
using Gemstar.BSPMS.Hotel.Services.EF.PosManage;
using Gemstar.BSPMS.Hotel.Services.EF.SystemManage;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models;
using Newtonsoft.Json;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Controllers
{
    public class BaseScanOrderController : Controller
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        protected static string BaseHid { get; set; }

        /// <summary>
        /// 餐台ID
        /// </summary>
        protected static string BaseTabid { get; set; }

        /// <summary>
        /// 餐台ID
        /// </summary>
        protected static string BaseOpenid { get; set; }

        /// <summary>
        /// 用于判断是否清空餐台
        /// </summary>
        public static string OpenFlag { get; set; }

        /// <summary>
        /// 用于设置掩码
        /// </summary>
        protected ICurrentInfo currentInfo { get; set; }
        #region 获取服务接口

        /// <summary>
        /// 获取指定服务接口的实例
        /// </summary>
        /// <typeparam name="T">服务接口类型</typeparam>
        /// <returns>指定服务接口的实例</returns>
        protected T GetService<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

        /// <summary>
        /// 获取中央数据库实例
        /// </summary>
        /// <returns>中央数据库实例</returns>
        protected DbCommonContext GetCenterDb()
        {
            return GetService<DbCommonContext>();
        }

        /// <summary>
        /// 获取指定酒店id对应的营业数据库实例
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>酒店id对应的营业数据库实例</returns>
        protected DbHotelPmsContext GetHotelDb(string hid)
        {
            var hotelInfoService = GetService<IHotelInfoService>();
            var hotelInfo = hotelInfoService.GetHotelInfo(hid);
            var isConnectViaInternet = hotelInfoService.IsConnectViaInternte();
            var connStr = ConnStrHelper.GetConnStr(hotelInfo.DbServer, hotelInfo.DbName, hotelInfo.Logid, hotelInfo.LogPwd, "GemstarBSPMS",hotelInfo.DbServerInternet,isConnectViaInternet);
            var hotelDb = new DbHotelPmsContext(connStr, hid, "", Request);
            return hotelDb;
        }

        /// <summary>
        /// 设置基础信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="tabid">餐台ID</param>
        /// <param name="openid">Openid</param>
        public static void SetBaseInfo(string hid = "", string tabid = "", string openid = "")
        {
            if (!string.IsNullOrWhiteSpace(hid) && (string.IsNullOrWhiteSpace(BaseHid) || BaseHid != hid))
            {
                BaseHid = hid;
            }
            if (!string.IsNullOrWhiteSpace(tabid) && (string.IsNullOrWhiteSpace(BaseTabid) || BaseTabid != tabid))
            {
                BaseTabid = tabid;
            }
            if (!string.IsNullOrWhiteSpace(openid) && (string.IsNullOrWhiteSpace(BaseOpenid) || BaseOpenid != openid))
            {
                BaseOpenid = openid;
            }


        }

        #endregion 获取服务接口

        #region 操作记录

        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="type">操作类型</param>
        /// <param name="text">操作内容</param>
        /// <param name="keys">关键字</param>
        public void AddOperationLog(OpLogType type, string text, string keys = "")
        {

            OperationLogService oplog = new OperationLogService(GetHotelDb(BaseHid), currentInfo);
            oplog.AddOperationLog(BaseHid, type, text, string.IsNullOrEmpty(BaseOpenid) ? "扫码点餐" : BaseOpenid, Common.Extensions.UrlHelperExtension.GetRemoteClientIPAddress(), keys);
            //  GetService<IoperationLog>().AddOperationLog(BaseHid, type, text, BaseOpenid, Common.Extensions.UrlHelperExtension.GetRemoteClientIPAddress(), keys);
        }

        #endregion 操作记录


        #region 读取消费项目对应作法

        /// <summary>
        /// 根据消费项目生产消费项目明细数据以及计算作法加价金额
        /// </summary>
        /// <param name="db">数据库</param>
        /// <param name="itemId">消费项目ID</param>
        /// <param name="quantity">数量</param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        public List<ScanPosBillDetailAction> billDetailActionList(DbHotelPmsContext db, string itemId, decimal? quantity, int orderId, out decimal? amount)
        {
            var billDetailActions = new List<ScanPosBillDetailAction>();//定义一个新的对象

            var itemActionService = new PosItemActionService(db);

            var actionService = new PosActionService(db);

            var itemService = new PosItemService(db);
            var item = itemService.GetEntity(BaseHid, itemId);  //消费项目

            var actionByItemidResults = itemActionService.GetPosItemActionListByItemId(BaseHid, itemId);

            amount = 0.00M;
            if (actionByItemidResults != null && actionByItemidResults.Count > 0)
            {
                //是否
                foreach (var temp in actionByItemidResults.Where(m => m.isNeed == true))
                {
                    var action = actionService.Get(temp.actionid);
                    var actionByItemidResult = actionByItemidResults.Where(w => w.actionid == temp.actionid).FirstOrDefault();
                    if (actionByItemidResult != null)
                    {
                        #region 账单明细对应作法赋值

                        var billDetailAction = new ScanPosBillDetailAction
                        {
                            Hid = BaseHid,
                            ActionName = temp.ActionName,
                            ActionNo = temp.ActionCode,
                            AddPrice = temp.addPrice,

                            IByGuest = temp.isByGuest,
                            IByQuan = temp.isByQuan,
                            Igroupid = 1,
                            Memo = "扫码点餐",
                            ModiDate = DateTime.Now,
                            ModiUser = "扫码点餐",
                            Nmultiple = temp.multiple,
                            PrtNo = temp.prodPrinter,

                            DeptClassid = item.DeptClassid,
                            Quan = quantity,
                            OrderId = orderId,
                            LimitQuan = temp.limitQuan
                        };

                        #endregion 账单明细对应作法赋值

                        #region 作法加价

                        var addAmount = 0.0M;//作法加价的金额
                        //获取消费项目对应作法中的资料重算金额
                        if (temp.addPrice != null && temp.addPrice > 0)
                        {
                            if (temp.isByQuan != null && temp.isByQuan.Value && temp.isByGuest != null && temp.isByGuest.Value)
                            {
                                //数量相关最低数量大于点菜数量。作法加价不乘数量
                                if (temp.limitQuan != null && temp.limitQuan > 0 && quantity < temp.limitQuan)
                                {
                                    addAmount = Convert.ToDecimal(temp.addPrice) * 1;

                                }
                                else
                                {
                                    addAmount = Convert.ToDecimal(temp.addPrice) * Convert.ToDecimal(quantity) * 1;
                                }

                            }
                            else if (temp.isByQuan != null && temp.isByQuan.Value)
                            {
                                //数量相关最低数量大于点菜数量。作法加价不乘数量
                                if (temp.limitQuan != null && temp.limitQuan > 0 && quantity < temp.limitQuan)
                                {
                                    addAmount = 0;
                                }
                                else
                                {
                                    addAmount = Convert.ToDecimal(temp.addPrice) * Convert.ToDecimal(quantity);
                                }
                            }
                            else if (temp.isByGuest != null && temp.isByGuest.Value)
                            {

                                addAmount = Convert.ToDecimal(temp.addPrice) * 1;
                            }
                            else
                            {
                                addAmount = Convert.ToDecimal(temp.addPrice);
                            }
                        }

                        #endregion 作法加价

                        billDetailAction.Amount = addAmount;
                        amount += billDetailAction.Amount;

                        billDetailActions.Add(billDetailAction);
                    }
                }
            }

            return billDetailActions;
        }
        #endregion


        #region 对象赋值


        /// <summary>
        /// 模型赋值
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="source">数据源</param>
        public static void CopyModel(object target, object source)
        {
            Type type1 = target.GetType();
            Type type2 = source.GetType();
            foreach (var mi in type2.GetProperties())
            {
                var des = type1.GetProperty(mi.Name);
                if (des != null)
                {
                    try
                    {
                        des.SetValue(target, mi.GetValue(source, null), null);
                    }
                    catch
                    { }
                }
            }

        }
        #endregion

        /// <summary>
        /// 判断字符串是否为数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool CheckLetter(string str)
        {
            try
            {
                int.Parse(str);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

      
    }
}