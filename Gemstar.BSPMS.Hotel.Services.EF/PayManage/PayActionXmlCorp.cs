using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EnumsPos;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 挂合约单位账
    /// </summary>
    public class PayActionXmlCorp:PayActionXmlBase
    {
        private IPayActionXmlOperate _xmlOperate;
        public PayActionXmlCorp(IPayActionXmlOperate xmlOperate)
        {
            _xmlOperate = xmlOperate;
        }
        public override PosBillDetailStatus DefaultDetailStatus => PosBillDetailStatus.作废;
        public override JsonResultData DoOperate(PaymentOperatePara para, PaymentOperateBusinessPara businessPara)
        {
            var xmlStrBuilder = new StringBuilder();
            AppendBeforeBusinessXmlStr("合约单位挂账", xmlStrBuilder);
            xmlStrBuilder.Append("<CompanyFolio>")
                .Append("<Hid>").Append(businessPara.Hid).Append("</Hid>")
                .Append("<isCheck>0</isCheck>")
                .Append("<RefNo>").Append(businessPara.AddedBillResult.DetailId).Append("</RefNo>")
                .Append("<CompanyId>").Append(para.ProfileId).Append("</CompanyId>")
                .Append("<Sign>").Append(para.Sign).Append("</Sign>")
                .Append("<OutletCode>").Append(businessPara.PosOutlteCode).Append("</OutletCode>")
                .Append("<PosCode></PosCode>")
                .Append("<Amount>").Append(para.Amount).Append("</Amount>")
                .Append("<InvNo>").Append(businessPara.AddedBillResult.BillNo).Append("</InvNo>")
                .Append("<Operator>").Append(businessPara.UserName).Append("</Operator>")
                .Append("<Remark>[POS客账号：").Append(businessPara.AddedBillResult.BillNo).Append(";收银点：").Append(businessPara.PosName).Append(";金额：").Append(para.Amount).Append(";酒店ID：").Append(businessPara.Hid).Append("]</Remark>")
                .Append("</CompanyFolio>");
            AppendAfterBusinessXmlStr(xmlStrBuilder);
            var xmlInfo = _xmlOperate.RealOperate(businessPara.Hid, "", xmlStrBuilder.ToString());
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlInfo);
            Dictionary<string, string> xmls = new Dictionary<string, string>();
            if (doc != null)
            {
                if (doc["CompanyFolio"] != null)
                {
                    if (doc["CompanyFolio"]["Rows"] != null)
                    {
                        if (doc["CompanyFolio"]["Rows"]["Row"] != null)
                        {
                            foreach (XmlNode node in doc["CompanyFolio"]["Rows"]["Row"])
                            {
                                if (node != null && node.Name != null && node.FirstChild != null)
                                {
                                    if (node.Name == "RC")
                                    {
                                        if (Convert.ToInt32(node.FirstChild.Value) == 0)
                                        {
                                            return JsonResultData.Successed("");
                                        } else
                                        {
                                            return JsonResultData.Failure(doc["CompanyFolio"]["Rows"]["Row"]["ErrMsg"].InnerText);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return JsonResultData.Failure("合约单位挂账失败，请稍后重试！");
        }
    }
}

