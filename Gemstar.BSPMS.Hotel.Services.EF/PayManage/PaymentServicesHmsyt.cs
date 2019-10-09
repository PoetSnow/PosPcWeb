using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    public class PaymentServicesHmsyt : IPaymentServices
    {
        public AddBillResult AddBill(PaymentOperatePara para, PaymentOperateBusinessPara businessPara, PosBillDetailStatus detailStatu)
        {
            throw new NotImplementedException();
        }

        public void ChangeBillStatuWhenSuccess(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            throw new NotImplementedException();
        }

        public void ChangeCashMemo(PaymentOperatePara para)
        {
            throw new NotImplementedException();
        }

        public void ClearTable(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            throw new NotImplementedException();
        }

        public PaymentBillInfo GetPaymentBillInfo(PaymentBillPara para)
        {
            throw new NotImplementedException();
        }

        public bool IsAllPaid(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            throw new NotImplementedException();
        }

        public void Settle(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            throw new NotImplementedException();
        }

        public void SmallChange(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            throw new NotImplementedException();
        }

        public void TailAmount(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            throw new NotImplementedException();
        }
    }
}
