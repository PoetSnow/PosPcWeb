using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.OnlineInterfaceManage
{
   public class OnlineInterfaceModel
    {
        /// <summary>
        /// 接口地址
        /// </summary>
        [Display(Name = "接口地址")]
        [Required(ErrorMessage = "请输入接口地址")]
        [RegularExpression("(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]")]
        public string RequestUrl { get; set; }
    }

    public enum InvoiceType
    {
        /// <summary>
        /// 云Pos 餐饮发票
        /// </summary>
        InvoicePosCY,
    }
}
