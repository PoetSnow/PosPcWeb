using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_query_shiftChangeResult
    {
        /// <summary>
        /// 收银点
        /// </summary>
        [Display(Name = "收银点")]
        public string Posid { get; set; }

        /// <summary>
        /// 当前营业日
        /// </summary>
        [Display(Name = "当前营业日")]
        public string Business { get; set; }

        /// <summary>
        /// 选择班次
        /// </summary>
        [Display(Name = "请选择班次")]
        public string Shiftid { get; set; }

        /// <summary>
        /// 当前班次
        /// </summary>
        [Display(Name = "当前班次")]
        public string ShiftName { get; set; }
    }
}
