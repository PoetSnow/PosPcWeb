using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Gemstar.BSPMS.Hotel.Web.Models.BasicDatas
{
    public class BasicDataGroupEditViewModel: BaseEditViewModel
    {
        /// <summary>
        /// 数据分发类型代码
        /// </summary>
        public string DataControlCode { get; set; }
        /// <summary>
        /// 数据分发类型名称
        /// </summary>
        [Display(Name = "分发方式")]
        public string DataControlName { get; set; }
        /// <summary>
        /// 选中的要分发的分店id，以逗号分隔
        /// </summary>
        [Display(Name = "分发分店")]
        public List<string> SelectedResortHids { get; set; }
        /// <summary>
        /// 有权限操作的分店列表
        /// </summary>
        public List<SelectListItem> ResortItems { get; set; }
        /// <summary>
        /// 分店是否可修改
        /// </summary>
        [Display(Name = "分店修改")]
        public bool ResortCanUpdate { get; set; }
        /// <summary>
        /// 要更新的属性
        /// </summary>
        [Display(Name = "属性列表")]
        public List<string> UpdateProperties { get; set; }
        /// <summary>
        /// 可供选择的属性
        /// </summary>
        public List<SelectListItem> PropertyItems { get; set; }
    }
}