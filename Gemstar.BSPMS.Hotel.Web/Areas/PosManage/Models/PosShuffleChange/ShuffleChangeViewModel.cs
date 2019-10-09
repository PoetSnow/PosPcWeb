using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosShuffleChange
{
    public class ShuffleChangeViewModel
    {
        /// <summary>
        /// 营业点
        /// </summary>
        [Display(Name = "请选择营业点")]
        public string Refeid { get; set; }
        /// <summary>
        /// 市别
        /// </summary>
        [Display(Name = "请选择市别")]
        public string Shuffleid { get; set; }
        /// <summary>
        /// 当前营业日
        /// </summary>
        [Display(Name = "当前营业日")]
        public string Business { get; set; }
        /// <summary>
        /// 当前市别
        /// </summary>
        [Display(Name = "当前市别")]
        public string ShuffleName { get; set; }
    }
}