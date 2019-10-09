using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 获取存储过程参数的方法 up_pos_ProcedureParameters 结果集
    /// </summary>
    public class up_pos_ProcedureParametersResult
    {
        /// <summary>
        /// 参数
        /// </summary>
        public string Parameter { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public short Length { get; set; }
    }
}
