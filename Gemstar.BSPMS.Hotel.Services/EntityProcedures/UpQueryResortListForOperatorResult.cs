namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 查询可操作分店存储过程返回结果
    /// </summary>
    public class UpQueryResortListForOperatorResult
    {
        /// <summary>
        /// 酒店id
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 酒店名称
        /// </summary>
        public string Hname { get; set; }

        /// <summary>
        /// 酒店模块
        /// </summary>
        public string CateringServicesModule { get; set; }
    }
}
