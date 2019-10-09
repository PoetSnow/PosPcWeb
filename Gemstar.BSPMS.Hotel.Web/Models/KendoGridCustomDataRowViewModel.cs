using Kendo.Mvc.UI.Fluent;
using System;
using System.Data;

namespace Gemstar.BSPMS.Hotel.Web.Models
{
    /// <summary>
    /// 用于通用的datarowview的视图模型
    /// 传递用于生成grid时需要的相关动态参数，以便其他参数统一由此分部视图来完成
    /// </summary>
    public class KendoGridCustomDataRowViewModel : KendoGridCustomCommonViewModel
    {
        public  Action<GridToolBarCommandFactory<DataRowView>> CustomToolbar { get; set; }

        public Action<DataSourceAggregateDescriptorFactory<DataRowView>> Aggregates { get; set; }
    }
}
