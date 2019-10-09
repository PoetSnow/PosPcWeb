using System;
using System.Net;
using System.Text;
using System.Xml;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    public class PayActionXmlHouse : PayActionXmlBase
    {
        private IPayActionXmlOperate _xmlOperate;
        public PayActionXmlHouse(IPayActionXmlOperate xmlOperate)
        {
            _xmlOperate = xmlOperate;
        }
        public override JsonResultData DoCheck(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(para.RoomNo))
                {
                    var xmlStrBuilder = new StringBuilder();
                    AppendBeforeBusinessXmlStr("房账客人资料查询", xmlStrBuilder);
                    xmlStrBuilder.Append("<RoomFolio>")
                            .Append("<hid>").Append(businessPara.Hid).Append("</hid>")
                            .Append("<roomNo>").Append(para.RoomNo).Append("</roomNo>")
                            .Append("<guestCName></guestCName>")
                            .Append("<Regid></Regid>")
                            .Append("<Outlet></Outlet>")
                        .Append("</RoomFolio>");
                    AppendAfterBusinessXmlStr(xmlStrBuilder);
                    var xmlInfo = _xmlOperate.RealOperate(businessPara.Hid, "", xmlStrBuilder.ToString());
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlInfo);
                    if (doc != null)
                    {
                        if (doc["RoomFolio"] != null)
                        {
                            if (doc["RoomFolio"]["Rows"] != null)
                            {
                                if (doc["RoomFolio"]["Rows"]["Row"] != null)
                                {
                                    foreach (XmlNode node in doc["RoomFolio"]["Rows"]["Row"])
                                    {
                                        if (node != null && node.Name != null && node.FirstChild != null)
                                        {
                                            if (node != null && node.Name != null && node.FirstChild != null)
                                            {
                                                if (node.Name.Equals("isTransfer", StringComparison.CurrentCultureIgnoreCase))
                                                {
                                                    if (Convert.ToInt32(doc["RoomFolio"]["Rows"]["Row"]["isTransfer"].FirstChild.Value) == 1)
                                                    {
                                                        return JsonResultData.Successed();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
            return JsonResultData.Failure("未指定要挂房账的房间号");
        }
        public override JsonResultData DoOperate(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            var xmlStrBuilder = new StringBuilder();
            AppendBeforeBusinessXmlStr("房账挂账", xmlStrBuilder);
            xmlStrBuilder.Append("<RoomFolio>")
                .Append("<hid>").Append(businessPara.Hid).Append("</hid>")
                .Append("<isCheck>0</isCheck>")
                .Append("<refNo>" + businessPara.AddedBillResult.DetailId + "</refNo>")
                .Append("<RoomNo>" + para.RoomNo + "</RoomNo>")
                .Append("<Regid>" + businessPara.AddedBillResult.BillNo + "</Regid>")
                .Append("<OutletCode>" + businessPara.PosOutlteCode + "</OutletCode>")
                .Append("<PosCode>" + Dns.GetHostName() + "</PosCode>")
                .Append("<Amount>" + para.Amount + "</Amount>")
                .Append("<Invno>" + para.Billid + "</Invno>")
                .Append("<Remark>[POS客账号：" + businessPara.AddedBillResult.BillNo + ";收银点：" + businessPara.PosName + ";金额：" + para.Amount + ";酒店ID：" + businessPara.Hid + "]</Remark>")
                .Append("<Operator>" + businessPara.UserName + "</Operator>")
                .Append("<GuestCname></GuestCname>")
                .Append("<LockCardNo></LockCardNo>")
                .Append("</RoomFolio>");
            AppendAfterBusinessXmlStr(xmlStrBuilder);

            var xmlInfo = _xmlOperate.RealOperate(businessPara.Hid, "", xmlStrBuilder.ToString());
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlInfo);
            if (doc != null)
            {
                if (doc["RoomFolio"] != null)
                {
                    if (doc["RoomFolio"]["Rows"] != null)
                    {
                        if (doc["RoomFolio"]["Rows"]["Row"] != null)
                        {
                            foreach (XmlNode node in doc["RoomFolio"]["Rows"]["Row"])
                            {
                                if (node != null && node.Name != null && node.FirstChild != null)
                                {
                                    if (node.Name == "RC")
                                    {
                                        if (Convert.ToInt32(node.FirstChild.Value) == 0)
                                        {
                                            return JsonResultData.Successed();
                                        } else
                                        {
                                            return JsonResultData.Failure(doc["RoomFolio"]["Rows"]["Row"]["ErrMsg"].InnerText);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return JsonResultData.Failure("收到格式不正确的接口返回结果：" + xmlInfo);
        }
        public override PosBillDetailStatus DefaultDetailStatus => PosBillDetailStatus.作废;
    }
}
