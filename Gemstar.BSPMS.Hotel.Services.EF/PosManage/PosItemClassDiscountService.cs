using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.PosManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Gemstar.BSPMS.Hotel.Services.EF.PosManage
{
    public class PosItemClassDiscountService : CRUDService<PosOnSale>, IPosItemClassDiscountService
    {
        private DbHotelPmsContext _db;
        private IPayActionXmlOperate _xmlOperate;

        public PosItemClassDiscountService(DbHotelPmsContext db, IPayActionXmlOperate xmlOperate) : base(db, db.PosOnSales)
        {
            _db = db;
            _xmlOperate = xmlOperate;
        }

        protected override PosOnSale GetTById(string id)
        {
            return new PosOnSale { Id = new Guid(id) };
        }

        /// <summary>
        /// 检测是否有冲突的大类折扣记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="itemid">消费项目ID</param>
        /// <param name="itemClassid">消费项目大类ID</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns></returns>
        public bool IsExists(string hid, string module, string itemid, string itemClassid, string CustomerTypeid, string Refeid, string TabTypeid, string Unitid, string startTime, string endTime)
        {
            Func<PosOnSale, bool> wherefunc = b =>
            {
                var res = true;
                res = b.Hid == hid
                    && b.Module == module
                    && b.iType == 2
                    && (b.CustomerTypeid == null || b.CustomerTypeid == CustomerTypeid)
                    && (b.Refeid == null || b.Refeid == Refeid)
                    && (b.TabTypeid == null || b.TabTypeid == TabTypeid)
                    && (b.Unitid == null || b.Unitid == Unitid)
                    && b.Itemid == itemid
                    && b.ItemClassid == itemClassid;

                return res;
            };
            var list = _db.PosOnSales.Where(wherefunc).ToList();

            if (list == null || list.Count == 0)
            {
                return false;
            }
            else
            {
                string today = DateTime.Today.ToString("yyyy-MM-dd");

                startTime = today + " " + startTime;
                endTime = today + " " + endTime;

                //结束时间时候是否为次日
                if (Convert.ToDateTime(startTime) >= Convert.ToDateTime(endTime))
                {
                    endTime = Convert.ToDateTime(endTime).AddDays(1).ToString();
                }

                //循环比较时间范围
                foreach (var temp in list)
                {
                    temp.StartTime = today + " " + temp.StartTime;
                    temp.EndTime = today + " " + temp.EndTime;

                    //结束时间时候是否为次日
                    if (Convert.ToDateTime(temp.StartTime) >= Convert.ToDateTime(temp.EndTime))
                    {
                        if (Convert.ToDateTime(temp.StartTime) >= Convert.ToDateTime(startTime) && Convert.ToDateTime(startTime) >= Convert.ToDateTime(endTime))
                        {
                            startTime = Convert.ToDateTime(startTime).AddDays(1).ToString();
                        }
                        else if (Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.StartTime) && Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.EndTime))
                        {
                            startTime = Convert.ToDateTime(startTime).AddDays(1).ToString();
                        }

                        temp.EndTime = Convert.ToDateTime(temp.EndTime).AddDays(1).ToString();
                    }

                    if (Convert.ToDateTime(temp.StartTime) <= Convert.ToDateTime(startTime) && Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.EndTime))
                    {
                        return true;
                    }
                    else if (Convert.ToDateTime(temp.StartTime) <= Convert.ToDateTime(endTime) && Convert.ToDateTime(endTime) <= Convert.ToDateTime(temp.EndTime))
                    {
                        return true;
                    }
                    else if (Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.StartTime) && Convert.ToDateTime(endTime) >= Convert.ToDateTime(temp.EndTime))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 检测是否有冲突的大类折扣记录
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="module">模块</param>
        /// <param name="itemid">消费项目ID</param>
        /// <param name="itemClassid">消费项目大类ID</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns></returns>
        public bool IsExists(string hid, string module, string itemid, string itemClassid, string CustomerTypeid, string Refeid, string TabTypeid, string Unitid, string startTime, string endTime, Guid exceptId)
        {
            Func<PosOnSale, bool> wherefunc = b =>
            {
                var res = true;
                res = b.Hid == hid
                    && b.Module == module
                    && b.iType == 2
                    && b.Id != exceptId
                    && (b.CustomerTypeid == CustomerTypeid)
                    && (b.Refeid == Refeid)
                    && (b.TabTypeid == TabTypeid)
                    && (b.Unitid == Unitid)
                        && b.Itemid == itemid
                    && b.ItemClassid == itemClassid;
                return res;
            };
            var list = _db.PosOnSales.Where(wherefunc).ToList();

            if (list == null || list.Count == 0)
            {
                return false;
            }
            else
            {
                string today = DateTime.Today.ToString("yyyy-MM-dd");

                startTime = today + " " + startTime;
                endTime = today + " " + endTime;

                //结束时间时候是否为次日
                if (Convert.ToDateTime(startTime) >= Convert.ToDateTime(endTime))
                {
                    endTime = Convert.ToDateTime(endTime).AddDays(1).ToString();
                }

                //循环比较时间范围
                foreach (var temp in list)
                {
                    temp.StartTime = today + " " + temp.StartTime;
                    temp.EndTime = today + " " + temp.EndTime;

                    //结束时间时候是否为次日
                    if (Convert.ToDateTime(temp.StartTime) >= Convert.ToDateTime(temp.EndTime))
                    {
                        if (Convert.ToDateTime(temp.StartTime) >= Convert.ToDateTime(startTime) && Convert.ToDateTime(startTime) >= Convert.ToDateTime(endTime))
                        {
                            startTime = Convert.ToDateTime(startTime).AddDays(1).ToString();
                        }
                        else if (Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.StartTime) && Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.EndTime))
                        {
                            startTime = Convert.ToDateTime(startTime).AddDays(1).ToString();
                        }

                        temp.EndTime = Convert.ToDateTime(temp.EndTime).AddDays(1).ToString();
                    }

                    if (Convert.ToDateTime(temp.StartTime) <= Convert.ToDateTime(startTime) && Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.EndTime))
                    {
                        return true;
                    }
                    else if (Convert.ToDateTime(temp.StartTime) <= Convert.ToDateTime(endTime) && Convert.ToDateTime(endTime) <= Convert.ToDateTime(temp.EndTime))
                    {
                        return true;
                    }
                    else if (Convert.ToDateTime(startTime) <= Convert.ToDateTime(temp.StartTime) && Convert.ToDateTime(endTime) >= Convert.ToDateTime(temp.EndTime))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 获取大类折扣
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="wherefunc">表达式</param>
        /// <returns></returns>
        public List<PosOnSale> GetItemClassDisCount(string hid, Func<PosOnSale, bool> wherefunc)
        {
            return _db.PosOnSales.Where(u => u.Hid == hid).Where(wherefunc).ToList();
        }

        //获取会员卡类型
        /// <summary>
        /// 会员卡类型
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="wherefunc">lambda表达式</param>
        /// <returns></returns>
        public List<MbrCardType> GetMbrCardTypes(string hid, Func<MbrCardType, bool> wherefunc)
        {
            return _db.MbrCardTypes.Where(u => u.Hid == hid).Where(wherefunc).ToList();
        }

        //获取会员信息
        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="Hid">酒店ID</param>
        /// <param name="CardID">会员卡ID</param>
        /// <param name="RefeCode">营业点代码</param>
        /// <returns></returns>
        public JsonResultData GetMbrCardInfoByCardID(string Hid, string CardID, string RefeCode)
        {
            var xmlStrBuilder = new StringBuilder();
            AppendBeforeBusinessXmlStr("会员查询", xmlStrBuilder);
            xmlStrBuilder.Append("<MbrQuery>")
                .Append("<ProfileID>").Append("").Append("</ProfileID>")
                .Append("<NetName/>").Append("<NetPwd/>")
                .Append("<OtherKeyWord/>").Append("<OtherName/>")
                .Append("<Mobile/>")
                .Append("<MbrCardNo>").Append(CardID).Append("</MbrCardNo>") //会员卡号
                .Append("<CertificateNo>").Append("").Append("</CertificateNo>")
                 .Append("<OutletCode>").Append(RefeCode).Append("</OutletCode>") //营业点代码
                .Append("</MbrQuery>");
            AppendAfterBusinessXmlStr(xmlStrBuilder);

            var xmlInfo = _xmlOperate.RealOperate(Hid, "", xmlStrBuilder.ToString());
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlInfo);
            Dictionary<string, string> xmls = new Dictionary<string, string>();

            // return JsonResultData.Successed("");

            if (doc != null)
            {
                if (doc["MbrQuery"] != null)
                {
                    XmlNode node = doc["MbrQuery"]["Rows"]["Row"];
                    if (node != null && node.Name != null && node.FirstChild != null)
                    {
                        var MbrCardType = node["MbrCardType"].InnerText;
                        var MbrCardTypeName = node["MbrCardTypeName"].InnerText;
                        var MbrCardNo = node["MbrCardNo"].InnerText;
                        var GuestCName = node["GuestCName"].InnerText;
                        var ProfileId = node["ProfileId"].InnerText;
                        var Balance = Convert.ToDecimal(node["Balance"].InnerText); //储值余额 + 增值余额
                        var BaseAmtBalance = Convert.ToDecimal(node["BaseAmtBalance"].InnerText); //本金余额（储值余额）
                        var Incamount = Convert.ToDecimal(node["incamount"].InnerText); //增值余额

                        var DiscountMode = "";
                        var DiscountRate = "1";
                        var DisCountModeName = "";
                        var IsHasDiscount = false;

                        if (node["DiscountMode"] != null && node["DiscountRate"] != null)
                        {
                            DiscountMode = node["DiscountMode"].InnerText;
                            DiscountRate = node["DiscountRate"].InnerText;
                            IsHasDiscount = true;

                            var discounttype = GetMemberDisCountType(DiscountMode).FirstOrDefault();
                            if (discounttype != null)
                            {
                                DisCountModeName = discounttype.Name;
                            }
                        }

                        return JsonResultData.Successed(new MbrCardInfoModel
                        {
                            MbrCardType = MbrCardType,
                            MbrCardTypeName = MbrCardTypeName,
                            MbrCardNo = MbrCardNo,
                            GuestCName = GuestCName,
                            DiscountMode = DiscountMode,
                            DiscountRate = Convert.ToDecimal(DiscountRate),
                            DisCountModeName = DisCountModeName,
                            IsHasDiscount = IsHasDiscount,
                            ProFileID = ProfileId,
                            Balance = Balance,
                            BaseAmtBalance = BaseAmtBalance,
                            Incamount = Incamount
                        });
                    }
                    return JsonResultData.Failure("查询出错");
                }
                else if (doc["ErrorMessage"] != null)
                {
                    if (doc["ErrorMessage"]["Message"] != null)
                    {
                        return JsonResultData.Failure(doc["ErrorMessage"]["Message"].InnerText);
                    }
                }
            }
            return JsonResultData.Failure("查询出错");
        }

        //获取会员折扣方式
        /// <summary>
        /// 获取捷云会员折扣方式
        /// </summary>
        /// <param name="typecode">会员折扣方式代码</param>
        /// <returns></returns>
        public List<v_codeListPubModel> GetMemberDisCountType(string typecode = "")
        {
            var sql = "select* from v_codeListPub where typeCode = 77 and status = 1";
            if (!string.IsNullOrEmpty(typecode))
            {
                sql += $"and code='{typecode}'";
            }

            return _db.Database.SqlQuery<v_codeListPubModel>(sql).ToList();
        }

        /// <summary>
        /// 会员项目折扣计算
        /// </summary>
        /// <param name="mbrCardInfo">会员信息</param>
        /// <param name="ItemClassDiscount">会员项目大类折扣</param>
        /// <param name="ItemDiscount">会员项目折扣</param>
        /// <param name="OldDisCount">原折扣</param>
        /// <param name="IsHasItemClassDisCount">是否有会员项目大类折扣</param>
        /// <param name="IsHasItemDisCount">是否有会员项目折扣</param>
        ///  <param name="ItemIsDiscount">项目是否可以折扣</param>
        /// <returns>最终的折扣</returns>
        public decimal CalculateMemberItemDisCount(MbrCardInfoModel mbrCardInfo, decimal ItemClassDiscount, decimal ItemDiscount, decimal OldDisCount, bool IsHasItemClassDisCount, bool IsHasItemDisCount, bool ItemIsDiscount)
        {
            //照单折以外，所有的折扣方式都要验证是否可以打折，不可打折返回默认折扣

            //不使用会员折扣，
            if (mbrCardInfo == null)
            {
                //验证项目是否可以打折
                if (!ItemIsDiscount)
                {
                    return OldDisCount;
                }
                //有项目折扣，使用项目折扣；
                //没有项目折扣，使用项目大类折扣；
                // 没有项目大类折扣，使用原始折扣；
                return IsHasItemDisCount ? ItemDiscount : (IsHasItemClassDisCount ? ItemClassDiscount : OldDisCount);
            }

            //验证项目是否可以打折：照单折以外，所有的折扣方式都要验证是否可以打折，不可打折返回默认折扣
            if (!ItemIsDiscount && mbrCardInfo.MbrCardType != "4")
            {
                return OldDisCount;
            }

            //使用会员折扣
            //获取会员折扣
            decimal MemberDisCount = mbrCardInfo.DiscountRate;

            switch (mbrCardInfo.DiscountMode)
            {
                //孰低法(取最低)
                case "0":
                    //获取最低折扣
                    var list = new List<decimal>();
                    //有会员项目折扣
                    if (IsHasItemDisCount)
                        list.Add(ItemDiscount);
                    //有会员大类折扣
                    if (IsHasItemClassDisCount)
                        list.Add(ItemClassDiscount);
                    //只有会员折扣
                    list.Add(MemberDisCount);
                    //返回最小值
                    return list.Min();
                    break;
                //原价折
                case "1":
                    //优先级 会员项目折扣 >  会员项目大类折扣 > 会员折扣
                    if (IsHasItemDisCount)
                    {
                        return ItemDiscount;
                    }
                    else if (IsHasItemClassDisCount)
                    {
                        return ItemClassDiscount;
                    }
                    else
                    {
                        return MemberDisCount;
                    }
                    break;
                //折后折
                case "2":
                    //优先级 会员项目折扣 >  会员项目大类折扣 > 会员折扣
                    if (IsHasItemDisCount)
                    {
                        return ItemDiscount * MemberDisCount;
                    }
                    else if (IsHasItemClassDisCount)
                    {
                        return ItemClassDiscount * MemberDisCount;
                    }
                    else
                    {
                        return MemberDisCount * OldDisCount;
                    }
                    break;
                //会员价
                case "3":
                    //优先级 会员项目折扣 >  会员项目大类折扣 > 会员折扣
                    if (IsHasItemDisCount)
                    {
                        return ItemDiscount;
                    }
                    else if (IsHasItemClassDisCount)
                    {
                        return ItemClassDiscount;
                    }
                    else
                    {
                        return MemberDisCount;
                    }
                    break;
                //照单折
                case "4":
                    return MemberDisCount;
                    break;
                //员工价
                case "5":
                    return 1;
                    break;

                default:
                    return 1;
                    break;
            }
        }

        /// <summary>
        /// 会员大类折扣计算
        /// </summary>
        /// <param name="mbrCardInfo">会员信息</param>
        /// <param name="ItemClassDiscount">会员项目大类折扣</param>
        /// <param name="OldDisCount">原折扣</param>
        /// <param name="IsHasItemClassDisCount">是否有会员项目大类折扣</param>
        /// <returns>最终的折扣</returns>
        public decimal CalculateMemberItemClassDisCount(MbrCardInfoModel mbrCardInfo, decimal ItemClassDiscount, decimal OldDisCount, bool IsHasItemClassDisCount)
        {
            //不使用会员折扣，
            if (mbrCardInfo == null)
            {
                //有项目大类折扣，使用项目大类折扣；
                // 没有项目大类折扣，使用原始折扣；
                return IsHasItemClassDisCount ? ItemClassDiscount : OldDisCount;
            }

            //使用会员折扣

            //获取会员折扣
            decimal MemberDisCount = mbrCardInfo.DiscountRate;

            switch (mbrCardInfo.DiscountMode)
            {
                //孰低法(取最低)
                case "0":
                    //获取最低折扣
                    var list = new List<decimal>();
                    //有会员大类折扣
                    if (IsHasItemClassDisCount)
                        list.Add(ItemClassDiscount);
                    //只有会员折扣
                    list.Add(MemberDisCount);
                    //返回最小值
                    return list.Min();
                    break;
                //原价折
                case "1":
                    //优先级 会员项目大类折扣 > 会员折扣
                    if (IsHasItemClassDisCount)
                    {
                        return ItemClassDiscount;
                    }
                    else
                    {
                        return MemberDisCount;
                    }
                    break;
                //折后折
                case "2":
                    //优先级   会员项目大类折扣 > 会员折扣
                    if (IsHasItemClassDisCount)
                    {
                        return ItemClassDiscount * MemberDisCount;
                    }
                    else
                    {
                        return MemberDisCount * OldDisCount;
                    }
                    break;
                //会员价
                case "3":
                    //优先级 会员项目大类折扣 > 会员折扣
                    if (IsHasItemClassDisCount)
                    {
                        return ItemClassDiscount;
                    }
                    else
                    {
                        return MemberDisCount;
                    }
                    break;
                //照单折
                case "4":
                    return MemberDisCount;
                    break;
                //员工价
                case "5":
                    return 1;
                    break;

                default:
                    return 1;
                    break;
            }
        }

        //会员大类折扣记录的时间筛选
        /// <summary>
        /// 待筛选的时间记录
        /// </summary>
        /// <param name="posOnSales"></param>
        /// <returns></returns>
        public List<PosOnSale> MemberDisCountTimeFileter(List<PosOnSale> posOnSales)
        {
            string today = DateTime.Today.ToString("yyyy-MM-dd");
            Func<PosOnSale, bool> wherfunc = b =>
            {
                try
                {
                    b.StartTime = today + " " + b.StartTime;
                    b.EndTime = today + " " + b.EndTime;

                    //结束时间时候是否为次日
                    if (Convert.ToDateTime(b.StartTime) >= Convert.ToDateTime(b.EndTime))
                    {
                        b.EndTime = Convert.ToDateTime(b.EndTime).AddDays(1).ToString();
                    }
                    //时间筛选
                    if (DateTime.Now >= Convert.ToDateTime(b.StartTime) && DateTime.Now <= Convert.ToDateTime(b.EndTime))
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    return false;
                }
            };

            return posOnSales.Where(wherfunc).ToList();
        }

        /// <summary>
        /// 给xml增加业务信息之前的通用信息
        /// </summary>
        /// <param name="businessTitle">接口名称</param>
        /// <param name="xmlStrBuilder">xml生成器</param>
        private void AppendBeforeBusinessXmlStr(string businessTitle, StringBuilder xmlStrBuilder)
        {
            xmlStrBuilder.Append("<?xml version='1.0' encoding='gbk' ?>")
                       .Append("<RealOperate>")
                       .Append("<XType>JxdBSPms</XType>")
                       .Append("<OpType>").Append(businessTitle).Append("</OpType>");
        }

        /// <summary>
        /// 给xml增加业务信息之后的通用信息
        /// </summary>
        /// <param name="xmlStrBuilder">xml生成器</param>
        private void AppendAfterBusinessXmlStr(StringBuilder xmlStrBuilder)
        {
            xmlStrBuilder.Append("</RealOperate> ");
        }
    }
}