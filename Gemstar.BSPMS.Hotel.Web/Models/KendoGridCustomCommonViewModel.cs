using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Web.Models
{
    public class KendoGridCustomCommonViewModel: KendoGridBaseViewModel {
        public KendoGridCustomCommonViewModel()
        {
            HtmlAttributes = null;
        }
        /// <summary>
        /// 状态值对应的列名称，用于从数据行中获取状态值，根据状态值来判断是否禁用，显示禁用状态
        /// </summary>
        public string StatusColumnName { get; set; }
        /// <summary>
        /// 当前表格列表的area
        /// </summary>
        public string Area { get; set; }
        /// <summary>
        /// 当前表格列表的controller
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// 当前表格列表的action
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 默认的列设置信息
        /// </summary>
        public List<GridColumnSetting> DefaultColumnSettings { get; set; }

        /// <summary>
        /// 表格属性
        /// </summary>
        public IDictionary<string,object> HtmlAttributes { get; set; }

    }
}
