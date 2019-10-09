using Gemstar.BSPMS.Hotel.Services.PayManage;

namespace Gemstar.BSPMS.Hotel.Services.EF.PayManage
{
    /// <summary>
    /// 信用卡支付
    /// </summary>
    public class PayCreditService :PayBaseService
    {
        /// <summary>
        /// 输入卡号，有效期，把两个拼接后保存到folio的refno里面
        /// </summary>
        /// <param name="jsonStrPara">json格式的参数</param>
        /// <returns>支付号，即卡号和有效期的拼接字符串</returns>
        public override PayResult DoPayBeforeSaveFolio(string jsonStrPara)
        {
            var paraDic = GetParaDicFromJsonStr(jsonStrPara);
            return new PayResult { RefNo = string.Format("卡号:{0};有效期:{1}", paraDic["cardNo"], paraDic["expire"]), IsWaitPay = false };
        }
    }
}
