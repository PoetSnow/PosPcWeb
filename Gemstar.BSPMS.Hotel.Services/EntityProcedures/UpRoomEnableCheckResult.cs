using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 房间可用性检查结果，如果有记录则表示是有冲突，需要提示给操作人员
    /// </summary>
    public class UpRoomEnableCheckResult
    {
        /// <summary>
        /// 冲突房号
        /// </summary>
        public string RoomNo { get; set; }
        /// <summary>
        /// 冲突房型名称
        /// </summary>
        public string RoomTypeName { get; set; }
        /// <summary>
        /// 冲突日期
        /// </summary>
        public string Usedate { get; set; }
        /// <summary>
        /// 冲突说明
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否可继续保存
        /// 0:不能保存
        /// 1：需要提示操作员是否继续，如果操作员选择继续则可以保存
        /// </summary>
        public byte CanSave { get; set; }
    }
}
