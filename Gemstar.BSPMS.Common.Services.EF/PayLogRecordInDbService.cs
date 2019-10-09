using System;
using System.Text;
using Gemstar.BSPMS.Common.PayManage;

namespace Gemstar.BSPMS.Common.Services.EF
{
    /// <summary>
    /// 记录支付日志到数据库的服务实例
    /// </summary>
    public class PayLogRecordInDbService : IPayLogService
    {
        public PayLogRecordInDbService(DbCommonContext commonDb)
        {
            _commonDb = commonDb;
        }

        public void Debug(string hid, string className, string content)
        {
            AddLog("Debug", hid, className, content);
        }

        public void Error(string hid, string className, Exception ex)
        {
            var innerException = ex;
            while (innerException.InnerException != null)
            {
                innerException = innerException.InnerException;
            }
            var errInfo = new StringBuilder();
            errInfo.Append("错误信息:").AppendLine(innerException.Message)
                .Append("堆栈信息：").AppendLine(innerException.StackTrace);

            AddLog("Error", hid, className, errInfo.ToString());
        }

        public void Error(string hid, string className, string content)
        {
            AddLog("Error", hid, className, content);
        }

        public void Info(string hid, string className, string content)
        {
            AddLog("Info", hid, className, content);
        }
        private void AddLog(string level,string hid,string className,string content)
        {
            _commonDb.PayLogs.Add(new Entities.PayLog
            {
                Hid = hid,
                Level = level,
                Title = className,
                Content = content,
                CDate = DateTime.Now
            });
            _commonDb.SaveChanges();
        }
        private DbCommonContext _commonDb;
    }
}
