using Gemstar.BSPMS.Common.Extensions;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.EF;
using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.MbrCardCenter;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.SystemManage;
using System.Collections.Generic;
using Gemstar.BSPMS.Common.Enumerator;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Common.Services.Enums;
using Gemstar.BSPMS.Common.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EF.PayManage;
using Gemstar.BSPMS.Common.PayManage;
using System.Web.Mvc;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;
using Gemstar.BSPMS.Common.Tools;

namespace Gemstar.BSPMS.Hotel.Services.EF.MbrCardCenter
{
    public class MbrCardService : CRUDService<MbrCard>, IMbrCardService
    {

        public MbrCardService(DbHotelPmsContext db, ICurrentInfo currentInfo, IHotelStatusService hotelStatusService) : base(db, db.MbrCards)
        {
            _pmsContext = db;
            _currentInfo = currentInfo;
            _hotelStatusService = hotelStatusService;
        }

        protected override MbrCard GetTById(string id)
        {
            return new MbrCard { Id = Guid.Parse(id) };
        }

        private DbHotelPmsContext _pmsContext;
        private ICurrentInfo _currentInfo;
        private IHotelStatusService _hotelStatusService;

        /// <summary>
        /// 检查会员卡号是否已存在
        /// </summary>
        /// <param name="cardNo">会员卡号</param>
        /// <param name="profileId">排除会员ID</param>
        /// <returns>true已存在,false不存在</returns>
        public bool RepeatCheck(string cardNo, string profileId = null)
        {
            //var result = _pmsContext.MbrCards.AsQueryable();
            List<MbrCard> result;
            if (string.IsNullOrWhiteSpace(profileId))
            {
                //result = result.Where(e => e.Hid == _currentInfo.HotelId && e.MbrCardNo == cardNo);
                result = GetMbrCard(cardNo: cardNo);
            }
            else
            {
                var guId = Guid.Parse(profileId);
                //result = result.Where(e => e.Hid == _currentInfo.HotelId && e.MbrCardNo == cardNo && e.Id != guId);
                result = GetMbrCard(cardNo: cardNo);
                result = result.Where(c => c.Id != guId).ToList();
            }
            return result.Any();
        }
        /// <summary>
        /// 检查感应卡号是否存在
        /// </summary>
        /// <param name="InductionCar"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public bool RepeatCheckCar(string InductionCar, string profileId = null)
        {
            //var result = _pmsContext.MbrCards.AsQueryable();
            List<MbrCard> result;
            if (string.IsNullOrWhiteSpace(profileId))
            {
                //result = result.Where(e => e.Hid == _currentInfo.HotelId && e.InductionCar == InductionCar);
                result = GetMbrCard(inductionCar: InductionCar);
            }
            else
            {
                var guId = Guid.Parse(profileId);
                //result = result.Where(e => e.Hid == _currentInfo.HotelId && e.InductionCar == InductionCar && e.Id != guId);
                result = GetMbrCard(inductionCar: InductionCar);
                result = result.Where(c => c.Id != guId).ToList();
            }
            return result.Any();
        }
        /// <summary>
        /// 检查会员卡号和感应号是否同时存在
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="InductionCar"></param>
        /// <returns></returns>
        public bool RepeatChecks(string cardNo, string InductionCar)
        {
            //return _pmsContext.MbrCards.Any(a => a.MbrCardNo == cardNo && a.InductionCar == InductionCar && a.Hid == _currentInfo.HotelId);
            return GetMbrCard(null, cardNo, InductionCar).Any();
        }
        /// <summary>
        /// 检查手机号是否存在
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public bool CheckMobile(string mobile, string id)
        {

            if (string.IsNullOrWhiteSpace(id))
            {
                //return _pmsContext.MbrCards.AsNoTracking().Any(w => w.Hid == _currentInfo.HotelId && w.Mobile == mobile.Trim());
                return GetMbrCard(mobile: mobile.Trim()).Any();
            }
            else
            {
                var gid = Guid.Parse(id);
                //var data = _pmsContext.MbrCards.AsNoTracking().FirstOrDefault(w => w.Hid == _currentInfo.HotelId && w.Id == gid);
                //var result = _pmsContext.MbrCards.AsNoTracking().FirstOrDefault(w => w.Hid == _currentInfo.HotelId && w.Mobile == mobile.Trim());
                var data = GetMbrCard(gid).FirstOrDefault();
                var result = GetMbrCard(mobile: mobile.Trim()).FirstOrDefault();
                if (data != null && result != null)
                {
                    return data.Mobile != result.Mobile;
                }
                return false;
            }


        }
        /// <summary>
        /// 检查证件号是否存在
        /// </summary>
        /// <param name="cerType"></param>
        /// <param name="cerid"></param>
        /// <returns></returns>
        public bool CheckCer(string cerType, string cerid)
        {
            if (string.IsNullOrWhiteSpace(cerid) || string.IsNullOrWhiteSpace(cerType))
                return false;
            //return _pmsContext.MbrCards.Any(w => w.CerType == cerType && w.Hid == _currentInfo.HotelId && w.Cerid == cerid);
            return GetMbrCard(cerType: cerType, cerid: cerid).Any();
        }
        /// <summary>
        /// 根据会员卡号获取会员信息
        /// </summary>
        /// <param name="mbrNo"></param>
        /// <param name="hid"></param>
        /// <returns></returns>
        public MbrCard GetMbrCar(string mbrNo, string hid)
        {
            //return _pmsContext.MbrCards.FirstOrDefault(w => w.MbrCardNo == mbrNo && w.Hid == hid);
            return GetMbrCard(null, mbrNo).FirstOrDefault();
        }
        /// <summary>
        /// 通过券号获取卡券信息
        /// </summary>
        /// <param name="ticketNo"></param>
        /// <returns></returns>
        public ProfileCard GetProfileCard(string ticketNo)
        {
            return _pmsContext.ProfileCards.FirstOrDefault(w => w.TicketNo == ticketNo);
        }
        /// <summary>
        /// 通过优惠券类型获取券信息
        /// </summary>
        /// <param name="ticketTypeId"></param>
        /// <param name="hid"></param>
        /// <returns></returns>
        public Coupon GetTicket(string ticketTypeId, string hid)
        {
            return _pmsContext.Coupons.FirstOrDefault(w => w.Id == ticketTypeId && w.Hid == hid);
        }        
        /// <summary>
        /// 通过现金券号获取金额
        /// </summary>
        /// <param name="ticketNo"></param>
        /// <returns></returns>
        public decimal? GetCouponMoney(string ticketNo)
        {
            string ticketTypeid = _pmsContext.ProfileCards.FirstOrDefault(w => w.TicketNo == ticketNo).TicketTypeid;
            return _pmsContext.Coupons.FirstOrDefault(w => w.Id == ticketTypeid).CouponMoney;
        }
        /// <summary>
        /// 查询酒店是否有本会员
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public bool IsHotelMbr(Guid profileId)
        {
            var entity = GetMbrCard(profileId).FirstOrDefault();
            return (entity != null);
        }
        /// <summary>
        /// 根据会员号得到账务信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<ProfileTrans> GetProfileTrans(Guid id)
        {
            return _pmsContext.ProfileTranses.Where(w => w.Profileid == id).OrderByDescending(o => o.TransBsnsdate);
        }
        /// <summary>
        /// 获取会员消费记录 排除反结
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<ProfileTrans> GetProfileTransRes(Guid id)
        {
            return _pmsContext.Database.SqlQuery<ProfileTrans>(@"
SELECT *  FROM  dbo.profileTrans WHERE refno IN(
SELECT refno FROM  dbo.profileTrans WHERE profileid = @profileid
GROUP BY refno HAVING COUNT(refno) = 1)  ORDER BY dbo.profileTrans.transDate DESC"
, new SqlParameter("@hid", _currentInfo.HotelId)
, new SqlParameter("@profileid", id)).ToList();

        }
        /// <summary>
        /// 获取会员账务信息
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <returns></returns>
        public MbrCardBalance GetCardBalance(Guid id)
        {
            return _pmsContext.MbrCardBalances.Find(id);
        }
        /// <summary>
        /// 获取集团ID
        /// </summary>
        /// <param name="hid"></param>
        /// <returns></returns>
        public string GetGrpid(string hid)
        {
            var entity = _pmsContext.PmsHotels.FirstOrDefault(c => c.Hid == hid).Grpid;
            if(entity != null && entity.Trim() != "")
            {
                return entity;
            }
            else
            {
                return hid;
            }
        }
        /// <summary>
        /// 更新会员实体单个项的值,包括（卡号，状态，审核状态，业务员，延期）
        /// </summary>
        /// <param name="attributeName">会员实体属性名</param>
        /// <param name="model">会员实体</param>
        /// <returns></returns>
        public JsonResultData UpdateSingle(string attributeName, MbrCard model)
        {
            //MbrCard entity = _pmsContext.MbrCards.Find(model.Id);
            MbrCard entity = GetMbrCard(model.Id).FirstOrDefault();
            if (entity == null)
            {
                return JsonResultData.Failure("错误信息，请关闭后再重试");
            }
            string oldValue = "";
            string newValue = "";
            string type = "";
            OpLogType opLog;
            string text = "";
            switch (attributeName)
            {
                case "MbrCardNo":
                    {
                        type = "换卡号";
                        opLog = OpLogType.会员换卡号;
                        text = string.Format("会员卡号：{0}=>{1}，感应卡号：{2}=>{3}", entity.MbrCardNo, model.MbrCardNo, entity.InductionCar, model.InductionCar);
                        oldValue = entity.MbrCardNo;
                        entity.MbrCardNo = model.MbrCardNo;
                        newValue = entity.MbrCardNo;
                        if (RepeatCheck(newValue, entity.Id.ToString()))
                        {
                            return JsonResultData.Failure("卡号已存在，请修改");
                        }

                        //会员卡号不变，改变感应卡号
                        if (oldValue == newValue)
                        {
                            oldValue = entity.InductionCar;
                            newValue = model.InductionCar;
                        }
                        entity.InductionCar = model.InductionCar;
                        if (RepeatCheckCar(entity.InductionCar, entity.Id.ToString()))
                            return JsonResultData.Failure("感应卡号已存在，请修改");

                    }
                    break;
                case "Status":
                    {
                        type = "变更卡状态";
                        opLog = OpLogType.会员变更卡状态;
                        MbrCardStatus enumStaus;
                        Enum.TryParse<MbrCardStatus>(entity.Status == null ? "" : entity.Status.ToString(), true, out enumStaus);
                        oldValue = enumStaus == 0 ? "" : enumStaus.GetDescription();
                        entity.Status = model.Status;
                        MbrCardStatus enumStausNew;
                        Enum.TryParse<MbrCardStatus>(entity.Status == null ? "" : entity.Status.ToString(), true, out enumStausNew);
                        newValue = enumStausNew == 0 ? "" : enumStausNew.GetDescription();
                        text = string.Format("卡状态：{0}=>{1}", enumStaus.GetDescription(), enumStausNew.GetDescription());
                    }
                    break;
                case "IsAudit":
                    {
                        type = "审核";
                        oldValue = entity.IsAudit == true ? "已审核" : "未审核";
                        entity.IsAudit = model.IsAudit;
                        newValue = entity.IsAudit == true ? "已审核" : "未审核";
                        opLog = OpLogType.会员审核;
                        text = string.Format("会员审核状态：{0}=>{1}", oldValue, newValue);
                    }
                    break;
                case "Sales":
                    {
                        type = "更换业务员";
                        oldValue = entity.Sales;
                        entity.Sales = model.Sales;
                        newValue = entity.Sales;
                        opLog = OpLogType.会员更改业务员;
                        text = string.Format("业务员：{0}=>{1}", oldValue, newValue);
                    }
                    break;
                case "ValidDate":
                    {
                        type = "延期";
                        oldValue = entity.ValidDate == null ? "" : entity.ValidDate.ToDateString();
                        entity.ValidDate = model.ValidDate;
                        newValue = entity.ValidDate == null ? "" : entity.ValidDate.ToDateString();
                        opLog = OpLogType.会员延期;
                        text = string.Format("有效期：{0}=>{1}", oldValue, newValue);
                    }
                    break;
                default:
                    return JsonResultData.Failure("错误信息，请关闭后再重试");
            }
            if (oldValue == newValue)
            {
                return JsonResultData.Successed();
            }
            _pmsContext.Entry(entity).State = EntityState.Modified;
            AddProfileLog(entity.Id, type, oldValue, newValue, model.Remark, 0);
            AddOperationLog(_currentInfo, opLog, text, entity.MbrCardNo);
            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }

