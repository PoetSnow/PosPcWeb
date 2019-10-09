namespace Gemstar.BSPMS.Hotel.Web.Areas.ScanOrder.Models
{
    public class AttchModel
    {
        /// <summary>
        /// 酒店ID
        /// </summary>
        public string Hid { get; set; }
        /// <summary>
        /// 餐台ID
        /// </summary>
        public string Tabid { get; set; }
        /// <summary>
        /// 账单ID
        /// </summary>
        public string Billid { get; set; }
        /// <summary>
        /// 付款方式ID
        /// </summary>
        public long BillDetailId { get; set; }
    }
}