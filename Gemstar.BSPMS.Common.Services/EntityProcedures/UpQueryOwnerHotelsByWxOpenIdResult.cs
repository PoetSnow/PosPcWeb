using System;

namespace Gemstar.BSPMS.Common.Services.EntityProcedures
{
    /// <summary>
    /// 根据业主微信查询业主所在酒店信息结果集
    /// 存储过程up_queryOwnerHotelsByWxOpenId的结果集
    /// </summary>
    public class UpQueryOwnerHotelsByWxOpenIdResult
    {
        /// <summary>
        /// 业主id
        /// </summary>
        public Guid ProfileId { get; set; }
        /// <summary>
        /// 酒店id
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 酒店名称
        /// </summary>
        public string Name { get; set; }
    }
}
