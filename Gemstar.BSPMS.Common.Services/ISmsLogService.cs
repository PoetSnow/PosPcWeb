using System;

namespace Gemstar.BSPMS.Common.Services
{
    public interface ISmsLogService
    {
        void AddLog(string mobile, string msg, string returnMsg, DateTime? sendDate = null);
    }
}
