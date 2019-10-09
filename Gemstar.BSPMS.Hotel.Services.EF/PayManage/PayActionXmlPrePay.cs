using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Tools;
using Gemstar.BSPMS.Hotel.Services.EF.PosManage;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.PosManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    public class PayActionXmlPrePay : PayActionXmlBase
    {
        private IYtPrepayService _YtPrepayService;
        public PayActionXmlPrePay(IYtPrepayService ytPrepayService)
        {
            _YtPrepayService = ytPrepayService;
        }

        public override PosBillDetailStatus DefaultDetailStatus => PosBillDetailStatus.正常;

        public override JsonResultData DoCheck(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            var model = _YtPrepayService.GetPrePayInfoById(businessPara.Hid, "CY", para.PrePayId);
            if (para.Amount> model.BalanceAmount)
            {
                para.Amount = model.BalanceAmount;
            }
            return JsonResultData.Successed();
        }

        public override JsonResultData DoOperate(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            try
            {
                var oriModel = _YtPrepayService.Get(Guid.Parse(para.PrePayId));

                //插入一条押金付款信息
                var preModel = new YtPrepay();
                AutoSetValueHelper.SetValues(oriModel, preModel);
                bool isClear = false;
                if (businessPara.AddedBillResult.DueAmount == preModel.Amountbwb)
                {
                    isClear = true;
                }
                preModel.Id = Guid.NewGuid();
                preModel.Amount = businessPara.AddedBillResult.DueAmount;
                preModel.Amountbwb = businessPara.AddedBillResult.DueAmount;
                preModel.Creator = businessPara.UserName;
                preModel.CreateDate = DateTime.Now;
                preModel.PaidNo = businessPara.AddedBillResult.DetailId;
                preModel.IPrepay = (byte)PrePayStatus.押金付款;
                _YtPrepayService.Add(preModel);
                _YtPrepayService.AddDataChangeLog(OpLogType.Pos定金添加);
                _YtPrepayService.Commit();

                //添加押金退款数据
                if (para.IsRefund == "1" && isClear == false) //退款
                {
                    isClear = true;
                    var RefundModel = new YtPrepay();
                    AutoSetValueHelper.SetValues(oriModel, RefundModel);
                    RefundModel.Id = Guid.NewGuid();
                    RefundModel.Amountbwb = oriModel.Amountbwb - businessPara.AddedBillResult.DueAmount;
                    RefundModel.Creator = businessPara.UserName;
                    RefundModel.CreateDate = DateTime.Now;
                    RefundModel.IPrepay = (byte)PrePayStatus.退押金;
                    RefundModel.PaidNo = businessPara.AddedBillResult.DetailId;
                    _YtPrepayService.Add(RefundModel);
                    _YtPrepayService.AddDataChangeLog(OpLogType.Pos定金添加);
                    _YtPrepayService.Commit();
                }


                //修改原押金单的已结状态
                if (isClear)
                {
                    //修改所有的押金状态为已结
                    _YtPrepayService.UpdatePrePayStatus(businessPara.Hid, oriModel.BillNo, "CY");
                }


            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex.Message.ToString());
            }

            return JsonResultData.Successed();
        }

    }
}
