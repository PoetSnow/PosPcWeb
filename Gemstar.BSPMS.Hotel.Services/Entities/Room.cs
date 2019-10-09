using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("Room")]
    [LogCName("房间资料表")]
    public class Room
    {

        [Column(TypeName = "varchar")]
        [LogCName("酒店代码")]
        public string Hid { get; set; }

        [Key]
        [Column(TypeName = "varchar")]
        [LogCName("房间id")]
        public string Id { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房号前缀")]
        [LogAnywayWhenEdit]
        public string PreFix { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间号")]
        [LogAnywayWhenEdit]
        public string RoomNo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间说明")]
        public string Description { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间特色")]
        public string Feature { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间类型id")]
        public string RoomTypeid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间类型代码")]
        public string RoomTypeCode { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房间类型名称")]
        public string RoomTypeName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("房型简称")]
        public string RoomTypeShortName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("分机号")]
        public string Tel { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("门锁号")]
        public string Lockid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("门锁接口信息")]
        public string LockInfo { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string Remark { get; set; }

        [LogCName("状态")]
        public EntityStatus Status { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("楼层id")]
        public string Floorid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("楼层名")]
        public string FloorName { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("销售平台")]
        public string ChannelCode { get; set; }


    }
}
