namespace Gemstar.BSPMS.Hotel.Web.Models {
    /// <summary>
    /// Grid列可定义属性设置
    /// </summary>
    public class GridColumnSetting {
        /// <summary>
        /// 数据源中的列名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// header中显示的标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool Hidden { get; set; }
        /// <summary>
        /// 列宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 用来设置时间格式。
        /// </summary>
        public string format { get; set; }
        /// <summary>
        /// HTML属性
        /// </summary>
        public object HtmlAttributes { get; set; }
        /// <summary>
        /// 页面底部模板
        /// </summary>
        public string ClientFooterTemplate { get; set; }

        

    }
}