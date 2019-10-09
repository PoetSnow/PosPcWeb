using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Services.EntityProcedures
{
    public class UpHardwareInterfacecs
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionID { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        public string PicLink { get; set; }

        /// <summary>
        /// 版本描述
        /// </summary>
        public string VersionDescribe { get; set; }
    }
}
