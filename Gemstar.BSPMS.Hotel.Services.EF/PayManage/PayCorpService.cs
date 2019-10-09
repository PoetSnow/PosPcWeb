using System;
using System.Data.SqlClient;
using System.Linq;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;
using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 会员卡储值余额付款服务
    /// </summary>
    public class PayCorpService : PayBaseService
    {
        public PayCorpService(DbHotelPmsContext pmsDb,string hid,string userName)
        {
            _pmsDb = pmsDb;
            _username = userName;
            _hid = hid;
        }
        public override PayResult DoPayBeforeSaveFolio(string jsonStrPara)
        {
            var paraDic = GetParaDicFromJsonStr(jsonStrPara);
            var corpId = GetValueSafely(paraDic,"corpId");
            if (string.IsNullOrWhiteSpace(corpId))
            {
                throw new ApplicationException("请指定要挂账的合约单位");
            }
            var signPerson = GetValueSafely(paraDic,"signPerson");
            var amount = Convert.ToDecimal(GetValueSafely(paraDic,"amount","0"));
            var outletCode = GetValueSafely(paraDic,"outletCode");
            var invno = GetValueSafely(paraDic,"invno");
            var remark = GetValueSafely(paraDic,"remark");
            var regid = GetValueSafely(paraDic,"regid");
            var transType = "01";//挂账
            var execResult = _pmsDb.Database.SqlQuery<upProfileCaInputResult>("exec up_companyca_input @hid=@hid,@inputUser=@inputUser,@companyid=@companyid,@outletcode=@outletcode,@type=@type,@amount=@amount,@invno=@invno,@remark=@remark,@sign=@sign,@regid=@regid"
                , new SqlParameter("@hid", _hid)
                ,new SqlParameter("@inputUser", _username??"")
                ,new SqlParameter("@companyid", corpId??"")
                ,new SqlParameter("@outletcode", outletCode??"")
                ,new SqlParameter("@type", transType)
                ,new SqlParameter("@amount", amount)
                ,new SqlParameter("@invno", invno??"")
                ,new SqlParameter("@remark", remark??"")
                ,new SqlParameter("@sign", signPerson??"")
                ,new SqlParameter("@regid",regid ?? "")
                ).Single();
            if (execResult.Success)
            {
                return new PayResult { RefNo = execResult.Data, IsWaitPay = false };
            }else
            {
                throw new ApplicationException(string.Format("合约单位挂账不成功，原因：{0}", execResult.Data));
            }
        }
        private DbHotelPmsContext _pmsDb;
        private string _username;
        private string _hid;
    }
}
