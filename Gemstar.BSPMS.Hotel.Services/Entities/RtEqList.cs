using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; 
using Gemstar.BSPMS.Common.Services;

namespace Gemstar.BSPMS.Hotel.Services.Entities
{
   public class RtEqList
    {
        [Column(TypeName = "varchar")]
        [LogCName("商品id")]
        public string Goodsid { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("商品代码")]
        public string code { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("商品名称")]
        public string name { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("备注")]
        public string remark { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("续住数量")]
        public int? xuQuality { get; set; }

        [Column(TypeName = "varchar")]
        [LogCName("离店数量")]
        public int? DepQuality { get; set; }
    }
}
