using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_cmp_tabStatusResult
    {
        [Display(Name = "全部")]
        public int All { get; set; }

        [Display(Name = "订台")]
        public int Reserve { get; set; }

        [Display(Name = "空台")]
        public int Empty { get; set; }

        [Display(Name = "维修")]
        public int Repair { get; set; }

        [Display(Name = "就座")]
        public int Sit { get; set; }

        [Display(Name = "清洁中")]
        public int Clean { get; set; }

        [Display(Name = "落单超时")]
        public int BeAlone { get; set; }

        [Display(Name = "打单超时")]
        public int Intimidate { get; set; }

        [Display(Name = "正在操作")]
        public int Operation { get; set; }

        [Display(Name = "总人数")]
        public int TotalNumber { get; set; }

        [Display(Name = "开台人数")]
        public int OriginalNumber { get; set; }

        [Display(Name = "已买单")]
        public int PayBill { get; set; }
    }
}
