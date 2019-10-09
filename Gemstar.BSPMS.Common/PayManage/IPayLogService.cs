using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.PayManage
{
    /// <summary>
    /// 支付日志记录接口
    /// </summary>
    public interface IPayLogService
    {
        /// <summary>
        /// 向日志文件写入调试信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="className">类名</param>
        /// <param name="content">写入内容</param>
        void Debug(string hid,string className, string content);
        /// <summary>
        /// 向日志文件写入运行时信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="className">类名</param>
        /// <param name="content">写入内容</param>
        void Info(string hid,string className, string content);
        /// <summary>
        /// 向日志文件写入出错信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="className">类名</param>
        /// <param name="content">写入内容</param>
        void Error(string hid, string className, string content);

        /// <summary>
        /// 向日志文件写入出错信息
        /// </summary>
        /// <param name="hid">酒店id</param>
        /// <param name="className">类名</param>
        /// <param name="ex">异常实例</param>
        void Error(string hid, string className, Exception ex);
    }
}
