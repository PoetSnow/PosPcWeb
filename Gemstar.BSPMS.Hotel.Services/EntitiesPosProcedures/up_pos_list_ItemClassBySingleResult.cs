using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_list_ItemClassBySingleResult
    {

        public string Id { get; set; }

        public string Hid { get; set; }

        public string Code { get; set; }

        public string Cname { get; set; }

        public string Ename { get; set; }

        public string Module { get; set; }

        public bool? IsSubClass { get; set; }
        public bool? IsIpadShow { get; set; }

        public string Refeid { get; set; }

        public string Bmp { get; set; }

        public int? Seqid { get; set; }

        public string Remark { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public byte? IStatus { get; set; }

        public string iTagperiod { get; set; }
    }
}
