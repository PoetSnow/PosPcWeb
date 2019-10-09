using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Tools;

namespace Gemstar.BSPMS.Common.Services.EF
{
    /// <summary>
    /// EF的DbContext类扩展方法
    /// </summary>
    public static class DbContextExtension
    {

        //通用方法
        //获取流水号
        private static string GetBaseNo(this DbContext context,string hid, string baseName, string dateStr = "", int no47 = 0, int len = 6)
        {
            return context.Database.SqlQuery<string>(
                "exec up_getBaseNo @hid=@hid,@as_baseName=@as_baseName,@as_other=@as_other,@no47=@no47,@len=@len"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@as_baseName", baseName)
                , new SqlParameter("@as_other", dateStr)
                , new SqlParameter("@no47", no47)
                , new SqlParameter("@len", len)
                ).Single();
        }
        /// <summary>
        /// 获取指定酒店的预订流水号
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>预订流水号</returns>
        public static string GetBaseNoForRes(this DbContext context, string hid)
        {
            return GetBaseNo(context,hid, "res", DateTime.Today.ToDateString(), len: 4);
        }
        /// <summary>
        /// 获取指定酒店的登记单号，取出后可以直接赋值给regid
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <returns>登记单号</returns>
        public static string GetBaseNoForRegId(this DbContext context, string hid)
        {
            var baseNo = GetBaseNo(context,hid, "regid", len: 0);
            return string.Format("{0}{1}", hid, baseNo);
        }
        /// <summary>
        /// 根据酒店id和预订流水号生成对应的订单唯一主键
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="resNo">预订流水号</param>
        /// <returns>对应的订单唯一主键</returns>
        public static string GetResIdFromHidAndResNo(string hid,string resNo)
        {
            return string.Format("{0}{1}", hid, resNo);
        }
        /// <summary>
        /// 判断指定的数据库实例是否是通过外网ip进行连接的
        /// </summary>
        /// <param name="dbContext">要判断的数据库实例</param>
        /// <returns>true:使用外网ip进行连接，false:使用内网ip进行连接</returns>
        public static bool IsConnectViaInternetIp(this DbContext dbContext)
        {
            try
            {
                var dataSource = dbContext.Database.Connection.DataSource.Trim();

                return ConnStrHelper.IsDatasourceInternet(dataSource);
            }
            catch
            {
                return false;
            }
        }
    }
}
