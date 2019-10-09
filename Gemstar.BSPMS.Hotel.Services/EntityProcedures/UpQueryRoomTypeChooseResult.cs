using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    public class UpQueryRoomTypeChooseResult
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int? seqid { get; set; }
        /// <summary>
        /// 房间类型ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 房间类型名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 属于此房间类型的房间数量
        /// </summary>
        public int? roomqty { get; set; }
        public string rate { get; set; }
        public int? bbf { get; set; }
    }
}
