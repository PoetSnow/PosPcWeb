using System;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 查询房态中具体房间详细信息的结果对象
    /// </summary>
    public class UpQueryRoomStatuDetailInfoResult
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int? Seqid { get; set; }
    }
}
