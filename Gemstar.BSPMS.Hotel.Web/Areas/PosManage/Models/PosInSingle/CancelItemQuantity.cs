using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class CancelItemQuantity
    {
        public string Id { get; set; }


        /// <summary>
        /// 账单明细状态
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 取消原因
        /// </summary>
        public string canReason { get; set; }

        /// <summary>
        /// 类型 0：赠送 1：取消
        /// </summary>
        public string istagtype { get; set; }

        /// <summary>
        /// 是否加回库存
        /// </summary>
        public string isreuse { get; set; }


        /// <summary>
        /// 回调函数
        /// </summary>
        public string Callback { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal? quantity { get; set; }



        /// <summary>
        /// 输入时回调函数
        /// </summary>
        public string InputCallback { get; set; }


        /// <summary>
        /// 消费项目
        /// </summary>
        public string itemName { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 取消的数量
        /// </summary>
        public decimal? CancelNum { get; set; }

    }
}