        /// <summary>
        /// 变更卡类型
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="cardTypeId">新的会员卡类型ID</param>
        /// <param name="score">扣减积分数额</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public JsonResultData CardUpgrade(Guid id, string cardTypeId, int score, string remark)
        {
            score = (-Math.Abs(score));
            //MbrCard entity = _pmsContext.MbrCards.Find(id);
            MbrCard entity = GetMbrCard(id).FirstOrDefault();
            if (entity == null || string.IsNullOrWhiteSpace(cardTypeId))
            {
                return JsonResultData.Failure("错误信息，请关闭后再重试");
            }
            if (entity.MbrCardTypeid == cardTypeId)
            {
                return JsonResultData.Successed();
            }
            var newremark = string.IsNullOrWhiteSpace(remark) ? "升级卡类型扣除积分" : "升级卡类型扣除积分：" + remark;
            var caInputPara = new ProfileCaInputPara { ProfileId = entity.Id, AccountType = ProfileAccountType.AccountScore, CaType = ProfileCaType.ScoreUpdate, PayAmount = score, Remark = newremark };
            JsonResultData result = FinanceAction(caInputPara);
            if (!result.Success)
            {
                return result;
            }
            string oldValue = entity.MbrCardTypeid;
            entity.MbrCardTypeid = cardTypeId;
            var oldEntity = _pmsContext.MbrCardTypes.Where(m => m.Id == oldValue).FirstOrDefault();
            var newEntity = _pmsContext.MbrCardTypes.Where(m => m.Id == cardTypeId).FirstOrDefault();
            _pmsContext.Entry(entity).State = EntityState.Modified;
            AddProfileLog(entity.Id, "升级卡类型", oldEntity.Name, newEntity.Name, remark, score);
            AddOperationLog(_currentInfo, OpLogType.会员升级卡类型, string.Format("卡类型：{0}=>{1}", oldEntity.Name, newEntity.Name), entity.MbrCardNo);
            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }

