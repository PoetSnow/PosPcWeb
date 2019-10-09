using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Common.Tools
{
    public class ResultDataModel<T>
    {
        public string OriginJsonData { get; set; }

        public T Data { get; set; }
    }
}
