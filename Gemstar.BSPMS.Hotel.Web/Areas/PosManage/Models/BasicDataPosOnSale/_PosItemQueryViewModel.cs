using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosOnSale
{
    public class _PosItemQueryViewModel
    {
        public string CodeAndName { get; set; }  //名称

        public string ItemClassid { get; set; } //大类ID

        public string SubClassid { get; set; } //分类ID

        public string DeptId { get; set; } //部门ID

        public string StartCode { get; set; }

        public string EndCode { get; set; }

    }
}