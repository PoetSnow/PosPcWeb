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
    [Table("RoomOwnerRoomInfos")]
    [LogCName("业主房间委托")]
    public class RoomOwnerRoomInfos
    {
        [Key]
        [Required(ErrorMessage = "请输入业主房间信息")]
        [Display(Name = "业主房间信息id")]
        [LogCName("业主房间信息id")]
        public Guid RoomInfoId { get; set; }

        [Required(ErrorMessage = "请输入酒店id")]
        [Display(Name = "酒店id")]
        [LogCName("酒店id")]
        [LogIgnore]
        public string Hid { get; set; }

        [Required(ErrorMessage = "请输入业主姓名")]
        [Display(Name = "业主姓名")]
        [LogCName("业主姓名")]
        [LogRefrenceName(Sql = "SELECT guestName FROM profile WHERE id={0}")]
        public Guid ProfileId { get; set; }

        [Required(ErrorMessage = "请输入房间id")]
        [Display(Name = "房间id")]
        [LogCName("房间id")]
        [LogIgnore]
        public string RoomId { get; set; }

        [Required(ErrorMessage = "请输入房号")]
        [Display(Name = "房号")]
        [LogCName("房号")]
        [LogAnywayWhenEdit]
        public string RoomNo { get; set; }

        [Display(Name = "生效日期")]
        [LogCName("生效日期")]
        [LogRefrenceName(Sql = "select case when len({0})<17 then convert(varchar(8),{0},10) else convert(varchar(10),{0},10) end")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "失效日期")]
        [LogCName("失效日期")]
        [LogRefrenceName(Sql = "select case when len({0})<17 then convert(varchar(8),{0},10) else convert(varchar(10),{0},10) end")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "分成类型名称")]
        [LogCName("分成类型名称")]
        [LogRefrenceName(Sql = "SELECT TypeName FROM RoomOwnerCalcType WHERE TypeId={0}")]
        public Guid CalcTypeId { get; set; }

        [Display(Name = "")]
        public string r1 { get; set; }

        [Display(Name = "")]
        public string r2 { get; set; }

        [Display(Name = "")]
        public string r3 { get; set; }

        [Display(Name = "")]
        public string r4 { get; set; }

        [Display(Name = "")]
        public string r5 { get; set; }

        [Display(Name = "")]
        public string r6 { get; set; }

        [Display(Name = "")]
        public string r7 { get; set; }

        [Display(Name = "")]
        public string r8 { get; set; } 

        [Display(Name = "备注")]
        [LogCName("备注")]
        public string Remark { get; set; }
    }
}
