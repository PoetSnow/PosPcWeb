using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Hotel.Services;
using Gemstar.BSPMS.Hotel.Services.EF;
using Kendo.Mvc.UI.Fluent;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Web.Models
{
    public class GridColumnsManager
    {
        /// <summary>
        /// 获取指定列表界面的用户自定义列设置，如果用户没有自定义设置，则返回默认列设置信息
        /// </summary>
        /// <param name="area">area</param>
        /// <param name="controller">controller</param>
        /// <param name="action">action</param>
        /// <param name="defaultSettings">默认列设置信息</param>
        /// <returns>用户自定义设置信息，如果用户没有自定义，则返回默认列设置信息</returns>
        public static List<GridColumnSetting> GetGridColumnSettings(string area, string controller, string action, List<GridColumnSetting> defaultSettings)
        {
            var db = DependencyResolver.Current.GetService<DbHotelPmsContext>();
            var currentInfo = DependencyResolver.Current.GetService<ICurrentInfo>();
            var hid = currentInfo.HotelId;
            var setting = db.GridColumnsSettings.SingleOrDefault(w => w.Hid == hid && w.Area == area && w.Controller == controller && w.Action == action);

            if (setting != null && !string.IsNullOrWhiteSpace(setting.ColumnSettings))
            {
                var series = new JavaScriptSerializer();
                var columns = series.Deserialize<List<GridColumnSetting>>(setting.ColumnSettings);
                return columns;
            }
            return defaultSettings;
        }
        /// <summary>
        /// 将列设置信息绑定到grid控件中
        /// </summary>
        /// <param name="cols">grid控件的列构造器</param>
        /// <param name="columns">自定义列设置信息</param>
        public static void BoundColumns(GridColumnFactory<DataRowView> cols, List<GridColumnSetting> columns)
        {
            var i = 0;
            foreach (var column in columns)
            {

                var col = cols.Bound(column.Name).Title(column.Title).Hidden(column.Hidden);
                if (column.Width > 0)
                {
                    col = col.Width(column.Width);
                }
                if (!string.IsNullOrEmpty(column.format))
                {
                    col = col.Format(column.format);
                }
                if (!string.IsNullOrEmpty(column.ClientFooterTemplate))
                {
                    col.ClientFooterTemplate(column.ClientFooterTemplate);
                }
                //2018年3月30日15:45:45 tanjian 添加html属性
                if (column.HtmlAttributes != null)
                {
                    col.HtmlAttributes(column.HtmlAttributes);
                }
                if (i == 0)
                {
                    col.ClientTemplate("<span class='selectRowBorder'></span>#: " + column.Name + " #");
                    col.HtmlAttributes(new { style = "white-space: nowrap; position: relative" });
                }
                i++;
            }
        }
        /// <summary>
        /// 将列设置信息绑定到grid控件中
        /// </summary>
        /// <param name="cols">grid控件的列构造器</param>
        /// <param name="columns">自定义列设置信息</param>
        public static void BoundColumns(GridColumnFactory<DataRow> cols, List<GridColumnSetting> columns)
        {
            foreach (var column in columns)
            {
                var col = cols.Bound(column.Name).Title(column.Title).Hidden(column.Hidden);
                if (column.Width > 0)
                {
                    col = col.Width(column.Width);
                }
                if (!string.IsNullOrEmpty(column.format))
                {
                    col = col.Format(column.format);
                }
            }
        }
        /// <summary>
        /// 客情列表界面的绑定列
        /// </summary>
        /// <param name="cols">grid控件的列构造器</param>
        /// <param name="columns">自定义列设置信息</param>
        public static void BoundColumns(GridColumnFactory<UpQueryResDetailResult> cols, List<GridColumnSetting> columns)
        {
            foreach (var column in columns)
            {
                var col = cols.Bound(column.Name).Title(column.Title).Hidden(column.Hidden);
                if (column.Width > 0)
                {
                    col = col.Width(column.Width);
                }
                if (!string.IsNullOrEmpty(column.format))
                {
                    col = col.Format(column.format);
                }
                if (column.HtmlAttributes != null)
                {
                    col.HtmlAttributes(new { @class = "td_regid_a", hid = "#: Hid #", regid = "#: RegId #", statusname = "#: StatuName #", style = "#:IsCustemSecret==1?'color:red;':''#" });
                }
                else
                {
                    col.HtmlAttributes(new { style = "#:IsCustemSecret==1?'color:red;':''#" });
                }
            }
        }
    }
}