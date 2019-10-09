using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.ResManage
{
    public class PermanentRoomInfo
    {
        public class PermanentRoomSet
        {
            public Guid Id { get; set; }

            public string Hid { get; set; }

            public string Regid { get; set; }

            public byte RentType { get; set; }

            public int WaterMeter { get; set; }

            public int WattMeter { get; set; }

            public int NaturalGas { get; set; }

            public List<PermanentRoomFixedCostSet> PermanentRoomFixedCostSets { get; set; }
        }

        public class PermanentRoomFixedCostSet
        {
            public int Id { get; set; }

            public string Hid { get; set; }

            public Guid PermanentRoomSetId { get; set; }

            public string Itemid { get; set; }

            public decimal Amount { get; set; }

            /// <summary>
            /// 类型（1加收，2包费）
            /// </summary>
            public byte? Type { get; set; }
        }


        public class PermanentRoomSetPara
        {
            public Guid? Id { get; set; }

            public string Hid { get; set; }

            public string Regid { get; set; }

            public byte RentType { get; set; }

            public int WaterMeter { get; set; }

            public int WattMeter { get; set; }

            public int NaturalGas { get; set; }

            public List<PermanentRoomFixedCostSetPara> FixedCost { get; set; }
        }

        public class PermanentRoomFixedCostSetPara
        {
            public string Itemid { get; set; }

            public decimal Amount { get; set; }

            /// <summary>
            /// 类型（1加收，2包费）
            /// </summary>
            public byte? Type { get; set; }
        }


        public class PermanentRoomImportFolioPara
        {
            /// <summary>
            /// 账号
            /// </summary>
            public string Regid { get; set; }

            /// <summary>
            /// 短账号
            /// </summary>
            public string ShortRegid { get; set; }

            /// <summary>
            /// 房号
            /// </summary>
            public string RoomNo { get; set; }

            /// <summary>
            /// 上次读表数
            /// </summary>
            public long? LastTimeMeterReading { get; set; }

            /// <summary>
            /// 本次读表数
            /// </summary>
            public long? ThisTimeMeterReading { get; set; }

            /// <summary>
            /// 数量
            /// </summary>
            public int? Quantity { get; set; }
            /// <summary>
            /// 单价
            /// </summary>
            public decimal? Price { get; set; }

            /// <summary>
            /// 金额
            /// </summary>
            public decimal? AmountD { get; set; }

            /// <summary>
            /// 单号
            /// </summary>
            public string InvNo { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            public string Remark { get; set; }
        }

        public class LastMeter
        {
            public int Water { get; set; }

            public int Watt { get; set; }

            public int Gas { get; set; }
        }

    }
}