        /// <summary>
        /// 卡费收取
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="payWayId">支付方式ID</param>
        /// <param name="payMoney">卡费金额</param>
        /// <param name="invNo">单号</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public JsonResultData CardFee(Guid id, string payWayId, decimal payMoney, string invNo, string remark, decimal? originPayAmount = null, string refno = null)
        {
            string splitStr = "&|&";
            if (!string.IsNullOrWhiteSpace(payWayId) && payWayId.Contains(splitStr))
            {
                payWayId = payWayId.Substring(0, payWayId.IndexOf(splitStr));
            }
            if (string.IsNullOrWhiteSpace(payWayId))
            {
                return JsonResultData.Failure("请选择付款方式");
            }
            var rechargePara = new ProfileCaInputPara { ProfileId = id, AccountType = ProfileAccountType.AccountCardFee, CaType = ProfileCaType.CardFee, PayWayId = payWayId, PayAmount = payMoney, LargessAmount = 0, InvNo = invNo, Remark = remark, OriginPayAmount = originPayAmount, RefNo = refno };
            //如果收取卡费成功，则更改禁用状态的会员为启用状态
            var result = FinanceAction(rechargePara);
            if (result.Success)
            {
                _pmsContext.Database.ExecuteSqlCommand("update profile set status = @normal where id=@profileId and status = @disable"
                    ,new SqlParameter("@normal",(byte)MbrCardStatus.Nomal)
                    , new SqlParameter("@profileId", id.ToString())
                    ,new SqlParameter("@disable",(byte)MbrCardStatus.Disabled)
                    );
            }
            return result;
        }
        /// <summary>
        /// 充值
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="payWayId">支付方式ID</param>
        /// <param name="payMoney">充值金额</param>
        /// <param name="sendMoney">赠送金额</param>
        /// <param name="invNo">单号</param>
        /// <param name="remark">备注</param>
        /// <param name="originPayAmount">原币金额</param>
        /// <param name="refno">外部挂帐的关键字</param>
        /// <returns></returns>
        public JsonResultData Recharge(Guid id, string payWayId, decimal payMoney, decimal sendMoney, string invNo, string remark, decimal? originPayAmount = null, string refno = null)
        {
            string splitStr = "&|&";
            if (!string.IsNullOrWhiteSpace(payWayId) && payWayId.Contains(splitStr))
            {
                payWayId = payWayId.Substring(0, payWayId.IndexOf(splitStr));
            }
            if (string.IsNullOrWhiteSpace(payWayId))
            {
                return JsonResultData.Failure("请选择付款方式");
            }
            //充值时允许直接充负数来调整充错的金额，所以参数传递时不需要转换为正数
            var rechargePara = new ProfileCaInputPara { ProfileId = id, AccountType = ProfileAccountType.AccountRecharge, CaType = ProfileCaType.Recharge, PayWayId = payWayId, PayAmount = payMoney, LargessAmount = sendMoney, InvNo = invNo, Remark = remark, OriginPayAmount = originPayAmount, RefNo = refno };
            return FinanceAction(rechargePara);
        }

        /// <summary>
        /// 扣款
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="accountType">会员账户类型（01储值，02赠送金额）</param>
        /// <param name="money">扣款金额</param>
        /// <param name="invNo">单号</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public JsonResultData SubtractMoney(Guid id, string accountType, decimal money, string invNo, string remark)
        {
            if (string.IsNullOrWhiteSpace(accountType) || (accountType != "01" && accountType != "02"))
            {
                return JsonResultData.Failure("请选择扣款类型");
            }
            var subtractPara = new ProfileCaInputPara { ProfileId = id, AccountType = accountType == "01" ? ProfileAccountType.AccountRecharge : ProfileAccountType.AccountLargess, CaType = ProfileCaType.Consume, PayAmount = -money, LargessAmount = 0, InvNo = invNo, Remark = remark };
            return FinanceAction(subtractPara);
        }

