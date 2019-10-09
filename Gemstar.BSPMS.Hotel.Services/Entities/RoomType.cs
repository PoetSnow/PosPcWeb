using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemstar.BSPMS.Common.Services;
using Gemstar.BSPMS.Common.Services.BasicDataControls;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("RoomType")]
    [LogCName("房间类型")]
    public class RoomType: IBasicDataCopyEntity, IEntityEnable
    {
        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("房类id")]
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
        [LogCName("简称")]
        [BasicDataUpdate(UpdateName = "简称")]
        public string ShortName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("床型")]
        [BasicDataUpdate(UpdateName = "床型")]
        public string BedType { get; set; }

        [LogCName("牌价")]
        [BasicDataUpdate(UpdateName = "牌价")]
        public decimal? Price { get; set; }

        [LogCName("标准人数")]
        [BasicDataUpdate(UpdateName = "标准人数")]
        public byte? Count { get; set; }

        [LogCName("最大人数")]
        [BasicDataUpdate(UpdateName = "最大人数")]
        public byte? MaxCount { get; set; }

        [LogCName("可否加床")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "可否加床")]
        public bool? IsAdd { get; set; }

        [LogCName("是否渠道可用")] 
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "是否渠道可用")]
        public bool? ChanelValid { get; set; }

        [LogCName(" ")]
        [BasicDataUpdate(UpdateName = "超预订数")]
        public int? OverQauntity { get; set; }

        [LogCName("是否关闭渠道")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "是否关闭渠道")]
        public bool? IsClose { get; set; }

        [LogCName("是否假房")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "是否假房")]
        public bool? IsNotRoom { get; set; }

        [LogCName("状态")]
        [BasicDataUpdate(UpdateName = "状态")]
        public EntityStatus Status { get; set; }

        [LogIgnore]
        [LogCName("总房数")]
        [BasicDataUpdate(UpdateName = "总房数")]
        public int? TotalRooms { get; set; }

        [LogCName("排序号")]
        [BasicDataUpdate(UpdateName = "排序号")]
        public int? Seqid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房型描述")]
        [BasicDataUpdate(UpdateName = "房型描述")]
        public string Cdescribe { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房型图片")]
        [BasicDataUpdate(UpdateName = "房型图片")]
        public string PicAdd { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("rt1")]
        public string Rt1 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("rt2")]
        public string Rt2 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("rt3")]
        public string Rt3 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("rt4")]
        public string Rt4 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("rt5")]
        public string Rt5 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("rt6")]
        public string Rt6 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("rt7")]
        public string Rt7 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("rt8")]
        public string Rt8 { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("数据来源")]
        public string DataSource { get; set; }
        /// <summary>
        /// 数据分发id
        /// </summary>
        [LogCName("数据分发id")]
        [Column(TypeName = "varchar")]
        public string DataCopyId { get; set; }

        [LogCName("夜审置脏")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "夜审置脏")]
        public bool? isChangeDirty { get; set; }

        [LogCName("离店置脏")]
        [LogBool("是", "否")]
        [BasicDataUpdate(UpdateName = "离店置脏")]
        public bool? isDepChangeDirty { get; set; }

    }
}
