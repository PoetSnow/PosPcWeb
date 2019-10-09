namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    /// <summary>
    /// 客账操作参数
    /// 存储过程up_resFolio_op需要的参数对象
    /// </summary>
    public class ResFolioOpPara
    {
        /// <summary>
        /// 可以传指定酒店代码，也可以传AUTO,，这样所有超过指写时间的都重新过租
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// checkout 清账并离店   out 迟付,只离店不结帕萨特 clear 只清账不离店 checkout_c 清账并离店检查   clear_c 只清账不离店的检查
        /// Recheckout 清账并离店  
        /// </summary>
        public string OpType { get; set; }
        /// <summary>
        /// 用逗号分开 clear 时，无意义
        /// </summary>
        public string RegIds { get; set; }
        /// <summary>
        /// out 时，无意义
        /// </summary>
        public string TransIds { get; set; }
        /// <summary>
        /// 操作员姓名
        /// </summary>
        public string InputUser { get; set; }
        /// <summary>
        /// 班次id
        /// </summary>
        public string Shiftid { get; set; }
        /// <summary>
        /// 迟付原因
        /// </summary>
        public string OutReason { get; set; }
    }
}
