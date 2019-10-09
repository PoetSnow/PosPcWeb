using System;

namespace Gemstar.BSPMS.Common.PayManage.WxProviderPay
{
    public class WxPayException : Exception
    {
        public WxPayException(string msg) : base(msg)
        {

        }
        public WxPayException(string msg,Exception innerException) : base(msg, innerException)
        {

        }
    }
}