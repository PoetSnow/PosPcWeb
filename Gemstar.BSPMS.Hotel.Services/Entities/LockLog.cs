using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("LockLog")]
    [LogCName("门锁卡")]
    public class LockLog
    {
        [Key]
        [LogCName("流水号")]
        public Int64 Seqid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        [LogIgnore]
        public string Hid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("账号")]
        [LogAnywayWhenEdit]
        [LogStartsWithHid]
        [LogKey]
        public string Regid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房号")]
        public string Roomno { get; set; }

        [LogCName("发卡时间")]
        [LogDatetimeFormat(Gemstar.BSPMS.Common.Extensions.DateTimeExtension.DateTimeWithoutSecondFormatStr)]
        public DateTime CreateDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("卡号")]
        public string CardNo { get; set; }

        [LogCName("开始时间")]
        [LogDatetimeFormat(Gemstar.BSPMS.Common.Extensions.DateTimeExtension.DateTimeWithoutSecondFormatStr)]
        public DateTime BeginDate { get; set; }

        [LogCName("结束时间")]
        [LogDatetimeFormat(Gemstar.BSPMS.Common.Extensions.DateTimeExtension.DateTimeWithoutSecondFormatStr)]
        public DateTime EndDate { get; set; }

        [LogCName("状态")]
        [LogEnum(typeof(Gemstar.BSPMS.Hotel.Services.Enums.LockStatus))]
        public byte Status { get; set; }

        [LogCName("操作员")]
        [LogIgnore]
        public string InputUser { get; set; }

        [LogCName("注销时间")]
        [LogDatetimeFormat(Gemstar.BSPMS.Common.Extensions.DateTimeExtension.DateTimeWithoutSecondFormatStr)]
        public DateTime? LogoutDate { get; set; }

        /// <summary>
        /// 流水号,如果增加一张卡，要注意这个号码与之前那张卡要相同
        /// </summary>
        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string SeqNo { get; set; }

        /// <summary>
        /// 门锁类型，区分在线门锁和发卡门锁
        /// </summary>
        [LogIgnore]
        [Column(TypeName = "varchar")]
        public string Locktype { get; set; }
    }
}
