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
    [Table("RoomOwnerFee")]
    [LogCName("业主费用记录")]
    public class RoomOwnerFee
    {
        [Key]
        [Required(ErrorMessage = "请输入")]
        [Display(Name = "费用编号")]
        public Guid FeeId { get; set; }

        [Required(ErrorMessage = "请输入")]
        [Display(Name = "酒店代码")]
        [LogCName("酒店代码")]
        [LogIgnore]
        public string hid { get; set; }

        [Required(ErrorMessage = "请输入")]
        [Display(Name = "房间编号")]
        [LogCName("房间编号")]
        [LogIgnore]
        public string roomId { get; set; }

        [Display(Name = "业主名")]
        [LogCName("业主名")]
        [LogRefrenceName(Sql = "SELECT guestName FROM profile WHERE id={0}")]
        [LogAnywayWhenEdit]
        public Guid Profileid { get; set; }

        [Required(ErrorMessage = "请输入")]
        [Display(Name = "房号")]
        [LogCName("房号")]
        [LogAnywayWhenEdit]
        public string roomNo { get; set; }

        [Required(ErrorMessage = "请输入项目名称")]
        [Display(Name = "项目名称")]
        [LogCName("项目名称")]
        [LogRefrenceName(Sql = "SELECT name FROM item WHERE id={0}")]
        public string itemId { get; set; }

        [Required(ErrorMessage = "请输入")]
        [Display(Name = "费用日期")]
        [LogCName("费用日期")]
        [LogRefrenceName(Sql = "select case when len({0})<17 then convert(varchar(8),{0},10) else convert(varchar(10),{0},10) end")]
        public DateTime? FeeDate { get; set; }

        [Display(Name = "上次抄表数")]
        [LogCName("上次抄表数")]
        public decimal? preReadQty { get; set; }

        [Display(Name = "当前抄表数")]
        [LogCName("当前抄表数")]
        public decimal? currentReadQty { get; set; }
        
        [Display(Name = "数量")]
        [LogCName("数量")]
        public decimal? qty { get; set; }
        
        [Display(Name = "单价")]
        [LogCName("单价")]
        public decimal? price { get; set; }
         
        [Display(Name = "金额")]
        [LogCName("金额")]
        public decimal? amount { get; set; }

        [Required(ErrorMessage = "请输入")]
        [Display(Name = "操作时间")]
        [LogCName("操作时间")]
        public DateTime inputDate { get; set; }

        [Display(Name = "操作人")]
        [LogCName("操作人")]
        public string inputUser { get; set; }

        [Display(Name = "是否导入")]
        [LogCName("是否导入")] 
        [LogBool("是", "否")]
        public bool isImport { get; set; }

        [Display(Name = "备注")]
        [LogCName("备注")]
        public string Remark { get; set; }

    }
}
