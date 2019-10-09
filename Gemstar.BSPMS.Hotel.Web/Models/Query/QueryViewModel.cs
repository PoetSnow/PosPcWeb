namespace Gemstar.BSPMS.Hotel.Web.Models.Query
{
    /// <summary>
    /// 通用查询参数视图模型
    /// </summary>
    public class QueryViewModel
    {
        /// <summary>
        /// 参数索引
        /// </summary>
        public int ParameterIndex { get; set; }
        /// <summary>
        /// 参数名称
        /// </summary>
        public string ParameterName { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public string ParameterValue { get; set; }
        /// <summary>
        /// 参数值的显示文本
        /// </summary>
        public string ParameterValueText { get; set; }
        /// <summary>
        /// 其他自定义显示属性
        /// </summary>
        public object CustomViewPara { get; set; }
    }
}