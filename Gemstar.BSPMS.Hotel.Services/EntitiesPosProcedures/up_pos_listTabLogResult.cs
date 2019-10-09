using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{

    /// <summary>
    /// up_pos_listTabLog 查询结果
    /// </summary>
    public class up_pos_listTabLogResult
    {

        public Guid Id { get; set; }


        public string Hid { get; set; }


        public string Refeid { get; set; }


        public string Tabid { get; set; }


        public string TabNo { get; set; }


        public string Billid { get; set; }


        public string Msg { get; set; }


        public byte? Status { get; set; }


        public string Computer { get; set; }

        public DateTime? ConnectDate { get; set; }


        public string Module { get; set; }


        public string Remark { get; set; }

        public string TransUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public string BillNo { get; set; }
    }
}
