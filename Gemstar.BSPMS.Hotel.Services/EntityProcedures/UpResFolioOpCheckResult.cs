namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 调用存储过程up_resFolio_op进行结账或清账检查返回的结果集对象
    /// </summary>
    public class UpResFolioOpCheckResult
    {
        /// <summary>
        /// 付款方式编码
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 付款方式名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 汇率
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 付款处理方式
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 需付款金额
        /// </summary>
        public decimal Balance { get; set; }
    }
}
