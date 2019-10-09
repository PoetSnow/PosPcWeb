using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 银联卡支付服务
    /// </summary>
    public class PayBankCardService : PayBaseService
    {
        /// <summary>
        /// 输入卡号，保存到folio的refno里面
        /// </summary>
        /// <param name="jsonStrPara"></param>
        /// <returns></returns>
        public override PayResult DoPayBeforeSaveFolio(string jsonStrPara)
        {
            var paraDic = GetParaDicFromJsonStr(jsonStrPara);
            return new PayResult { RefNo = string.Format("卡号:{0}", paraDic["cardNo"]), IsWaitPay = false };
        }
    }
}
