namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 查询预订列表结果集
    /// </summary>
    public class UpQueryResDetailResult
    {
        public string Hid { get; set; }
        public string ResNo { get; set; }
        public string Name { get; set; }
        public string ResCustName { get; set; }
        public string ResCustMobile { get; set; }
        public string GuestName { get; set; }
        public string GuestMobile { get; set; }
        public string RoomTypeName { get; set; }
        public string RoomQty { get; set; }
        public string RoomNo { get; set; }
        public string Rate { get; set; }
        public string ArrDate { get; set; }
        public string DepDate { get; set; }
        public string StatuName { get; set; }
        public string Cdate { get; set; }
        public string RegId { get; set; }
        public string ExtType { get; set; }
        public string CompanyName { get; set; }
        public string RateCode { get; set; }
        public string Bbf { get; set; }
        public string SourceName { get; set; }
        public string MbrCardNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public string CreditAmount { get; set; }
        /// <summary>
        /// 消费金额
        /// </summary>
        public string DebitAmount { get; set; }
        /// <summary>
        /// 余额金额（支付金额-消费金额）
        /// </summary>
        public string BalanceAmount { get; set; }
        /// <summary>
        /// 授权金额
        /// </summary>
        public string CardAuthAmount { get; set; }
        /// <summary>
        /// 是否团体
        /// </summary>
        public string Billtype { get; set; }
        /// <summary>
        /// 是否客情保密
        /// </summary>
        public byte IsCustemSecret { get; set; }
    }
}
