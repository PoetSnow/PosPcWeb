using Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosTabStatus;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Models.Account
{
    /// <summary>
    /// 刷卡视图模型
    /// </summary>
    public class PayByCardViewModel
    {
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardId { get; set; }

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
        /// 返回类型
        /// </summary>
        public byte ReturnType { get; set; }

        /// <summary>
        /// 餐台ID
        /// </summary>
        public string Tabid { get; set; }

        /// <summary>
        /// 餐台号
        /// </summary>

        public string TabNo { get; set; }

        /// <summary>
        /// 账单ID
        /// </summary>

        public string Billid { get; set; }

        /// <summary>
        /// 电脑名
        /// </summary>
        public string ComputerName { get; set; }

        /// <summary>
        /// 区分是正常开台还是抹台(A:正常开台，B：抹台，C：退出程序)
        /// </summary>
        public string Flag { get; set; }

    }
}