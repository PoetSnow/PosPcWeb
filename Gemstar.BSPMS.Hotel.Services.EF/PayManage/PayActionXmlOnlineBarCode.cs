using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Script.Serialization;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Hotel.Services.SystemManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 微信条码支付或者支付宝条码支付
    /// </summary>
    public class PayActionXmlOnlineBarCode:PayActionXmlBase
    {
        private IPayServiceBuilder _payServiceBuilder;
        private DbCommonContext _dbCommon;
        private IPmsParaService _pmsParaService;
        private IWaitPayListService _waitPayListService;
        private IoperationLog _operationLog;
        public PayActionXmlOnlineBarCode(IPayServiceBuilder payServiceBuilder,DbCommonContext dbCommon,IPmsParaService pmsParaService,IWaitPayListService waitPayListService,IoperationLog operationLog)
        {
            _payServiceBuilder = payServiceBuilder;
            _dbCommon = dbCommon;
            _pmsParaService = pmsParaService;
            _waitPayListService = waitPayListService;
            _operationLog = operationLog;
        }

        public override PosBillDetailStatus DefaultDetailStatus => PosBillDetailStatus.作废;
        public override JsonResultData DoCheck(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            var settleTransno = Guid.NewGuid().ToString("N");
            businessPara.SettleTransNo = settleTransno;
            return JsonResultData.Successed("");
        }
        public override JsonResultData DoOperate(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            var commonPayParas = _dbCommon.M_v_payParas.ToList();
            var hotelPayParas = _pmsParaService.GetPmsParas(businessPara.Hid);

            OpLogType logType = OpLogType.Pos付款;
            var payService = _payServiceBuilder.GetPayService(para.FolioItemAction, commonPayParas, hotelPayParas, businessPara.IsTestEnv);
            using (var tc = new TransactionScope())
            {
                //如果是付款，则获取支付服务实例，进行支付，并且将支付成功后返回的交易号保存到refno中
                var payResult = new PayResult { RefNo = "", IsWaitPay = false };
                if (payService != null)
                {
                    if (string.IsNullOrWhiteSpace(para.FolioItemActionJsonPara))
                    {
                        return JsonResultData.Failure("参数不能为空");
                    }
                    payResult = payService.DoPayBeforeSaveFolio(para.FolioItemActionJsonPara);
                }
                if (payResult.IsWaitPay)
                {
                    //如果是待支付的，则增加一待支付的记录，等到支付成功回调处理中再调存储过程来进行处理
                    var waitPay = new WaitPayList
                    {
                        WaitPayId = Guid.Parse(businessPara.SettleTransNo),
                        CreateDate = DateTime.Now,
                        ProductType = PayProductType.PosPayment.ToString(),
                        Status = 0
                    };

                    var payPara = new
                    {
                        Hid = businessPara.Hid,
                        Billid = para.Billid,
                        Refeid = businessPara.AddedBillResult.BillRefeId,
                        Tabid = businessPara.AddedBillResult.BillTabId,

                        //账单明细
                        IsCheck = 1,
                        Settleid = businessPara.SettleId,
                        SettleBsnsDate = businessPara.AddedBillResult.PosBusinessDate,
                        SettleShuffleid = businessPara.AddedBillResult.BillShuffleid,
                        SettleShiftId = businessPara.AddedBillResult.BillShiftid,
                        SettleUser = businessPara.UserName,
                        SettleTransno = businessPara.SettleTransNo,
                        SettleTransName = "买单",

                        //餐台状态
                        TabStatus = (byte)PosTabStatusEnum.空净,
                        OpTabid = "",

                        //账单
                        DepBsnsDate = businessPara.AddedBillResult.PosBusinessDate,
                        MoveUser = businessPara.UserName,
                        Status = (byte)PosBillStatus.清台,
                        WaitStatus = 1,
                        BillDetailStatus = PosBillDetailStatus.正常,
                        Isauto = (byte)PosBillDetailIsauto.付款,
                    };

                    var serializer = new JavaScriptSerializer();
                    var payParaStr = serializer.Serialize(businessPara);
                    waitPay.BusinessPara = payParaStr.ReplaceJsonDateToDateString();
                    _waitPayListService.Add(waitPay);
                    _waitPayListService.Commit();
                }

                tc.Complete();
                tc.Dispose();
            }

            //记录日志
            var logStr = new StringBuilder();
            logStr.Append("Pos付款")
                .Append(" 金额：").Append(para.Amount)
                .Append("，结账ID：").Append(businessPara.SettleTransNo)
                .Append("，付款方式：").Append(businessPara.AddedBillResult.ItemName);
            if (!string.IsNullOrWhiteSpace(para.Billid))
            {
                logStr.Append("，单号:").Append(para.Billid);
            }
            _operationLog.AddOperationLog(businessPara.Hid, logType, logStr.ToString(), businessPara.UserName, UrlHelperExtension.GetRemoteClientIPAddress(), "");

            var returnResult = new ResFolioAddResult
            {
                FolioTransId = businessPara.SettleTransNo,
                Statu = PayStatu.Successed.ToString(),
                Callback = "",
                QrCodeUrl = "",
                QueryTransId = "",
                DCFlag = PosItemDcFlag.C.ToString()
            };

            if (payService != null)
            {
                //转换一下folio的transid格式，以保证长度为32位
                var afterPayResult = payService.DoPayAfterSaveFolio(PayProductType.PosPayment, businessPara.SettleTransNo, para.FolioItemActionJsonPara);
                returnResult.Statu = afterPayResult.Statu.ToString();
                returnResult.Callback = afterPayResult.Callback;
                returnResult.QrCodeUrl = afterPayResult.QrCodeUrl;
                returnResult.QueryTransId = afterPayResult.QueryTransId;
            }

            return JsonResultData.Failure(returnResult, 2);
        }
    }
}
