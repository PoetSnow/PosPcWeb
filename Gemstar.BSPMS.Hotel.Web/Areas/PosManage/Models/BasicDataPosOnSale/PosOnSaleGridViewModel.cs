﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosOnSale
{
    /// <summary>
    /// 特价菜表格数据模型
    /// </summary>
    public class PosOnSaleGridViewModel
    {
        public Guid Id { get; set; }
        public string hid { get; set; }

        public string iTagperiod { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string itemid { get; set; }

        public decimal? price { get; set; }
        public decimal? discount { get; set; }

        public string Module { get; set; }
        public string Remark { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedDateStra { get; set; }
        public string refeName { get; set; }
        public string customerTypeName { get; set; }
        public string tabTypeName { get; set; }
        public string itemName { get; set; }
        public string unitName { get; set; }
        public string isLimitText { get; set; }
        public string isServiceText { get; set; }
        public string isDiscountText { get; set; }
        public string iCmpTypeText { get; set; }
        public string isUsedText { get; set; }
        public string iTagperiodName { get; set; }
    }
}