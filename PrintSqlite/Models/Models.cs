using System.Data;

namespace PrintSqlite
{
    /// <summary>
    /// 打印机列表模型
    /// </summary>
    public class PrintListModel
    {
        /// <summary>
        /// 等待打印列表
        /// </summary>
        public DataTable WaitDataTable { get; set; }

        /// <summary>
        /// 打印机
        /// </summary>
        public DataRow Print { get; set; }
    }
}
