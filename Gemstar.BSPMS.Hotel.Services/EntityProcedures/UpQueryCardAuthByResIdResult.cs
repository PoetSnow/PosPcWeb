namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 根据订单id查询预授权信息结果对象
    /// 存储过程up_queryCardAuthByResId的结果
    /// </summary>
    public class UpQueryCardAuthByResIdResult
    {
        public string Id { get; set; }
        public string Regid { get; set; }
        public string RoomNo { get; set; }
        public string GuestName { get; set; }
        public string ItemName { get; set; }
        public string ItemId { get; set; }
        public string CardNo { get; set; }
        public string AuthNo { get; set; }
        public decimal? AuthAmount { get; set; }
        public decimal? BillAmount { get; set; }
        public string ValidDate { get; set; }
        public string Remark { get; set; }
        public string StatuName { get; set; }
        public byte Status { get; set; }
        public string CreateUser { get; set; }
        public string CreateDate { get; set; }
        public string CompleteUse { get; set; }
        public string CompleteDate { get; set; }
    }
}
