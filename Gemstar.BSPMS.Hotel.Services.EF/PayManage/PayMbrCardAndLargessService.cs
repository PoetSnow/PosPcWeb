using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 会员卡储值+增值余额付款服务
    /// 根据另外两个系统参数来决定扣增值和储值的方式
    /// </summary>
    public class PayMbrCardAndLargessService : PayBaseService
    {
        public PayMbrCardAndLargessService(DbHotelPmsContext pmsDb,string hid,string userName,string shiftId)
        {
            _pmsDb = pmsDb;
            _username = userName;
            _shiftId = shiftId;
            _hid = hid;
        }
        public override PayResult DoPayBeforeSaveFolio(string jsonStrPara)
        {
            var paraDic = GetParaDicFromJsonStr(jsonStrPara);
            var cardNo = paraDic["cardNo"].ToString();
            var amount = Convert.ToDecimal(paraDic["amount"]);
            var outletCode = paraDic["outletCode"].ToString();
            var invno = paraDic["invno"].ToString();
            var remark = paraDic["remark"].ToString();
            var regid = paraDic["regid"].ToString();
            var itemid = paraDic["itemid"].ToString();
            var profileId = GetProfileIdByCardNo(_pmsDb,_hid, cardNo);
            if (string.IsNullOrWhiteSpace(profileId))
            {
                throw new ApplicationException("指定的会员卡号无效");
            }
            //取出系统参数中的储值增值扣款类型和扣款比例值
            var mbrCardAndLargessType = _pmsDb.PmsParas.SingleOrDefault(w => w.Hid == _hid && w.Code == "mbrCardAndLargessType");
            if(mbrCardAndLargessType == null)
            {
                throw new ApplicationException("没有找到mbrCardAndLargessType系统参数");
            }
            var typeValue = mbrCardAndLargessType.Value;
            PayResult payResult = null;
            if (typeValue == "1")
            {
                //1:会员增值优先,增值不足时再扣储值
                payResult = ProfileCaInputLargessFirst(profileId, outletCode, amount, invno, remark, regid, itemid);
            }else if(typeValue == "2")
            {
                //2:会员储值优先,储值不足时再扣增值
                payResult = ProfileCaInputBalanceFirst(profileId, outletCode, amount, invno, remark, regid, itemid);
            }else if (typeValue == "3")
            {
                //3:按参数中的比例扣减储值增值,不足时从另一账户扣除
                payResult = ProfileCaInputLargessAndBalanceAllow(profileId, outletCode, amount, invno, remark, regid, itemid);
            }else if(typeValue == "4")
            {
                //4:按参数中的比例扣减储值增值,储值余额不足时不允许扣款
                payResult = ProfileCaInputLargessAndBalanceCheckBalance(profileId, outletCode, amount, invno, remark, regid, itemid);
            }else if(typeValue == "5")
            {
                //5:按当前会员余额中的储值增值动态比例扣减
                payResult = ProfileCaInputLargessAndBalanceDynamicPercent(profileId, outletCode, amount, invno, remark, regid, itemid);
            }
            else
            {
                throw new ApplicationException("系统参数mbrCardAndLargessType的值不在支持范围内，只支持1，2，3，4中取值");
            }
            //统一处理会员的储值余额和增值余额，会员卡号，将这些信息以备注的形式返写回账务表中
            if(payResult != null)
            {
                var profileBalance = PayBaseService.GetProfileBalance(_pmsDb, _hid, profileId);
                remark = string.Format("[会员卡号:{0};储值余额:{1};增值余额:{2}]",
                    cardNo,
                    profileBalance.Balance.HasValue ? profileBalance.Balance.Value.ToString("0.00") : "0",
                    profileBalance.Free.HasValue ? profileBalance.Free.Value.ToString("0.00") : "0");
                if (payResult.IsMultiple)
                {
                    foreach(var r in payResult.MultipleItemInfos)
                    {
                        r.Remark = remark;
                    }
                }
                else
                {
                    payResult.Remark = remark;
                }
            }
            return payResult;
        }

        public override PayAfterResult DoPayAfterSaveFolio(PayProductType productType, string payTransId, string jsonStrPara)
        {
            //发送短信
            if (!string.IsNullOrWhiteSpace(_profileId) && !string.IsNullOrWhiteSpace(_profileCaId))
            {
                try
                {
                    var sendService = DependencyResolver.Current.GetService<ISMSSendService>();
                    var _sysParaService = DependencyResolver.Current.GetService<ISysParaService>();
                    var para = _sysParaService.GetSMSSendPara();
                    var smsLogService = DependencyResolver.Current.GetService<ISmsLogService>();
                    sendService.SendMbrConsumeViaChargeAndLargess(_hid, _profileId, _profileCaId, para, smsLogService);
                }
                catch { }
            }
            return base.DoPayAfterSaveFolio(productType, payTransId, jsonStrPara);
        }

        #region 基础的真实扣减操作

        /// <summary>
        /// 扣减会员增值金额
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="outletCode"></param>
        /// <param name="amount"></param>
        /// <param name="invno"></param>
        /// <param name="remark"></param>
        /// <param name="regid"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        private PayResult.PayItemInfo ProfileCaInputLargess(string hid,string profileId, string outletCode, decimal amount, string invno, string remark, string regid, string itemid)
        {
            var balanceType = "02";//赠送金额
            var transType = "02";//扣款
            var execResult = _pmsDb.Database.SqlQuery<upProfileCaInputResult>("exec up_profileCaInput @hid=@hid,@profileId=@profileId,@balanceType=@balanceType,@type=@type,@outLetCode=@outLetCode,@floatAmount=@floatAmount,@freeAmount=0,@invno=@invno,@inputUser=@inputUser,@remark=@remark,@refno=@refno,@itemid=@itemid,@shiftId=@shiftId"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@profileId", profileId)
                , new SqlParameter("@balanceType", balanceType)
                , new SqlParameter("@type", transType)
                , new SqlParameter("@outLetCode", outletCode ?? "")
                , new SqlParameter("@floatAmount", -amount)
                , new SqlParameter("@inputUser", _username ?? "")
                , new SqlParameter("@invno", invno)
                , new SqlParameter("@remark", remark)
                , new SqlParameter("@refno", regid)
                , new SqlParameter("@itemid", itemid)
                , new SqlParameter("@shiftId", _shiftId)
                ).Single();
            if (execResult.Success)
            {
                _profileId = profileId;
                if (string.IsNullOrWhiteSpace(_profileCaId))
                {
                    _profileCaId = execResult.Data.ToString();
                }
                else
                {
                    _profileCaId = string.Format("{0},{1}", _profileCaId, execResult.Data.ToString());
                }
                return new PayResult.PayItemInfo { RefNo = execResult.Data, Amount = amount, ItemId = string.Format("{0}1510", _hid) };
            }
            else
            {
                throw new ApplicationException(string.Format("会员增值金额扣款不成功，原因：{0}", execResult.Data));
            }
        }
        /// <summary>
        /// 扣减会员储值余额
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="outletCode"></param>
        /// <param name="amount"></param>
        /// <param name="invno"></param>
        /// <param name="remark"></param>
        /// <param name="regid"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        private PayResult.PayItemInfo ProfileCaInputBalance(string hid,string profileId, string outletCode, decimal amount, string invno, string remark, string regid, string itemid)
        {
            var balanceType = "01";//储值
            var transType = "02";//扣款
            var execResult = _pmsDb.Database.SqlQuery<upProfileCaInputResult>("exec up_profileCaInput @hid=@hid,@profileId=@profileId,@balanceType=@balanceType,@type=@type,@outLetCode=@outLetCode,@floatAmount=@floatAmount,@freeAmount=0,@invno=@invno,@inputUser=@inputUser,@remark=@remark,@refno=@refno,@itemid=@itemid,@shiftId=@shiftId"
                , new SqlParameter("@hid", hid)
                , new SqlParameter("@profileId", profileId)
                , new SqlParameter("@balanceType", balanceType)
                , new SqlParameter("@type", transType)
                , new SqlParameter("@outLetCode", outletCode ?? "")
                , new SqlParameter("@floatAmount", -amount)
                , new SqlParameter("@inputUser", _username ?? "")
                , new SqlParameter("@invno", invno)
                , new SqlParameter("@remark", remark)
                , new SqlParameter("@refno", regid)
                , new SqlParameter("@itemid", itemid)
                , new SqlParameter("@shiftId", _shiftId)
                ).Single();
            if (execResult.Success)
            {
                _profileId = profileId;
                if (string.IsNullOrWhiteSpace(_profileCaId))
                {
                    _profileCaId = execResult.Data.ToString();
                }
                else
                {
                    _profileCaId = string.Format("{0},{1}", _profileCaId, execResult.Data.ToString());
                }
                return new PayResult.PayItemInfo { RefNo = execResult.Data, Amount = amount, ItemId = string.Format("{0}1500", _hid) };
            }
            else
            {
                throw new ApplicationException(string.Format("会员储值金额扣款不成功，原因：{0}", execResult.Data));
            }
        }
        #endregion

        #region 1:会员增值优先,增值不足时再扣储值

        private PayResult ProfileCaInputLargessFirst(string profileId, string outletCode, decimal amount, string invno, string remark, string regid, string itemid)
        {
            var profileBalance = PayBaseService.GetProfileBalance(_pmsDb, _hid, profileId);
            var cardBalance = profileBalance.Balance ?? 0;
            var largessBalance = profileBalance.Free ?? 0;

            if (largessBalance + cardBalance < amount)
            {
                //如果总的余额不足，则直接返回错误
                throw new ApplicationException("会员余额不足");
            }
            //计算增值和储值账户应扣金额
            var largessAmount = amount;
            decimal cardAmount = 0;
            if (largessBalance < amount)
            {
                largessAmount = largessBalance;
                cardAmount = amount - largessBalance;
            }
            var payResult = new PayResult
            {
                IsWaitPay = false,
                IsMultiple = true,
                MultipleItemInfos = new List<PayResult.PayItemInfo>()
            };
            //开启事务
            using (var scope = new TransactionScope())
            {
                if (largessAmount != 0)
                {
                    var itemInfo = ProfileCaInputLargess(_hid,profileId, outletCode, largessAmount, invno, remark, regid, itemid);
                    payResult.MultipleItemInfos.Add(itemInfo);
                }
                if (cardAmount != 0)
                {
                    var itemInfo = ProfileCaInputBalance(_hid,profileId, outletCode, cardAmount, invno, remark, regid, itemid);
                    payResult.MultipleItemInfos.Add(itemInfo);
                }

                scope.Complete();
            }

            return payResult;
        }
        #endregion

        #region 2:会员储值优先,储值不足时再扣增值
        private PayResult ProfileCaInputBalanceFirst(string profileId, string outletCode, decimal amount, string invno, string remark, string regid, string itemid)
        {
            var profileBalance = PayBaseService.GetProfileBalance(_pmsDb, _hid, profileId);
            var cardBalance = profileBalance.Balance ?? 0;
            var largessBalance = profileBalance.Free ?? 0;

            if (largessBalance + cardBalance < amount)
            {
                //如果总的余额不足，则直接返回错误
                throw new ApplicationException("会员余额不足");
            }
            //计算增值和储值账户应扣金额
            var cardAmount = amount;
            decimal largessAmount = 0;
            if (cardBalance < amount)
            {
                cardAmount = cardBalance;
                largessAmount = amount - cardBalance;
            }
            var payResult = new PayResult
            {
                IsWaitPay = false,
                IsMultiple = true,
                MultipleItemInfos = new List<PayResult.PayItemInfo>()
            };
            //开启事务
            using (var scope = new TransactionScope())
            {
                if (cardAmount != 0)
                {
                    var itemInfo = ProfileCaInputBalance(_hid,profileId, outletCode, cardAmount, invno, remark, regid, itemid);
                    payResult.MultipleItemInfos.Add(itemInfo);
                }
                if (largessAmount != 0)
                {
                    var itemInfo = ProfileCaInputLargess(_hid,profileId, outletCode, largessAmount, invno, remark, regid, itemid);
                    payResult.MultipleItemInfos.Add(itemInfo);
                }

                scope.Complete();
            }
            return payResult;
        }
        #endregion

        #region 3:按参数中的比例扣减储值增值,不足时从另一账户扣除
        /// <summary>
        /// 按比例扣减会员增值和储值，并且某账户余额不足时，扣减另外一个账户
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="outletCode"></param>
        /// <param name="amount"></param>
        /// <param name="invno"></param>
        /// <param name="remark"></param>
        /// <param name="regid"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        private PayResult ProfileCaInputLargessAndBalanceAllow(string profileId, string outletCode, decimal amount, string invno, string remark, string regid, string itemid)
        {
            var profileBalance = PayBaseService.GetProfileBalance(_pmsDb, _hid, profileId);
            var cardBalance = profileBalance.Balance ?? 0;
            var largessBalance = profileBalance.Free ?? 0;

            if (largessBalance + cardBalance < amount)
            {
                //如果总的余额不足，则直接返回错误
                throw new ApplicationException("会员余额不足");
            }
            //取出系统参数中的增值扣除比例
            var largessPercentPara = _pmsDb.PmsParas.SingleOrDefault(w => w.Hid == _hid && w.Code == "mbrLargessPercent");
            if (largessPercentPara == null)
            {
                throw new ApplicationException("未找到系统参数mbrLargessPercent");
            }
            var largessPercentValue = largessPercentPara.Value;
            if (string.IsNullOrWhiteSpace(largessPercentValue))
            {
                throw new ApplicationException("系统参数mbrLargessPercent设置的值不正确，必须是n:m的形式");
            }
            var percentInfos = largessPercentValue.Split(':');
            if (percentInfos.Length != 2)
            {
                throw new ApplicationException("系统参数mbrLargessPercent设置的值不正确，必须是n:m的形式");
            }
            decimal largessPercent = 0, balancePercent = 0;
            if (!decimal.TryParse(percentInfos[0], out balancePercent))
            {
                throw new ApplicationException("系统参数mbrLargessPercent设置的值不正确，必须是n:m的形式,并且n,m必须是数字");
            }
            if (!decimal.TryParse(percentInfos[1],out largessPercent))
            {
                throw new ApplicationException("系统参数mbrLargessPercent设置的值不正确，必须是n:m的形式,并且n,m必须是数字");
            }
            //计算增值和储值账户应扣金额
            var percentLargessAmount = amount * largessPercent / (largessPercent + balancePercent);
            var percentCardAmount = amount - percentLargessAmount;
            var cardAmount = percentCardAmount;
            decimal largessAmount = percentLargessAmount;
            if (cardBalance < percentCardAmount)
            {
                cardAmount = cardBalance;
                largessAmount = amount - cardBalance;
            }
            else if (largessBalance < percentLargessAmount)
            {
                largessAmount = largessBalance;
                cardAmount = amount - largessBalance;
            }

            var payResult = new PayResult
            {
                IsWaitPay = false,
                IsMultiple = true,
                MultipleItemInfos = new List<PayResult.PayItemInfo>()
            };
            //开启事务
            using (var scope = new TransactionScope())
            {
                if (cardAmount != 0)
                {
                    var itemInfo = ProfileCaInputBalance(_hid,profileId, outletCode, cardAmount, invno, remark, regid, itemid);
                    payResult.MultipleItemInfos.Add(itemInfo);
                }
                if (largessAmount != 0)
                {
                    var itemInfo = ProfileCaInputLargess(_hid,profileId, outletCode, largessAmount, invno, remark, regid, itemid);
                    payResult.MultipleItemInfos.Add(itemInfo);
                }

                scope.Complete();
            }
            return payResult;
        }
        #endregion

        #region 4:按参数中的比例扣减储值增值,储值余额不足时不允许扣款
        /// <summary>
        /// 按比例扣减会员增值和储值，并且某账户余额不足时，扣减另外一个账户，但如果储值余额不足时不允许扣款
        /// </summary>
        /// <param name="profileId"></param>
        /// <param name="outletCode"></param>
        /// <param name="amount"></param>
        /// <param name="invno"></param>
        /// <param name="remark"></param>
        /// <param name="regid"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        private PayResult ProfileCaInputLargessAndBalanceCheckBalance(string profileId, string outletCode, decimal amount, string invno, string remark, string regid, string itemid)
        {
            var profileBalance = PayBaseService.GetProfileBalance(_pmsDb, _hid, profileId);
            var cardBalance = profileBalance.Balance ?? 0;
            var largessBalance = profileBalance.Free ?? 0;

            if (largessBalance + cardBalance < amount)
            {
                //如果总的余额不足，则直接返回错误
                throw new ApplicationException("会员余额不足");
            }
            //取出系统参数中的增值扣除比例
            var largessPercentPara = _pmsDb.PmsParas.SingleOrDefault(w => w.Hid == _hid && w.Code == "mbrLargessPercent");
            if (largessPercentPara == null)
            {
                throw new ApplicationException("未找到系统参数mbrLargessPercent");
            }
            var largessPercentValue = largessPercentPara.Value;
            if (string.IsNullOrWhiteSpace(largessPercentValue))
            {
                throw new ApplicationException("系统参数mbrLargessPercent设置的值不正确，必须是n:m的形式");
            }
            var percentInfos = largessPercentValue.Split(':');
            if (percentInfos.Length != 2)
            {
                throw new ApplicationException("系统参数mbrLargessPercent设置的值不正确，必须是n:m的形式");
            }
            decimal largessPercent = 0, balancePercent = 0;
            if (!decimal.TryParse(percentInfos[0], out balancePercent))
            {
                throw new ApplicationException("系统参数mbrLargessPercent设置的值不正确，必须是n:m的形式,并且n,m必须是数字");
            }
            if (!decimal.TryParse(percentInfos[1], out largessPercent))
            {
                throw new ApplicationException("系统参数mbrLargessPercent设置的值不正确，必须是n:m的形式,并且n,m必须是数字");
            }
            //计算增值和储值账户应扣金额
            var percentLargessAmount = amount * largessPercent / (largessPercent + balancePercent);
            var percentCardAmount = amount - percentLargessAmount;
            var cardAmount = percentCardAmount;
            decimal largessAmount = percentLargessAmount;
            if (largessBalance < percentLargessAmount)
            {
                largessAmount = largessBalance;
                cardAmount = amount - largessBalance;
            }
            if (cardBalance < cardAmount)
            {
                throw new ApplicationException(string.Format("会员储值余额不足，应扣{0:0.00},现余额{1:0.00}", cardAmount, cardBalance));
            }
            var payResult = new PayResult
            {
                IsWaitPay = false,
                IsMultiple = true,
                MultipleItemInfos = new List<PayResult.PayItemInfo>()
            };
            //开启事务
            using (var scope = new TransactionScope())
            {
                if (cardAmount != 0)
                {
                    var itemInfo = ProfileCaInputBalance(_hid,profileId, outletCode, cardAmount, invno, remark, regid, itemid);
                    payResult.MultipleItemInfos.Add(itemInfo);
                }
                if (largessAmount != 0)
                {
                    var itemInfo = ProfileCaInputLargess(_hid,profileId, outletCode, largessAmount, invno, remark, regid, itemid);
                    payResult.MultipleItemInfos.Add(itemInfo);
                }

                scope.Complete();
            }
            return payResult;
        }
        #endregion

        #region 5:按当前会员余额中的储值增值动态比例扣减

        private PayResult ProfileCaInputLargessAndBalanceDynamicPercent(string profileId, string outletCode, decimal amount, string invno, string remark, string regid, string itemid)
        {
            var profileBalance = PayBaseService.GetProfileBalance(_pmsDb,_hid,profileId);
            var cardBalance = profileBalance.Balance ?? 0;
            var largessBalance = profileBalance.Free ?? 0;

            if (largessBalance + cardBalance < amount)
            {
                //如果总的余额不足，则直接返回错误
                throw new ApplicationException("会员余额不足");
            }
            //计算增值和储值账户应扣金额
            var percentLargessAmount = amount * largessBalance / (largessBalance + cardBalance);
            var percentCardAmount = amount - percentLargessAmount;
            var cardAmount = percentCardAmount;
            decimal largessAmount = percentLargessAmount;
            if (largessBalance < percentLargessAmount)
            {
                largessAmount = largessBalance;
                cardAmount = amount - largessBalance;
            }
            if (cardBalance < cardAmount)
            {
                throw new ApplicationException(string.Format("会员储值余额不足，应扣{0:0.00},现余额{1:0.00}", cardAmount, cardBalance));
            }
            var payResult = new PayResult
            {
                IsWaitPay = false,
                IsMultiple = true,
                MultipleItemInfos = new List<PayResult.PayItemInfo>()
            };
            //开启事务
            using (var scope = new TransactionScope())
            {
                if (cardAmount != 0)
                {
                    var itemInfo = ProfileCaInputBalance(_hid,profileId, outletCode, cardAmount, invno, remark, regid, itemid);
                    payResult.MultipleItemInfos.Add(itemInfo);
                }
                if (largessAmount != 0)
                {
                    var itemInfo = ProfileCaInputLargess(_hid,profileId, outletCode, largessAmount, invno, remark, regid, itemid);
                    payResult.MultipleItemInfos.Add(itemInfo);
                }

                scope.Complete();
            }
            return payResult;
        }
        #endregion
        
        private DbHotelPmsContext _pmsDb;
        private string _username;
        private string _shiftId;
        private string _hid;
        private string _profileId;
        private string _profileCaId;
    }
}
