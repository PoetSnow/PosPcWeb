using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    public class upJsonResultData<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
    }
}
