using Gemstar.BSPMS.Hotel.Services.EntitiesPos;
using Gemstar.BSPMS.Hotel.Web.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.BasicDataPosItemMultiClass
{
    public class PosItemMultiClassViewModel : BaseEditViewModel
    {
        
        [Display(Name = "id")]
        public Guid Id { get; set; }

        [Display(Name = "消费项目")]
        public string itemName { get; set; }

        [Display(Name = "项目大类")]
        public string itemClassName { get; set; }

        [Display(Name = "是否分类")]
        public string isSubClassStr { get; set; }

        [Display(Name = "备注")]
        public string Remark { get; set; }

        [Display(Name = "修改时间")]
        public string ModifiedStr { get; set; }

        [Display(Name = "项目大类ID")]
        public string ItemClassidForEdit { get; set; }

        [Display(Name = "消费项目ID")]
        public string ItemId { get; set; }

        
    }





}