using Gemstar.BSPMS.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Gemstar.BSPMS.Hotel.Services.Entities
{
    [Table("PermanentRoomSet")]
    public class PermanentRoomSet
    {
        public Guid Id { get; set; }

        [Key]
        [Column(Order = 1, TypeName = "varchar")]
        public string Hid { get; set; }

        [Key]
        [Column(Order = 2, TypeName = "varchar")]
        public string Regid { get; set; }

        public byte RentType { get; set; }

        public int WaterMeter { get; set; }

        public int WattMeter { get; set; }

        public int NaturalGas { get; set; }

        /// <summary>
        /// 计费周期（day：按天计费，每天生成房租和固定费用；month：按月计费，每月生成房租和固定费用）
        /// </summary>
        [Column(TypeName = "varchar")]
        public string CalculateCostCycle { get; set; }
        /// <summary>
        /// 收租周期（几天或几个月收一次租，几天或几个月生成一次房租和固定费用）押几付几
        /// </summary>
        public byte GenerateCostsCycle { get; set; }
        /// <summary>
        /// 押金 押几付几
        /// </summary>
        public byte GenerateCostsCycle_deposit { get; set; }
        /// <summary>
        /// 租金生成日期（大于0：推迟，推迟几天生成；等于0准时，准时生成；小于0：提前，提前几天生成）
        /// </summary>
        public short GenerateCostsDateAdd { get; set; }

    }
}