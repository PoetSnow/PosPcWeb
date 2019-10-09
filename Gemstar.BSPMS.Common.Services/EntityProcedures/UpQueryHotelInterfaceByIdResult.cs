using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Services.EntityProcedures
{
    public class UpQueryHotelInterfaceByIdResult
    {
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 硬件接口版本
        /// </summary>
        public string EditionName { get; set; }
    }
}
