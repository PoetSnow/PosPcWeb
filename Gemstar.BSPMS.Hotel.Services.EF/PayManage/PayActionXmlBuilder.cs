using System;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    public class PayActionXmlBuilder
    {
        private IDependencyResolver _dependencyResolver;
        public PayActionXmlBuilder(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }
        public PayActionXmlBase Build(string action)
        {
            if (action.Equals("house", StringComparison.OrdinalIgnoreCase))
            {
                return _dependencyResolver.GetService<PayActionXmlHouse>();
            }
            if (action.Equals("mbrCard", StringComparison.OrdinalIgnoreCase))
            {
                return _dependencyResolver.GetService<PayActionXmlMbrcard>();
            }
            if (action.Equals("mbrLargess", StringComparison.OrdinalIgnoreCase))
            {
                return _dependencyResolver.GetService<PayActionXmlMbrLargess>();
            }
            if (action.Equals("mbrCardAndLargess", StringComparison.OrdinalIgnoreCase))
            {
                return _dependencyResolver.GetService<PayActionXmlMbrCardAndLargess>();
            }
            if (action.Equals("corp", StringComparison.OrdinalIgnoreCase))
            {
                return _dependencyResolver.GetService<PayActionXmlCorp>();
            }
            if (action.Equals("AliBarcode", StringComparison.OrdinalIgnoreCase)
                    || action.Equals("WxBarcode", StringComparison.OrdinalIgnoreCase))
            {
                return _dependencyResolver.GetService<PayActionXmlOnlineBarCode>();
            }
            if (action.Equals("PrePay", StringComparison.OrdinalIgnoreCase))
            {
                return _dependencyResolver.GetService<PayActionXmlPrePay>();
            }
            return new PayActionXmlBase();
        }
    }
}
