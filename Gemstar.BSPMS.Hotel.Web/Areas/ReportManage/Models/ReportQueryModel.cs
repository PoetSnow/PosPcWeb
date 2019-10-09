using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gemstar.BSPMS.Hotel.Web.Areas.ReportManage.Models
{
    /// <summary>
    /// 报表查询模型
    /// </summary>
    public class ReportQueryModel
    {
        public ReportQueryModel()
        {
            IsOpenSearchWindow = true;
        }

        /// <summary>
        /// 报表代码
        /// </summary>
        public string ReportCode { get; set; }

        /// <summary>
        /// 存储过程名称
        /// </summary>
        public string ProcedureName { get; set; }

        /// <summary>
        /// 存储过程参数值
        /// </summary>
        public string ParameterValues { get; set; }

        /// <summary>
        /// 是否打开查询窗口
        /// </summary>
        public bool IsOpenSearchWindow { get; set; }

        /// <summary>
        /// 报表中文名称 SRBillReportView需要
        /// </summary>
        public string ChineseName { get; set; }
        /// <summary>
        /// 格式名称
        /// </summary>
        public string StyleName { get; set; }
    }
}