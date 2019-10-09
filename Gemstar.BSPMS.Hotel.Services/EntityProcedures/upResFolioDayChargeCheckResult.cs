namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 存储过程up_resFolio_dayCharge_check的结果集
    /// </summary>
    public class upResFolioDayChargeCheckResult
    {
        public string RegId { get; set; }
        public string RoomNo { get; set; }
        public string GuestName { get; set; }
        public string RoomTypeName { get; set; }
        /// <summary>
        /// 全日租价，根据此价格来计算全日租和半日租
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 收取的类型,取值：全日租，半日租
        /// </summary>
        public string Type { get; set; } 
        /// <summary>
        /// 收取的金额
        /// </summary>
        public decimal Amount { get; set; } 

    }
}
