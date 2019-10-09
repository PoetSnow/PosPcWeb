using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.PosManage.Models.PosInSingle
{
    public class ChangeTableModel
    {

        [Display(Name = "原账单ID")]
        public string oldBillId { get; set; }

        [Display(Name = "原餐台ID")]
        public string oldTabId { get; set; }

        [Display(Name = "酒店ID")]
        public string newHid { get; set; }


        [Display(Name = "账单ID")]
        public string newBillId { get; set; }

        [Display(Name = "台号ID")]
        [Required(ErrorMessage = "新餐台不能为空")]
        public string newTabId { get; set; }

        [Display(Name = "台号")]
        public string newTabNo { get; set; }

        [Display(Name = "餐台")]
        public string newTabName { get; set; }

        [Display(Name = "营业点ID")]
        public string newRefeId { get; set; }

        [Display(Name = "服务费率")]
        public decimal? newServiceRate { get; set; }

        [Display(Name = "最低消费")]
        public decimal? newLimit { get; set; }

        [Display(Name = "营业点")]
        public string newRefeName { get; set; }

        [Display(Name = "服务费政策")]
        public string ServiceRateFlag { get; set; }

        [Display(Name = "开台项目")]
        public string ItemFlag { get; set; }

        [Display(Name = "控制换台之后退出哪里 A：入单界面 B：收银界面")]
        public string openFlag { get; set; }

        /// <summary>
        /// 转菜的项目与数量json 字符串
        /// </summary>
        public string ChangeItemJson { get; set; }

        /// <summary>
        /// 电脑名
        /// </summary>
        public string ComputerName { get; set; }

    }
}