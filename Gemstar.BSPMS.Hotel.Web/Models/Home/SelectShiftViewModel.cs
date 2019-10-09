using Gemstar.BSPMS.Hotel.Services.Entities;
using System.Collections.Generic;

namespace Gemstar.BSPMS.Hotel.Web.Models.Home
{
    /// <summary>
    /// 选择班次视图模型
    /// </summary>
    public class SelectShiftViewModel
    {
        public List<Shift> Shifts { get; set; }
        public string CurrentShiftId { get; set; }
        public string CurrentShiftName { get; set; }
    }
}