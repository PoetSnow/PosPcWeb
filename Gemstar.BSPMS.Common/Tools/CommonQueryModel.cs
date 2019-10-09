namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 通用的查询模型
    /// </summary>
    public class CommonQueryModel
    {
        /// <summary>
        /// grid列表控件id，用于在选择完查询参数后进行查询操作
        /// </summary>
        public string GridControlId { get; set; }
        /// <summary>
        /// 查询用的存储过程名称
        /// </summary>
        public string QueryProcedureName { get; set; }
        /// <summary>
        /// 查询参数名称及值
        /// </summary>
        public string QueryParameterValues { get; set; }
    }
}
