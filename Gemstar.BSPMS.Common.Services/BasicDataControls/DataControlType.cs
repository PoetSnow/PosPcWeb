namespace Gemstar.BSPMS.Common.Services.BasicDataControls
{
    /// <summary>
    /// 数据分发类型
    /// </summary>
    public class DataControlType
    {
        private DataControlType()
        {

        }
        /// <summary>
        /// 全部分店
        /// </summary>
        public static DataControlType AllResorts = new DataControlType { Code = "all", Name = "全部分店" };
        /// <summary>
        /// 选择分店
        /// </summary>
        public static DataControlType SelectedResorts = new DataControlType { Code = "selected", Name = "选择分店" };
        /// <summary>
        /// 根据分店属性创建分发类型实例
        /// </summary>
        /// <param name="code">分店属性代码</param>
        /// <param name="name">分店属性名称</param>
        /// <returns>分店属性分发类型实例</returns>
        public static DataControlType GetControlType(string code, string name)
        {
            return new DataControlType { Code = code, Name = name };
        }
        /// <summary>
        /// 分发类型代码
        /// </summary>
        public string Code { get; private set; }
        /// <summary>
        /// 分发类型名称
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 选中的分店id，多项以逗号分隔
        /// </summary>
        public string SelectedHids { get; set; }
    }
}
