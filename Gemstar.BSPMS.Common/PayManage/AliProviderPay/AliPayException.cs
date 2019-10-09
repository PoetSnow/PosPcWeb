using System;

namespace Gemstar.BSPMS.Common.PayManage.AliProviderPay
{
    public class AliPayException : Exception
    {
        public AliPayException(string msg) : base(msg)
        {

        }
    }
}