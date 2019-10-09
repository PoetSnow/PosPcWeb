namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_pos_list_billByPosid、up_pos_list_billReverseCheckout、up_pos_list_billGuestQuery 执行后的结果集对象
    /// </summary>
    public class up_pos_list_billByPosidResult
    {
        public long row { get; set; }

        public string billid { get; set; }

        public string billNo { get; set; }

        public string mBillid { get; set; }

        public string Refeid { get; set; }

        public string refeName { get; set; }

        public string tabid { get; set; }

        public string tabNo { get; set; }

        public string tabName { get; set; }

        public byte? status { get; set; }

        public int? iPrint { get; set; }

        public byte? tabFlag { get; set; }

        public string statusStr { get; set; }

        public string tabNoText { get; set; }
    }
}