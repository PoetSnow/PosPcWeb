using Gemstar.BSPMS.Hotel.Services.Entities;
using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Models.Home
{
    /// <summary>
    /// 选择班次视图模型
    /// </summary>
    public class SelectPosViewModel
    {
        //public List<PosPos> posList { get; set; }
        [Display(Name = "收银点ID")]
        public string CurrentPosId { get; set; }

        [Display(Name = "收银点")]
        public string CurrentPosName { get; set; }

        [Display(Name = "当前收银点")]
        public string CurrentPosNameS { get; set; }
    }
}