using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// up_pos_scan_list_TabListByHid 根据酒店查询扫码点餐的餐台列表
    /// </summary>
    public class up_pos_scan_list_TabListByHidResult
    {
        /// <summary>
        /// 餐台ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 酒店ID
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 台号
        /// </summary>
        public string TabNo { get; set; }
        /// <summary>
        /// 中文名
        /// </summary>
        public string Cname { get; set; }
        /// <summary>
        /// 英文名
        /// </summary>
        public string Ename { get; set; }
        /// <summary>
        /// 所属营业点
        /// </summary>
        public string RefeName { get; set; }
        /// <summary>
        /// 餐台类型
        /// </summary>
        public string TabTypeName { get; set; }
        /// <summary>
        /// 模块
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 点餐码
        /// </summary>
        public string OrderBarcode { get; set; }
        /// <summary>
        /// 付款码
        /// </summary>
        public string PayBarcode { get; set; }
    }
}
