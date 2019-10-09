using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.PosManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 会员增值金额支付
    /// </summary>
    public class PayActionXmlMbrLargess:PayActionXmlBase
    {

        private IPosShiftService _posShiftService;
        private IPayActionXmlOperate _xmlOperate;
        public PayActionXmlMbrLargess(IPosShiftService posShiftService, IPayActionXmlOperate xmlOperate)
        {
            _posShiftService = posShiftService;
            _xmlOperate = xmlOperate;
        }
        public override PosBillDetailStatus DefaultDetailStatus => PosBillDetailStatus.作废;
        public override JsonResultData DoOperate(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            var shift = _posShiftService.GetEntity(businessPara.Hid, businessPara.AddedBillResult.BillShiftid);
            var xmlStrBuilder = new StringBuilder();
            AppendBeforeBusinessXmlStr("会员交易", xmlStrBuilder);
            xmlStrBuilder.Append("<ProfileCa>")
                .Append("<ProfileId>").Append(para.ProfileId).Append("</ProfileId>")
                .Append("<OutletCode>").Append(businessPara.PosOutlteCode).Append("</OutletCode>")
                .Append("<HotelCode>").Append(businessPara.Hid).Append("</HotelCode>")
                .Append("<BalanceType>").Append(((byte)BalanceType.赠送金额).ToString("D2")).Append("</BalanceType>")
                .Append("<PaymentDesc>").Append(((byte)PaymentDesc.扣款).ToString("D2")).Append("</PaymentDesc>")
                .Append("<Amount>").Append(0 - para.Amount).Append("</Amount>")
                .Append("<RefNo>").Append(businessPara.AddedBillResult.DetailId).Append("</RefNo>")
                .Append("<Remark>[POS客账号：").Append(businessPara.AddedBillResult.BillNo).Append(";收银点：").Append(businessPara.PosName).Append(";金额：").Append(para.Amount).Append(";酒店ID：").Append(businessPara.Hid).Append("]</Remark>")
                .Append("<Creator>").Append(businessPara.UserName).Append("</Creator>")
                .Append("<PayTypeId>").Append(para.Itemid).Append("</PayTypeId>")
                .Append("<ShiftId>").Append(shift.Code).Append("</ShiftId>")
                .Append("<TransBsnsDate>").Append(businessPara.AddedBillResult.PosBusinessDate).Append("</TransBsnsDate>")
                .Append("</ProfileCa>");
            AppendAfterBusinessXmlStr(xmlStrBuilder);
            var xmlInfo = _xmlOperate.RealOperate(businessPara.Hid, "", xmlStrBuilder.ToString());
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlInfo);
            Dictionary<string, string> xmls = new Dictionary<string, string>();
            if (doc != null)
            {
                if (doc["ReturnMessage"] != null)
                {
                    foreach (XmlNode node in doc["ReturnMessage"])
                    {
                        if (node != null && node.Name != null && node.FirstChild != null)
                        {
                            if (node.Name == "MessageNo")
                            {
                                if (Convert.ToInt32(node.FirstChild.Value) == 1)
                                {
                                    return JsonResultData.Successed("");
                                }
                            }
                        }
                    }
                } else if (doc["ErrorMessage"] != null)
                {
                    if (doc["ErrorMessage"]["Message"] != null)
                    {
                        return JsonResultData.Failure(doc["ErrorMessage"]["Message"].InnerText);
                    }
                }
            }

            return JsonResultData.Failure("会员增值支付失败，请稍后重试！");
        }
    }
}