        /// <summary>
        /// 积分调整
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="accountType">会员账户类型（11积分，12业主分）</param>
        /// <param name="score">调整积分数量（正数调增，负数调减）</param>
        /// <param name="invNo">单号</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public JsonResultData ScoreAction(Guid id, string accountType, decimal score, string invNo, string remark)
        {
            if (string.IsNullOrWhiteSpace(accountType) || (accountType != "11" && accountType != "12"))
            {
                return JsonResultData.Failure("请选择账户类型");
            }
            if (score == 0)
            {
                return JsonResultData.Failure("请输入正确的积分");
            }
            var scorePara = new ProfileCaInputPara { ProfileId = id, AccountType = accountType == "11" ? ProfileAccountType.AccountScore : ProfileAccountType.AccountOwnerScore, CaType = ProfileCaType.ScoreUpdate, PayAmount = score, LargessAmount = 0, InvNo = invNo, Remark = remark };
            return FinanceAction(scorePara);
        }

        /// <summary>
        /// 积分兑换
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="itemId">兑换项目ID</param>
        /// <param name="score">扣除积分</param>
        /// <param name="invNo">单号</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public JsonResultData ScoreUse(Guid id, string itemId, int score, string invNo, string remark)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return JsonResultData.Failure("请选择兑换项目");
            }
            if (score < 0)
            {
                return JsonResultData.Failure("请输入正确的积分");
            }
            score = (-Math.Abs(score));
            //1.财务操作
            var newremark = string.IsNullOrWhiteSpace(remark) ? "积分兑换" : "积分兑换：" + remark;
            var scoreUsePara = new ProfileCaInputPara { ProfileId = id, AccountType = ProfileAccountType.AccountScore, CaType = ProfileCaType.ScoreUse, PayAmount = score, LargessAmount = 0, InvNo = invNo, Remark = newremark };
            JsonResultData result = FinanceAction(scoreUsePara);
            if (!result.Success)
            {
                return result;
            }
            //2.录入积分兑换明细表
            _pmsContext.ScoreUses.Add(new ScoreUse
            {
                Amount = 0,
                Hid = _currentInfo.HotelId,
                Id = Guid.NewGuid(),
                InputUser = _currentInfo.UserName,
                Itemid = "",
                Profileid = id,
                Score = score,
                Outletid = "01",
                ScoreItemid = itemId,
                TransDate = DateTime.Now,
                Remark = remark,
                Invno = invNo,
                TransBsnsDate = _hotelStatusService.GetBusinessDate(_currentInfo.HotelId),
                ShiftId = _currentInfo.ShiftId
            });
            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }

        /// <summary>
        /// 积分兑换-扣除金额
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <param name="itemId">兑换项目ID</param>
        /// <param name="score">扣除积分</param>
        /// <param name="money">扣除金额</param>
        /// <param name="payWayId">支付方式ID</param>
        /// <param name="invNo">单号</param>
        /// <param name="remark">备注</param>
        /// <param name="originMoney">原币金额</param>
        /// <returns></returns>
        public JsonResultData ScoreUse(Guid id, string itemId, int score, decimal money, string payWayId, string invNo, string remark, decimal? originMoney)
        {
            if (string.IsNullOrWhiteSpace(itemId))
            {
                return JsonResultData.Failure("请选择兑换项目");
            }
            if (score < 0)
            {
                return JsonResultData.Failure("请输入正确的积分");
            }
            if (money < 0 || originMoney < 0 || originMoney == null)
            {
                return JsonResultData.Failure("请输入正确的金额");
            }
            string splitStr = "&|&";
            if (!string.IsNullOrWhiteSpace(payWayId) && payWayId.Contains(splitStr))
            {
                payWayId = payWayId.Substring(0, payWayId.IndexOf(splitStr));
            }
            if (string.IsNullOrWhiteSpace(payWayId))
            {
                return JsonResultData.Failure("请选择付款方式");
            }
            score = (-Math.Abs(score));
            money = (Math.Abs(money));
            originMoney = (Math.Abs(originMoney.Value));


            var BalancesEntity = _pmsContext.MbrCardBalances.Where(c =>c.profileId == id).FirstOrDefault();
            if (BalancesEntity == null)
            {
                return JsonResultData.Failure("错误信息，请关闭后再重试");
            }
            if (BalancesEntity.Score < Math.Abs(score))
            {
                return JsonResultData.Failure("积分不足，请充值");
            }
            string itemAction = "mbrCard";//付款的处理动作	mbrCard	会员卡
            Item itemEntity = _pmsContext.Items.Find(payWayId);
            if (!string.IsNullOrWhiteSpace(itemEntity.Action) && itemEntity.Action.Trim() == itemAction)
            {
                if (BalancesEntity.Balance < Math.Abs(money))
                {
                    return JsonResultData.Failure("余额不足，请充值");
                }
            }
            //1.财务操作
            var newremark = string.IsNullOrWhiteSpace(remark) ? "积分加金额兑换" : "积分加金额兑换：" + remark;
            var scoreUsePara = new ProfileCaInputPara { ProfileId = id, AccountType = ProfileAccountType.AccountScore, CaType = ProfileCaType.ScoreUse, PayAmount = score, LargessAmount = 0, InvNo = invNo, Remark = newremark };
            JsonResultData result1 = FinanceAction(scoreUsePara);
            if (!result1.Success)
            {
                return result1;
            }
            if (!string.IsNullOrWhiteSpace(itemEntity.Action) && itemEntity.Action.Trim() == itemAction)
            {
                var consumePara = new ProfileCaInputPara { ProfileId = id, AccountType = ProfileAccountType.AccountRecharge, CaType = ProfileCaType.Consume, PayWayId = payWayId, PayAmount = money, LargessAmount = 0, InvNo = invNo, Remark = remark, OriginPayAmount = originMoney };
                JsonResultData result2 = FinanceAction(consumePara);
                if (!result2.Success)
                {
                    return result2;
                }
            }
            //2.录入积分兑换明细表
            _pmsContext.ScoreUses.Add(new ScoreUse
            {
                Amount = money,
                Hid = _currentInfo.HotelId,
                Id = Guid.NewGuid(),
                InputUser = _currentInfo.UserName,
                Itemid = payWayId,
                Profileid = id,
                Score = score,
                Outletid = "01",
                ScoreItemid = itemId,
                TransDate = DateTime.Now,
                Remark = remark,
                Invno = invNo,
                TransBsnsDate = _hotelStatusService.GetBusinessDate(_currentInfo.HotelId),
                ShiftId = _currentInfo.ShiftId,
                OriginAmount = originMoney
            });
            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }



        /// <summary>
        /// 会员财务操作
        /// </summary>
        /// <param name="para">会员账务参数实例</param>
        private JsonResultData FinanceAction(ProfileCaInputPara para)
        {
            try
            {
                //MbrCard entity = _pmsContext.MbrCards.Find(para.ProfileId);
                MbrCard entity = GetMbrCard(para.ProfileId).FirstOrDefault();
                if (entity == null)
                {
                    return JsonResultData.Failure("在当前酒店中未找到指定的会员记录");
                }
                var result = _pmsContext.Database.SqlQuery<upProfileCaInputResult>("exec up_profileCaInput @hid=@hid,@profileId=@profileId,@balanceType=@balanceType,@type=@type,@outLetCode=@outLetCode,@floatAmount=@floatAmount,@freeAmount=@freeAmount,@invno=@invno,@inputUser=@inputUser,@remark=@remark,@refno=@refno,@itemid=@itemid,@originFloatAmount=@originFloatAmount,@shiftId=@shiftId",
                    new SqlParameter("@hid", _currentInfo.HotelId),
                    new SqlParameter("@profileId", para.ProfileId),
                    new SqlParameter("@balanceType", para.AccountType.Code),
                    new SqlParameter("@type", para.CaType.Code),
                    new SqlParameter("@outLetCode", "01"),
                    new SqlParameter("@floatAmount", para.PayAmount),
                    new SqlParameter("@freeAmount", para.LargessAmount),
                    new SqlParameter("@invno", para.InvNo ??""),
                    new SqlParameter("@inputUser", _currentInfo.UserName),
                    new SqlParameter("@remark", para.Remark??""),
                    new SqlParameter("@refno", para.RefNo ?? ""),
                    new SqlParameter("@itemid", para.PayWayId??""),
                    new SqlParameter("@shiftId", _currentInfo.ShiftId),
                    (para.OriginPayAmount.HasValue ? new SqlParameter("@originFloatAmount", para.OriginPayAmount.Value) : new SqlParameter("@originFloatAmount", DBNull.Value))
                    ).FirstOrDefault();
                if (result.Success)
                {
                    return JsonResultData.Successed(result.Data);
                }
                return JsonResultData.Failure("会员账务处理失败");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }
        /// <summary>
        /// 添加会员变更记录
        /// </summary>
        /// <param name="profileId">会员ID</param>
        /// <param name="type">变更类型</param>
        /// <param name="oldValue">旧值</param>
        /// <param name="newValue">新值</param>
        /// <param name="remark">备注</param>
        /// <param name="score">积分</param>
        private void AddProfileLog(Guid profileId, string type, string oldValue, string newValue, string remark, int score)
        {
            _pmsContext.ProfileLogs.Add(new ProfileLog
            {
                Cdate = DateTime.Now,
                Hid = _currentInfo.HotelId,
                Id = Guid.NewGuid(),
                InputUser = _currentInfo.UserName,
                New = string.IsNullOrWhiteSpace(newValue) ? "" : newValue,
                Old = string.IsNullOrWhiteSpace(oldValue) ? "" : oldValue,
                Profileid = profileId,
                Type = type,
                Remark = remark,
                Score = score,
                Grpid = _currentInfo.GroupId
            });
        }

        /// <summary>
        /// 会员列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="nameOrMobile">会员名称或手机号码</param>
        /// <returns></returns>
        public IQueryable<MbrCard> List(string hid, string nameOrMobile)
        {
            DateTime nowDate = DateTime.Now;
            //return GetMbrCard().AsQueryable().Where(c => c.Status == 1 && c.IsAudit == true && c.ValidDate > nowDate && (c.GuestName == nameOrMobile || c.Mobile == nameOrMobile || c.MbrCardNo == nameOrMobile || c.InductionCar == nameOrMobile));
            return _pmsContext.Database.SqlQuery<MbrCard>(@"SELECT p.* FROM dbo.profile p LEFT JOIN dbo.pmsHotel b ON p.hid=b.hid WHERE (b.grpid = @grpid OR b.hid=@grpid) AND  p.status=1 AND p.isAudit= 1 AND p.validDate > GETDATE()
                AND(p.guestName = @name OR p.mobile= @name OR p.mbrCardNo = @name OR p.inductionCar = @name)"
                , new SqlParameter("@grpid", _currentInfo.GroupHotelId)
                , new SqlParameter("@name", nameOrMobile)
            ).AsQueryable();
            //return _pmsContext.MbrCards.Where(c => c.Hid == hid && c.Status == 1 && c.IsAudit == true && c.ValidDate > nowDate && (c.GuestName == nameOrMobile || c.Mobile == nameOrMobile || c.MbrCardNo == nameOrMobile || c.InductionCar == nameOrMobile));

            List<string> hids = new List<string>();
            hids.Add(hid);

            var grpid = _pmsContext.PmsHotels.AsNoTracking().Where(c => c.Hid == hid).Select(c => c.Grpid).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(grpid))
            {
                hids = _pmsContext.PmsHotels.AsNoTracking().Where(c => c.Grpid == grpid).Select(c => c.Hid).ToList();
            }

            return _pmsContext.MbrCards.AsNoTracking().Where(c => hids.Contains(c.Hid) && c.Status == 1 && c.IsAudit == true && c.ValidDate > nowDate && (c.GuestName == nameOrMobile || c.Mobile == nameOrMobile || c.MbrCardNo == nameOrMobile || c.InductionCar == nameOrMobile));
        }

        /// <summary>
        /// 业主列表
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="nameOrMobile">会员名称或手机号码</param>
        /// <returns></returns>
        public List<MbrCard> ListforOwner(string hid,string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                keyword = "";
            }
            //DateTime nowDate = DateTime.Now;
            //var result = _pmsContext.MbrCards.Where(c => c.Hid == hid && c.Status == 1 && c.IsAudit == true && c.ValidDate > nowDate && c.IsOwner == true).ToList();
            return _pmsContext.Database.SqlQuery<MbrCard>(@"SELECT p.* FROM dbo.profile p LEFT JOIN dbo.pmsHotel b ON p.hid=b.hid WHERE (b.grpid = @grpid OR b.hid=@grpid) AND  p.status=1 AND p.isAudit= 1 AND p.validDate > GETDATE()
                AND p.isOwner = 1 and p.guestName like '%'+@keyword+'%'"
                , new SqlParameter("@grpid", _currentInfo.GroupHotelId)
                , new SqlParameter("@keyword", keyword)
            ).ToList();
            //if (!string.IsNullOrWhiteSpace(keyword))
            //{
            //    result = result.Where(w => (w.GuestName.Contains(keyword))).ToList();
            //}
            //return result;
        }
        /// <summary>
        /// 批量延期
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public JsonResultData DelayValidDate(string id, DateTime time)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var mctype = _pmsContext.MbrCards.Find(Guid.Parse(ids[i]));
                    AddProfileLog(Guid.Parse(ids[i]), "延期", mctype.ValidDate.ToDateString(), time.ToString("yyyy-MM-dd"), "批量延期", 0);
                    AddOperationLog(_currentInfo, OpLogType.会员延期, string.Format("会员有效期：{0}=>{1}", mctype.ValidDate.ToDateString(), time.ToString("yyyy-MM-dd")), mctype.MbrCardNo);
                    _pmsContext.Entry(mctype).State = EntityState.Modified;
                    mctype.ValidDate = time;
                }
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("保存成功");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure("保存失败");
                throw;
            }
        }
        /// <summary>
        /// 批量更换业务员
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sale"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public JsonResultData UpdateSales(string id, string sale, string remark)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var mctype = _pmsContext.MbrCards.Find(Guid.Parse(ids[i]));
                    AddOperationLog(_currentInfo, OpLogType.会员更改业务员, string.Format("业务员：{0}=>{1}", mctype.Sales, sale), mctype.MbrCardNo);
                    _pmsContext.Entry(mctype).State = EntityState.Modified;
                    mctype.Sales = sale; mctype.Remark = remark;
                    AddProfileLog(Guid.Parse(ids[i]), "更换业务员", mctype.Sales, sale, remark, 0);
                }
                _pmsContext.SaveChanges();
                return JsonResultData.Successed("保存成功");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure("保存失败");
                throw;
            }
        }

        /// <summary>
        /// 批量积分调整
        /// </summary>
        /// <param name="id"></param>
        /// <param name="score"></param>
        /// <param name="accounttype"></param>
        /// <param name="invno"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public JsonResultData UpdateScore(string id, string score, string accounttype, string invno, string remark)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var scorePara = new ProfileCaInputPara { ProfileId = Guid.Parse(ids[i]), AccountType = accounttype == ProfileAccountType.AccountOwnerScore.Code ? ProfileAccountType.AccountOwnerScore : ProfileAccountType.AccountScore, CaType = ProfileCaType.ScoreUpdate, PayAmount = decimal.Parse(score), LargessAmount = 0, InvNo = invno, Remark = remark };
                    FinanceAction(scorePara);
                }
                return JsonResultData.Successed("保存成功");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure("保存失败");
                throw;
            }
        }
        public JsonResultData GiveCoupons(string id, string ticketTypeid, int number,string remarks)
        {
            try
            {
                var ids = id.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    for(int j = 0;j < number;j ++)
                    {
                        GiveCoupons(Guid.Parse(ids[i]),ticketTypeid,remarks);
                    }
                }
                return JsonResultData.Successed("发放优惠券成功");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure("发放优惠券失败");
                throw;
            }
        }
        private JsonResultData GiveCoupons(Guid profileId,string ticketTypeid,string remarks)
        {
            try
            {
                //MbrCard entity = _pmsContext.MbrCards.Find(profileId);
                MbrCard entity = GetMbrCard(profileId).FirstOrDefault();
                if (entity == null)
                {
                    return JsonResultData.Failure("错误信息，请关闭后再重试");
                }
                var result = _pmsContext.Database.SqlQuery<upProfileCaInputResult>("EXEC up_profileCoupon @profileId=@profileId, @ticketTypeid = @ticketTypeid,@remarks=@remarks, @hid = @hid,@isUse=@isUse, @inputUser = @inputUser, @shiftId = @shiftId",
                    new SqlParameter("@profileId", profileId),
                    new SqlParameter("@ticketTypeid", ticketTypeid),
                    new SqlParameter("@remarks", remarks),
                    new SqlParameter("@hid", (_currentInfo.HotelId)),
                    new SqlParameter("@isUse", "1"),
                    new SqlParameter("@inputUser", _currentInfo.UserName),
                    new SqlParameter("@shiftId", _currentInfo.ShiftId)
                    ).FirstOrDefault();
                if (result.Success)
                {
                    return JsonResultData.Successed(result.Data);
                }
                return JsonResultData.Failure("发放优惠券失败");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure(ex);
            }
        }        
        /// <summary>
        /// 根据主键ID获取会员信息
        /// </summary>
        /// <param name="hid">酒店ID</param>
        /// <param name="id">会员主键ID</param>
        /// <returns></returns>
        public JsonResultData GetMbrCardInfo(string hid, Guid id)
        {
            DateTime nowDate = DateTime.Now;

            //var entity = _pmsContext.MbrCards.Where(c => c.Hid == hid && c.Id == id && c.Status == 1 && c.IsAudit == true && c.ValidDate > nowDate).AsNoTracking().FirstOrDefault();
            var entity = GetMbrCard(id).Where(c => c.Status == 1 && c.IsAudit == true && c.ValidDate > nowDate).AsQueryable().AsNoTracking().FirstOrDefault();
            if (entity != null)
            {
                string mbrCardTypeid = entity.MbrCardTypeid;
                if (!string.IsNullOrWhiteSpace(mbrCardTypeid))
                {
                    string rateCodeid = _pmsContext.MbrCardTypes.Where(c =>  c.Id == mbrCardTypeid).Select(c => c.RateCodeid).AsNoTracking().FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(rateCodeid))
                    {
                        if (_pmsContext.Rates.Where(c => c.Id == rateCodeid && c.Status == EntityStatus.启用 && c.BeginDate < nowDate && c.EndDate > nowDate).AsNoTracking().Any())
                        {
                            var resultNewEntity = new
                            {
                                RateCodeid = rateCodeid,
                                GuestName = entity.GuestName,
                                Gender = entity.Gender,
                                CerType = entity.CerType,
                                Interest = entity.Interest,
                                Mobile = entity.Mobile,
                                Cerid = entity.Cerid,
                                CarNo = entity.CarNo,
                                City = entity.City,
                                Email = entity.Email,
                                Birthday = entity.Birthday.ToDateString(),
                                Address = entity.Address,
                                Remark = entity.Remark,
                            };
                            return JsonResultData.Successed(resultNewEntity);
                        }
                    }
                }
                var resultEntity = new
                {
                    RateCodeid = "",
                    GuestName = entity.GuestName,
                    Gender = entity.Gender,
                    CerType = entity.CerType,
                    Interest = entity.Interest,
                    Mobile = entity.Mobile,
                    Cerid = entity.Cerid,
                    CarNo = entity.CarNo,
                    City = entity.City,
                    Email = entity.Email,
                    Birthday = entity.Birthday.ToDateString(),
                    Address = entity.Address,
                    Remark = entity.Remark,
                };
                return JsonResultData.Successed(resultEntity);
            }
            return JsonResultData.Successed(null);
        }
        public JsonResultData UpdateProfileCa(string hid, Guid id, string itemid, string remark)
        {
            var pro = _pmsContext.ProfileCas.FirstOrDefault(f => f.Id == id);
            if (pro == null)
                return JsonResultData.Failure("该交易记录不存在");
            _pmsContext.Entry(pro).State = EntityState.Modified;
            pro.Itemid = itemid;
            pro.Remark = remark;
            _pmsContext.SaveChanges();
            return JsonResultData.Successed();
        }

        /// <summary>
        /// 会员充值记录退款
        /// </summary>
        /// <param name="para">会员充值退款参数，具体字段含义见参数类字段说明</param>
        /// <returns>充值记录退款结果</returns>
        public JsonResultData Refund(RechargeRefundPara para)
        {
            var checkResult = _pmsContext.Database.SqlQuery<UpProfilecaRefundCheckResult>("exec up_profileca_refundCheck @hid=@hid,@profileCaId=@profileCaId", new SqlParameter("@hid", para.Hid), new SqlParameter("@profileCaId", para.ProfileCaId)).ToList();
            if (checkResult.Count < 1)
            {
                return JsonResultData.Failure("指定的充值记录不存在");
            }
            var checkObj = checkResult[0];
            if (checkObj.IsRefunded)
            {
                return JsonResultData.Failure("指定的充值记录已经退款，不能再次退款");
            }
            if (!checkObj.Amount.HasValue || checkObj.Amount.Value <= 0)
            {
                return JsonResultData.Failure("指定的充值记录金额小于等于0，不能退款");
            }
            if (checkObj.Action == "WxBarcode" || checkObj.Action == "WxQrcode" || checkObj.Action == "AliBarcode" || checkObj.Action == "AliQrcode")
            {
                var transId = Guid.Parse(checkObj.Refno).ToString("N");
                if (checkObj.Action == "AliBarcode" || checkObj.Action == "AliQrcode")
                {
                    var alipayParas = PayServiceBuilder.GetAliPayConfigPara(para.CommonPayParas, para.HotelPayParas, para.IsEnvTest);
                    var alipayRefundDirectly = new PayAliRefundDirectly(alipayParas, para.PayLogService, _pmsContext, para.Hid, para.UserName);
                    //退款失败会以异常的形式抛出，所以此处需要捕获一下异常
                    try
                    {
                        var refundResult = alipayRefundDirectly.DoRefund(PayProductType.MbrRecharge, transId, checkObj.Id.ToString(), checkObj.Amount.Value, "充值退款");
                    }
                    catch (Exception ex)
                    {
                        return JsonResultData.Failure(ex);
                    }
                }
                else if (checkObj.Action == "WxBarcode" || checkObj.Action == "WxQrcode")
                {
                    var wxParas = PayServiceBuilder.GetWxPayConfigPara(para.CommonPayParas, para.HotelPayParas, para.HotelName);
                    var wxRefundDirectly = new PayWxRefundDirectly(wxParas, para.PayLogService, _pmsContext, para.Hid);
                    //退款失败会以异常的形式抛出，所以此处需要捕获一下异常
                    try
                    {
                        var refundResult = wxRefundDirectly.DoRefund(PayProductType.MbrRecharge, transId, checkObj.Id.ToString(), checkObj.Amount.ToString(), checkObj.Amount.ToString(), para.UserName);
                    }
                    catch (Exception ex)
                    {
                        return JsonResultData.Failure(ex);
                    }
                }
            }
            //计算原充值金额的增值金额，以便把增值金额也扣减掉
            decimal sendMoney = 0;
            if (checkObj.BalanceType == "01")
            {
                //只有在是充值储值金额账户的记录，才需要计算增值金额
                sendMoney = para.ChargeFreeService.GetSendMoney(para.Hid, checkObj.MbrCardTypeid, checkObj.Amount.Value);
            }
            //增加一笔负数进行冲减余额
            var writeOffPara = new ProfileCaInputPara { ProfileId = checkObj.Profileid, AccountType = ProfileAccountType.AccountFromCode(checkObj.BalanceType), CaType = ProfileCaType.CaTypeFromCode(checkObj.Type), PayWayId = checkObj.ItemId, PayAmount = -checkObj.Amount.Value, LargessAmount = -sendMoney,OriginPayAmount = -checkObj.originAmount.Value, Remark = string.Format("会员充值{0}的退款", checkObj.transDate) };
            var writeOffResult = FinanceAction(writeOffPara);
            if (!writeOffResult.Success)
            {
                return JsonResultData.Failure(writeOffResult.Data);
            }
            //更改充值记录的状态为已退款
            _pmsContext.Database.ExecuteSqlCommand("update profileCa set IsRefunded = 1,remark=isnull(remark,'')+@addremark where hid = @hid and id = @profileCaId ", new SqlParameter("@hid", para.Hid), new SqlParameter("@profileCaId", para.ProfileCaId), new SqlParameter("@addremark", "(" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + ")已退"));
            return JsonResultData.Successed();
        } 
        /// <summary>
        /// 获取会员
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cardNo"></param>
        /// <param name="inductionCar"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public List<MbrCard> GetMbrCard(Guid? id=null,string cardNo="",string inductionCar="",string mobile="",string cerType ="",string cerid="")
        {
            if(id == null)
            {
                return _pmsContext.Database.SqlQuery<MbrCard>(@"EXEC dbo.up_query_mbrCard @hid = @hid,
                @cardNo = @cardNo,
                @inductionCar =@inductionCar,
                @mobile = @mobile,
                @cerType=@cerType,
                @cerid=@cerid
                "
                , new SqlParameter("@hid",_currentInfo.HotelId)
                , new SqlParameter("@cardNo", cardNo)
                , new SqlParameter("@inductionCar", inductionCar)
                , new SqlParameter("@mobile", mobile)
                , new SqlParameter("@cerType", cerType)
                , new SqlParameter("@cerid", cerid)
            ).ToList();
            }
            else
            {
                return _pmsContext.Database.SqlQuery<MbrCard>(@"EXEC dbo.up_query_mbrCard @hid = @hid,
                @profileId = @profileid,
                @cardNo = @cardNo,
                @inductionCar =@inductionCar,
                @mobile = @mobile,
                @cerType=@cerType,
                @cerid=@cerid
                "
                , new SqlParameter("@hid", _currentInfo.HotelId)
                , new SqlParameter("@profileid", id)
                , new SqlParameter("@cardNo", cardNo)
                , new SqlParameter("@inductionCar", inductionCar)
                , new SqlParameter("@mobile", mobile)
                , new SqlParameter("@cerType", cerType)
                , new SqlParameter("@cerid", cerid)
            ).ToList();
            }
        }
        /// <summary>
        /// 发送会员营销短信
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="mobiles"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public JsonResultData SendMarketSms(string ids, string mobiles, string content)
        {
            try
            {
                //检测酒店是否有启用短信模块，没有则直接返回
                var smsInfo = _pmsContext.Database.SqlQuery<UpGetSmsInfoResult>("exec up_GetSmsInfo @hid=@hid", new SqlParameter("@hid", _currentInfo.HotelId ?? "")).SingleOrDefault();
                if (smsInfo == null || smsInfo.Enable != "1")
                {
                    return JsonResultData.Failure("酒店没有启用短信模块");
                }
                var idArr = ids.Split(',');
                for (int i = 0; i < idArr.Length; i++)
                {
                    var id = Guid.Parse(idArr[i]);
                    var entity = _pmsContext.MbrCards.Where(m=>m.Id == id).FirstOrDefault();
                    entity.MarketSmsDate = DateTime.Now;
                    _pmsContext.Entry(entity).State = EntityState.Modified;
                }
                _pmsContext.SaveChanges();
                var sendService = DependencyResolver.Current.GetService<ISMSSendService>();
                var _sysParaService = DependencyResolver.Current.GetService<ISysParaService>();
                var para = _sysParaService.GetSMSSendPara();
                var smsLogService = DependencyResolver.Current.GetService<ISmsLogService>();
                var hotelenty = _pmsContext.PmsHotels.Where(c => c.Hid == _currentInfo.HotelId).SingleOrDefault();
                string hotelName = "";
                if (hotelenty != null && !string.IsNullOrEmpty(hotelenty.Hotelshortname))
                {
                    hotelName = hotelenty.Hotelshortname;
                }
                else
                {
                    var _currentInfo = DependencyResolver.Current.GetService<ICurrentInfo>();
                    hotelName = _currentInfo.HotelName;
                }
                SMSSendParaCommonSms sms = new SMSSendParaCommonSms
                {
                    Mobile = mobiles,
                    UserName = smsInfo.UserName,
                    Password = smsInfo.Password,
                    Content = content,
                    HotelName = hotelName
                };
                sendService.CommonSendSms(sms, para, smsLogService);
                return JsonResultData.Successed("");
            }
            catch (Exception ex)
            {
                return JsonResultData.Failure("发送营销短信失败");
                throw;
            }
        }
        #region 操作日志
        /// <summary>
        /// 添加操作日志
        /// </summary>
        /// <param name="currentInfo">登录信息</param>
        /// <param name="type">日志类型</param>
        /// <param name="text">内容</param>
        /// <param name="keys">关键字</param>
        public void AddOperationLog(ICurrentInfo currentInfo, OpLogType type, string text, string keys)
        {
            _pmsContext.OpLogs.Add(new OpLog
            {
                CDate = DateTime.Now,
                Hid = currentInfo.HotelId,
                CUser = currentInfo.UserName,
                Ip = UrlHelperExtension.GetRemoteClientIPAddress(),
                XType = type.ToString(),
                CText = (text.Length > 4000 ? text.Substring(0, 4000) : text),
                Keys = keys,
            });
        }
        #endregion

        /// <summary>
        /// 根据指定的会员 获取会员卡类型名称
        /// </summary>
        /// <param name="id">会员ID</param>
        /// <returns></returns>
        public string GetMbrCardTypeNameById(Guid id)
        {
            string sql = @"select top 1 b.name from profile a
                            inner join mbrCardType b on a.mbrCardTypeid = b.id
                            where a.id = @id";
            return _pmsContext.Database.SqlQuery<string>(sql, new SqlParameter("@id", id)).FirstOrDefault();
        }
    }
}
