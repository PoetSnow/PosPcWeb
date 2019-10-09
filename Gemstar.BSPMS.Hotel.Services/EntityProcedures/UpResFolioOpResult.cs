namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 调用存储过程up_resFolio_op返回的结果集对象
    /// </summary>
    public class UpResFolioOpResult
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string regids { get; set; }
        /// <summary>
        /// 消费金额
        /// </summary>
        public decimal? amount_d { get; set; }
        /// <summary>
        /// 消费数量
        /// </summary>
        public int? count_d { get; set; }
        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal? amount_c { get; set; }
        /// <summary>
        /// 付款数量
        /// </summary>
        public int? count_c { get; set; }
        /// <summary>
        /// 房号
        /// </summary>
        public string roomno { get; set; }
    }
}
