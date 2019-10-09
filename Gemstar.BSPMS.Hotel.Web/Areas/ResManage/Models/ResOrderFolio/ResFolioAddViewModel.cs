using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gemstar.BSPMS.Hotel.Services.EntityProcedures;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ResManage.Models.ResOrderFolio
{
    /// <summary>
    /// 入账的视图模型
    /// </summary>
    public class ResFolioAddViewModel
    {        
        /// <summary>
        /// 入账时下拉选择的账单列表
        /// </summary>
        public List<UpQueryResDetailsForFolioAddByResIdResult> RegIds { get; set; }
        /// <summary>
        /// 消费还是付款标记
        /// </summary>
        [Required(ErrorMessage ="请选择消费或付款")]
        public string FolioDCFlag { get; set; }
        /// <summary>
        /// 客账明细id
        /// </summary>
        [Required(ErrorMessage ="请选择要入账的客账明细记录")]
        public string FolioRegId { get; set; }
        /// <summary>
        /// 项目id
        /// </summary>
        [Required(ErrorMessage ="请选择要入账的项目")]
        public string FolioItemId { get; set; }
        /// <summary>
        /// 项目数量
        /// </summary>
        public decimal? FolioItemQty { get; set; }
        /// <summary>
        /// 原币含税金额
        /// </summary>
        public decimal? FolioOriAmount { get; set; }
        /// <summary>
        /// 含税金额
        /// </summary>
        [Required(ErrorMessage ="请输入要入账的金额")]
        public decimal? FolioAmount { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string FolioInvoNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string FolioRemark { get; set; }
        /// <summary>
        /// 付款项目的处理方式代码
        /// </summary>
        public string FolioItemAction { get; set; }
        /// <summary>
        /// 付款项目的处理方式对应的json格式的参数字符串
        /// </summary>
        public string FolioItemActionJsonPara { get; set; }

        /// <summary>
        /// 是否是点击结账过来的 1：结账过来的
        /// </summary>
        public int IsCheckout { get; set; }

        /// <summary>
        /// 授权完成后继续保存订单（授权主键ID+授权时间）
        /// </summary>
        public string AuthorizationSaveContinue { get; set; }

        /// <summary>
        /// 长租房押金类型
        /// </summary>
        public string FolioDepositType { get; set; }
    }
}