using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models
{
    public class LetterInputViewModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 文本框标签
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 文本值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 文本框显示/隐藏
        /// </summary>
        public string Display { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 回调函数
        /// </summary>
        public string Callback { get; set; }

        /// <summary>
        /// 输入时回调函数
        /// </summary>
        public string InputCallback { get; set; }

        /// <summary>
        /// 类型（A：（输入的不能超过100）B:类似金额）
        /// </summary>
        public string Flag { get; set; }
    }
}