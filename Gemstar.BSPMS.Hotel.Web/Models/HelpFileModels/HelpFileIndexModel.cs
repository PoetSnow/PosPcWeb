using System.Collections.Generic;
using Gemstar.BSPMS.Common.Services.Entities;

namespace Gemstar.BSPMS.Hotel.Web.Models.HelpFileModels
{
    /// <summary>
    /// 帮助文件显示模型
    /// </summary>
    public class HelpFileIndexModel
    {
        public bool HasAuthToAdd { get; set; }
        public string menuId { get; set; }
        public string menuName { get; set; }
        public int? helpId { get; set; }
        public HelpFiles helpFile { get; set; }
        public List<HelpFiles> allHelpsInMenu { get; set; }
    }
}