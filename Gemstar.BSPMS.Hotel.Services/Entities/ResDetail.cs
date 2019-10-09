using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("ResDetail")]
    [LogCName("订单明细")]
    public class ResDetail
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("账号")]
        [LogAnywayWhenEdit]
        [LogStartsWithHid]
        public string Regid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("订单id")]
        [LogIgnore]
        public string Resid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("主单号")]
        [LogAnywayWhenEdit]
        public string Resno { get; set; }

        [LogCName("是否团体主单")]
        public byte Billtype { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间类型")]
        [LogRefrenceName(Sql = "SELECT name FROM roomType WHERE id={0}")]
        public string RoomTypeid { get; set; }

        [LogCName("会员id")]
        public Guid? Profileid { get; set; }

        [LogCName("客历id")]
        public Guid? Guestid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客人来源")]
        public string Sourceid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("市场分类")]
        public string Marketid { get; set; }

        [LogCName("预订间数")]
        public int? RoomQty { get; set; }

        [LogCName("抵店时间")]
        [LogDatetimeFormat(Gemstar.BSPMS.Common.Extensions.DateTimeExtension.DateTimeWithoutSecondFormatStr)]
        public DateTime? ArrDate { get; set; }

        [LogCName("离店时间")]
        [LogDatetimeFormat(Gemstar.BSPMS.Common.Extensions.DateTimeExtension.DateTimeWithoutSecondFormatStr)]
        public DateTime? DepDate { get; set; }

        [LogCName("实际入住营业日")]
        public DateTime? ArrBsnsDate { get; set; }

        [LogCName("实际离店营业日")]
        public DateTime? DepBsnsDate { get; set; }

        [LogCName("入住间数")]
        public int? RegQty { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("价格码")]
        public string RateCode { get; set; }

        [LogCName("当前房价")]
        public decimal? Rate { get; set; }

        [LogCName("早餐份数")]
        public byte? Bbf { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "varchar")]
        [LogCName("状态")]
        public string Status { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("接待状态")]
        [ConcurrencyCheck]
        public string RecStatus { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("预订状态")]
        [ConcurrencyCheck]
        public string ResStatus { get; set; }

        [LogCName("是否结账")] 
        [LogBool("是", "否")]
        public bool? IsSettle { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房号")]
        [LogAnywayWhenEdit]
        public string RoomNo { get; set; }

        [LogCName("预订保留时间")]
        public DateTime? HoldDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间id")]
        [LogIgnore]
        public string Roomid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客人名")]
        public string Guestname { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客人手机号")]
        public string GuestMobile { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("特殊要求")]
        public string Spec { get; set; }

        [LogCName("订单创建日期")]
        public DateTime? Cdate { get; set; }

        [LogCName("订单创建营业日")]
        public DateTime? BsnsDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("免收日租半日租原因")]
        public string NoDayChargeReason { get; set; }

        [LogCName("外部订单类型")]
        public byte? ExtType { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("录入操作员")]
        public string InputUser { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("入住操作员")]
        public string liveInputUser { get; set; }
    }
}
