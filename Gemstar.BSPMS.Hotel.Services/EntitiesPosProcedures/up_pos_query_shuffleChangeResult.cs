using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    public class up_pos_query_shuffleChangeResult
    {
        /// <summary>
        /// 营业点
        /// </summary>
        [Display(Name = "营业点")]
        public string id { get; set; }

        /// <summary>
        /// 营业点
        /// </summary>
        [Display(Name = "营业点")]
        public string Cname { get; set; }

        /// <summary>
        /// 当前营业日
        /// </summary>
        [Display(Name = "当前营业日")]
        public string Business { get; set; }

        /// <summary>
        /// 市别id
        /// </summary>
        [Display(Name = "请选择市别")]
        public string Shuffleid { get; set; }

        /// <summary>
        /// 当前市别
        /// </summary>
        [Display(Name = "当前市别")]
        public string ShuffleName { get; set; }
    }
}
