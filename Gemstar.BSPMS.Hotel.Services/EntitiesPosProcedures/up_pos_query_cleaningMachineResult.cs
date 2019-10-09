using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_query_cleaningMachineResult
    {
        [Display(Name = "Posid")]
        public string PosId { get; set; }

        /// <summary>
        /// 收银点
        /// </summary>
        [Display(Name = "当前收银点")]
        public string PosName { get; set; }

        /// <summary>
        /// 当前营业日
        /// </summary>
        [Display(Name = "当前营业日")]
        public string Business { get; set; }

        /// <summary>
        /// 下个营业日
        /// </summary>
        [Display(Name = "下个营业日")]
        public string NextBusiness { get; set; }
    }
}
