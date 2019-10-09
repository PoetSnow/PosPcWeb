using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosOnSale
{
    public class _PosonSaleQueryViewModel
    {
        public string itemname { get; set; }  //项目名称

        public string unitid { get; set; } // 单位id

        public string refeid { get; set; } //营业点id

        public string tabid { get; set;  } //餐台类型

        public string customerid { get; set; } //客人类型

        public string iTagperiod { get; set; } //日期类型
        
        public string StartTime { get; set; } //开始时间

        public string Endtime { get; set; }  //结束时间

        public int? CmpType { get; set; } //计算类型

        public int isUsed { get; set; }  //是否启用

    }
}

