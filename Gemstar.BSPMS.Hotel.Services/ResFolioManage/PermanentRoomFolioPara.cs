using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.ResFolioManage
{
    public class PermanentRoomFolioPara
    {
        public class WaterAndElectricityFolioPara
        {
            /// <summary>
            /// 账号
            /// </summary>
            public string Regid { get; set; }

            /// <summary>
            /// 处理方式（51水费,52电费,53燃气费）
            /// </summary>
            public string Action { get; set; }
            /// <summary>
            /// 消费项目ID
            /// </summary>
            public string ItemId { get; set; }
            /// <summary>
            /// 消费项目名称
            /// </summary>
            public string ItemName { get; set; }


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
            public long LastTimeMeterReading { get; set; }

            /// <summary>
            /// 本次读表数
            /// </summary>
            public long ThisTimeMeterReading { get; set; }

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
    }
}
