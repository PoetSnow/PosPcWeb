namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 查询客账信息结果集
    /// 存储过程up_queryResDetailsForCommon的结果
    /// </summary>
    public class UpQueryResDetailsForCommonResult
    {
        public string RegId { get; set; }
        public string RegIdDisplay { get; set; }
        public string RoomNo { get; set; }
        public string GuestName { get; set; }
        public string ArrDate { get; set; }
        public string StatusName { get; set; }
    }
}
