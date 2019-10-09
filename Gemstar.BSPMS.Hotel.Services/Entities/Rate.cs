using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("Rate")]
    [LogCName("价格体系")]
    public class Rate : IBasicDataCopyEntity, IEntityEnable
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("id")]
        [LogIgnore]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("代码")]
        public string Code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("名称")]
        [LogAnywayWhenEdit]
        [BasicDataUpdate(UpdateName = "名称")]
        public string Name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("市场分类")]
        [LogRefrenceName(Sql = "SELECT name FROM codeList WHERE id={0}")]
        [BasicDataUpdate(UpdateName = "市场分类")]
        public string Marketid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("客人来源")]
        [LogRefrenceName(Sql = "SELECT name FROM codeList WHERE id={0}")]
        [BasicDataUpdate(UpdateName = "客人来源")]
        public string Sourceid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("半日租收取时间")]
        [BasicDataUpdate(UpdateName = "半日租收取时间")]
        public string HalfTime { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("日租收取时间")]
        [BasicDataUpdate(UpdateName = "日租收取时间")]
        public string DayTime { get; set; }

        [LogCName("预订须知")]
        [LogRefrenceName(Sql = "SELECT name FROM bookingNotes WHERE id = {0}")]
        [BasicDataUpdate(UpdateName = "预订须知")]
        public Guid? BookingNotesid { get; set; }

        [LogCName("是否适用散客")] 
        [LogBool("适用","不适用")]
        [BasicDataUpdate(UpdateName = "是否适用散客")]
        public bool? IsWalkIn { get; set; }

        [LogCName("是否有免费早餐数")]
        [LogBool("有","无")]
        [BasicDataUpdate(UpdateName = "是否有免费早餐数")]
        public byte? Bbf { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("携程价格代码")]
        [BasicDataUpdate(UpdateName = "携程价格代码")]
        [LogRefrenceName(Sql = "SELECT name FROM v_codeListPub WHERE  typeCode = '20' AND code = {0}")]
        public string CtripCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("适用合约单位类型")]
        [BasicDataUpdate(UpdateName = "适用合约单位类型")]
        [LogRefrenceName(Sql = "SELECT name FROM codeList WHERE id={0}")]
        public string CompanyTypes { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("适合渠道")] 
        [LogRefrenceName(Sql = "SELECT name FROM dbo.channel WHERE id={0}")]
        [BasicDataUpdate(UpdateName = "适合渠道")]
        public string Channelids { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("适用会员类型")]
        [BasicDataUpdate(UpdateName = "适用会员类型")]
        [LogRefrenceName(Sql = "SELECT name FROM mbrCardType WHERE id = {0}")]
        public string MbrCardTypes { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("适合房型")] 
        [LogRefrenceName(Sql = "SELECT name FROM dbo.roomType WHERE id = {0}")]
        [BasicDataUpdate(UpdateName = "适合房型")]
        public string RoomTypeids { get; set; }

        [LogCName("生效时间")]
        [Display(Name = "生效时间")]
        [BasicDataUpdate(UpdateName = "生效时间")]
        public DateTime? BeginDate { get; set; }


        [LogCName("失效时间")]
        [Display(Name = "失效时间")]
        [BasicDataUpdate(UpdateName = "失效时间")]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("价格方式")]
        [BasicDataUpdate(UpdateName = "价格方式")]
        [LogRefrenceName(Sql = "SELECT name FROM v_codeListPub WHERE  typeCode = '18' AND code = {0}")]
        public string PriceMode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("线上付款要求")]
        [BasicDataUpdate(UpdateName = "线上付款要求")]
        [LogRefrenceName(Sql = "SELECT name FROM v_codeListPub WHERE  typeCode = '19' AND code = {0}")]
        public string PayTypeids { get; set; }

        [LogCName("增减金额")]
        [BasicDataUpdate(UpdateName = "增减金额")]
        public decimal? AddAmount { get; set; }

        [Display(Name = "加时时长")]
        [LogCName("加时时长")]
        [BasicDataUpdate(UpdateName = "加时时长")]
        public Int16? AddMinute { get; set; }

        [LogCName("增减方式")]
        [LogBool("按百分比","按金额")]
        [BasicDataUpdate(UpdateName = "增减方式")]
        public bool? AddMode { get; set; }

        [Display(Name = "加时价格")]
        [LogCName("加时价格")]
        [BasicDataUpdate(UpdateName = "加时价格")]
        public decimal? AddPrice { get; set; }

        [LogCName("增减百分比")]
        [BasicDataUpdate(UpdateName = "增减百分比")]
        public decimal? AddRate { get; set; }

        [Display(Name = "基础时长")]
        [LogCName("基础时长")]
        [BasicDataUpdate(UpdateName = "基础时长")]
        public Int16? BaseMinute { get; set; }


        [Display(Name = "是否钟点房")]
        [LogCName("是否钟点房")] 
        [BasicDataUpdate(UpdateName = "是否钟点房")]
        [LogBool("是","否")] 
        public bool? IsHou { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("引用价格代码")] 
        [LogRefrenceName(Sql = "SELECT name FROM rate WHERE id = {0}")]
        [BasicDataUpdate(UpdateName = "引用价格代码")]
        public string RefRateid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        [BasicDataUpdate(UpdateName = "备注")]
        public string Remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("服务费率")]
        [BasicDataUpdate(UpdateName = "服务费率")]
        public string SrvRate { get; set; }

        [LogCName("状态")]
        [BasicDataUpdate(UpdateName = "状态")]
        public EntityStatus Status { get; set; }

        [Display(Name = "排序号")]
        [LogCName("排序号")]
        [BasicDataUpdate(UpdateName = "排序号")]
        public int Seqid { get; set; }

        [Display(Name = "未输入会员")]
        [LogCName("未输入会员")]
        [BasicDataUpdate(UpdateName = "未输入会员")]
        public string NoPrintProfile { get; set; }

        [Display(Name = "未输入合约单位")]
        [LogCName("未输入合约单位")]
        [BasicDataUpdate(UpdateName = "未输入合约单位")]
        public string NoPrintCompany { get; set; }

        [LogCName("长包房")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "长包房")]
        public bool? isMonth { get; set; }



        [Display(Name = "白日房")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "白日房")]
        public bool? IsDayRoom { get; set; }

        [Display(Name = "白日房超时-时间")]
        [Column(TypeName = "char")]
        [BasicDataUpdate(UpdateName = "白日房超时-时间")]
        public string DayRoomTime { get; set; }

        [Display(Name = "白日房超时-分钟")]
        [BasicDataUpdate(UpdateName = "白日房超时-分钟")]
        public short? DayRoomAddMinute { get; set; }

        [Display(Name = "白日房超时-价格")]
        [BasicDataUpdate(UpdateName = "白日房超时-价格")]
        public decimal? DayRoomAddPrice { get; set; }
         
        [Display(Name = "时间段内可用-开始时间")]
        [Column(TypeName = "char")]
        [BasicDataUpdate(UpdateName = "时间段内可用-开始时间")]
        public string StartTime { get; set; }

        [Display(Name = "时间段内可用-结束时间")]
        [Column(TypeName = "char")]
        [BasicDataUpdate(UpdateName = "时间段内可用-结束时间")]
        public string EndTime { get; set; }

        [Display(Name = "可调价")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "可调价")]
        public bool? IsPriceAdjustment { get; set; }

        [Display(Name = "是否积分换房")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "是否积分换房")]
        public bool IsUseScore { get; set; }

        [Display(Name = "是否可积分")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "是否可积分")]
        public bool IsGetScore { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("数据来源")]
        public string DataSource { get; set; }
        /// <summary>
        /// 数据分发id
        /// </summary>
        [LogCName("数据分发id")]
        [Column(TypeName = "varchar")]
        public string DataCopyId { get; set; }
    }
}
