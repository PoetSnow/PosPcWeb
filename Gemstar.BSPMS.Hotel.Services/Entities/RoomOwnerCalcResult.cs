using Gemstar.BSPMS.Common.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("RoomOwnerCalcResult")]
    [LogCName("分成类型定义")]
    public class RoomOwnerCalcResult
    {
        [Key]
        [Required(ErrorMessage = "请输入分成计算结果id")]
        [Display(Name = "分成计算结果id")]
        public Guid resultId { get; set; }

        [Required(ErrorMessage = "请输入酒店id")]
        [Display(Name = "酒店id")]
        public string hid { get; set; }

        [Required(ErrorMessage = "请输入业主id")]
        [Display(Name = "业主id")]
        public Guid profileId { get; set; }

        [Required(ErrorMessage = "请输入房间id")]
        [Display(Name = "房间id")]
        public string roomId { get; set; }

        [Required(ErrorMessage = "请输入房号")]
        [Display(Name = "房号")]
        public string roomNo { get; set; }

        [Required(ErrorMessage = "请输入计算年月")]
        [Display(Name = "计算年月")]
        public DateTime calcDate { get; set; }

        [Required(ErrorMessage = "请输入当前值的类型代码")]
        [Display(Name = "当前值的类型代码")]
        public string valueCode { get; set; }

        [Required(ErrorMessage = "请输入当前值的类型名称")]
        [Display(Name = "当前值的类型名称")]
        public string valueName { get; set; }

        [Display(Name = "当前值")]
        public string value { get; set; }

        [Display(Name = "排序号")]
        public int seqid { get; set; } 

        [Display(Name = "是否发布")]
        public bool isPublish { get; set; }

    }
}
