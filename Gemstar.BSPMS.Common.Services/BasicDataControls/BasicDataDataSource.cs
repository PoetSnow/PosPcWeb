namespace Gemstar.BSPMS.Common.Services.BasicDataControls
{
    /// <summary>
    /// 基础数据数据来源
    /// </summary>
    public class BasicDataDataSource
    {
        /// <summary>
        /// 自主增加
        /// </summary>
        public static BasicDataDataSource Added = new BasicDataDataSource { Code = "add", Name = "自主增加" };
        /// <summary>
        /// 集团分发
        /// </summary>
        public static BasicDataDataSource Copyed = new BasicDataDataSource { Code = "copy", Name = "集团分发" };
        private BasicDataDataSource()
        {

        }
        /// <summary>
        /// 数据来源代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 数据来源名称
        /// </summary>
        public string Name { get; set; }
    }
}
