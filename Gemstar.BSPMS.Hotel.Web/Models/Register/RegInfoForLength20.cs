using System;
using System.ComponentModel.DataAnnotations;

namespace Gemstar.BSPMS.Hotel.Web.Models.Register
{
    public class RegInfoForLength20
    {
        [Required(ErrorMessage = "请输入键值，并且必须为数字")]
        public Int32? No1 { get; set; }
        [Required(ErrorMessage = "请输入键值，并且必须为数字")]
        public Int32? No2 { get; set; }
        [Required(ErrorMessage = "请输入键值，并且必须为数字")]
        public Int32? No3 { get; set; }
        [Required(ErrorMessage = "请输入键值，并且必须为数字")]
        public Int32? No4 { get; set; }
        [Required(ErrorMessage = "请输入键值，并且必须为数字")]
        public Int32? No5 { get; set; }
        /// <summary>
        /// 用户序列号
        /// </summary>
        public string UserSeriesNo { get; set; }
        /// <summary>
        /// 注册码
        /// </summary>
        public string RegNo
        {
            get
            {
                return string.Format("{0}{1}{2}{3}{4}",
                    No1.ToString().PadLeft(4, '0'),
                    No2.ToString().PadLeft(4, '0'),
                    No3.ToString().PadLeft(4, '0'),
                    No4.ToString().PadLeft(4, '0'),
                    No5.ToString().PadLeft(4, '0'));
            }
        }
    }
}
