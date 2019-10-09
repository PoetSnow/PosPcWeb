using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class BillDetailEditPlaceModel
    {
        [Display(Name ="客位信息")]
        public string Place { get; set; }

        [Display(Name = "Id集合")]
        public string Ids { get; set; }
    }
}