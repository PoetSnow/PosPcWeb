using System;

namespace Gemstar.BSPMS.Common.PayManage
{
    /// <summary>
    /// 不记录日志的空服务实现
    /// </summary>
    public class PayLogNotRecordService : IPayLogService
    {
        public void Debug(string hid, string className, string content)
        {
        }

        public void Error(string hid, string className, Exception ex)
        {
        }

        public void Error(string hid, string className, string content)
        {
        }

        public void Info(string hid, string className, string content)
        {
        }
    }
}
