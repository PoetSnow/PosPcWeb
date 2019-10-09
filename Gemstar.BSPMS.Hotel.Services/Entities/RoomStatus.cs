using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("RoomStatus")]
    [LogCName("房态表")]
    public class RoomStatus
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("房间id")]
        public string Roomid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间号")]
        public string RoomNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房类id")]
        public string RoomTypeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房型名称")]
        public string RoomTypeName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("在住客人名")]
        public string GuestName { get; set; }

        [LogCName("预抵日期")]
        public DateTime? ArrDate { get; set; }

        [LogCName("预离日期")]
        public DateTime? DepDate { get; set; }

        [LogCName("脏净标志")]
        public byte? IsDirty { get; set; }

        [LogCName("维修标志")]
        public byte? IsService { get; set; }

        [LogCName("停用标志")]
        public byte? IsStop { get; set; }

        [LogCName("欠款金额")]
        public decimal? Balance { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("最近抵店id")]
        public string Arrid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("在住id")]
        public string Regid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("在住客人来源")]
        public string Sourceid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("在住订单号")]
        public string Resid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("在住团体名")]
        public string ResName { get; set; }

        [LogCName("维修预计完成时间")]
        public DateTime? ServiceEndDate { get; set; }

        [LogCName("停用用完成时间")]
        public DateTime? StopEndDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("置维修操作员")]
        public string ServiceUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("置停用操作员")]
        public string StorpUser { get; set; }


    }
}
