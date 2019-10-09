using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_changeItemListResult
    {
        public string 原餐台 { get; set; }
        public string 新餐台 { get; set; }
        public string 项目名称 { get; set; }
        public string 单位 { get; set; }
        public decimal? 数量 { get; set; }
        public string 作法 { get; set; }
        public string 要求 { get; set; }
        public decimal? 折前金额 { get; set; }
        public string 转菜人 { get; set; }
        public DateTime? 转菜时间 { get; set; }
        
      
    }
}
