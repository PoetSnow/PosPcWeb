using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    public class UpQueryOperationLog
    {
        public string hid { get; set; }
        public string cDate { get; set; }
        public string cUser { get; set; }
        public string ip { get; set; }
        public string xType { get; set; }
        public string cText { get; set; }
        public string keys { get; set; }
    }
    public class ResLogQueryPara
    {
        public string HotelId { get; set; }
        public string operationDateBegin { get; set; }
        public string operationDateEnd { get; set; }
        public string operationContent { get; set; }
        public string operators { get; set; }
        public string ip { get; set; }
        public string opeartionType { get; set; }
        public string opeartionNo { get; set; }

        /// <summary>
        /// 客账号
        /// </summary>
        public string billNo { get; set; }
    }
}
