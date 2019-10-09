using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Gemstar.BSPMS.Common.PayManage;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.PayManage;
using Gemstar.BSPMS.Hotel.Services.SMSSendManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 会员卡增值金额或返现金额付款服务
    /// </summary>
    public class PayMbrLargessService : PayBaseService
    {
        public PayMbrLargessService(DbHotelPmsContext pmsDb,string hid,string userName,string shiftId)
        {
            _pmsDb = pmsDb;
            _hid = hid;
            _username = userName;
            _shiftId = shiftId;
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
            var balanceType = "02";//赠送金额
            var transType = "02";//扣款
            var execResult = _pmsDb.Database.SqlQuery<upProfileCaInputResult>("exec up_profileCaInput @hid=@hid,@profileId=@profileId,@balanceType=@balanceType,@type=@type,@outLetCode=@outLetCode,@floatAmount=@floatAmount,@freeAmount=0,@invno=@invno,@inputUser=@inputUser,@remark=@remark,@refno=@refno,@itemid=@itemid,@shiftId=@shiftId"
                , new SqlParameter("@hid", _hid)
                , new SqlParameter("@profileId",profileId)
                ,new SqlParameter("@balanceType",balanceType)
                ,new SqlParameter("@type",transType)
                ,new SqlParameter("@outLetCode",outletCode??"")
                ,new SqlParameter("@floatAmount",-amount)
                ,new SqlParameter("@inputUser",_username??"")
                ,new SqlParameter("@invno",invno)
                ,new SqlParameter("@remark",remark)
                , new SqlParameter("@refno", regid)
                , new SqlParameter("@itemid", itemid)
                , new SqlParameter("@shiftId", _shiftId)
                ).Single();
            if (execResult.Success)
            {
                _profileId = profileId;
                _profileCaId = execResult.Data.ToString();
                //处理会员的储值余额和增值余额，会员卡号，将这些信息以备注的形式返写回账务表中
                var profileBalance = PayBaseService.GetProfileBalance(_pmsDb, _hid, profileId);
                remark = string.Format("[会员卡号:{0};储值余额:{1};增值余额:{2}]",
                    cardNo,
                    profileBalance.Balance.HasValue ? profileBalance.Balance.Value.ToString("0.00") : "0",
                    profileBalance.Free.HasValue ? profileBalance.Free.Value.ToString("0.00") : "0");
                return new PayResult { RefNo = execResult.Data, IsWaitPay = false, Remark = remark };
            }
            else
            {
                throw new ApplicationException(string.Format("会员增值金额扣款不成功，原因：{0}", execResult.Data));
            }
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
                    sendService.SendMbrConsume(_hid, _profileId, _profileCaId, para, smsLogService);
                }
                catch { }
            }
            return base.DoPayAfterSaveFolio(productType, payTransId, jsonStrPara);
        }
        private DbHotelPmsContext _pmsDb;
        private string _username;
        private string _shiftId;
        private string _hid;
        private string _profileId;
        private string _profileCaId;
    }
}
