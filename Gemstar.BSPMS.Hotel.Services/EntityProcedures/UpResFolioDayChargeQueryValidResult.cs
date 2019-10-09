namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 查询有效的当前营业日加收的日租半日租记录，用于记录日租半日租日志
    /// 存储过程up_resFolio_dayCharge_queryValid的结果集
    /// </summary>
    public class UpResFolioDayChargeQueryValidResult
    {
        /// <summary>
        /// 账号
        /// </summary>
        public int regidWithoutHid { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string roomNo { get; set; }
        /// <summary>
        /// 日租半日租名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal? amount { get; set; }
    }
}
