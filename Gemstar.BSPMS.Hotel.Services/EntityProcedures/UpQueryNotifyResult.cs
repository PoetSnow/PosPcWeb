using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    public class UpQueryNotifyResult
    {
        /// <summary>
        /// 提醒ID
        /// </summary>
       public Guid Id { get; set; }
        /// <summary>
        /// 房间号
        /// </summary>
        public string RoomNo { get; set; }
        /// <summary>
        /// 状态（已读，未读，已处理，已作废）
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 提醒ID
        /// </summary>
        public string CallTime { get; set; }
        /// <summary>
        /// 提醒内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creater { get; set; }
        /// <summary>
        /// 创建时间 
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 提醒类型
        /// </summary>
        public string WakeCallTypeName { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int? Count { get; set; }
    }
}
