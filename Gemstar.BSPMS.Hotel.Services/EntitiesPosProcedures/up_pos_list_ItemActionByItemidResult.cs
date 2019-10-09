using System;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_pos_list_ItemActionByItemid 执行后的结果集对象
    /// </summary>
    public class up_pos_list_ItemActionByItemidResult
    {
        public Guid id { get; set; }

        public string hid { get; set; }

        public string Itemid { get; set; }

        public string ItemCode { get; set; }

        public string ItemName { get; set; }

        public string actionid { get; set; }

        public string ActionCode { get; set; }

        public string ActionName { get; set; }

        public bool? isByQuan { get; set; }

        public string isByQuanStr { get; set; }

        public decimal? limitQuan { get; set; }

        public bool? isByGuest { get; set; }

        public string isByGuestStr { get; set; }

        public bool? isCommon { get; set; }

        public string isCommonStr { get; set; }

        public bool? isNeed { get; set; }

        public string isNeedStr { get; set; }

        public decimal? addPrice { get; set; }

        public decimal? multiple { get; set; }

        public string prodPrinter { get; set; }

        public int? seqID { get; set; }

        public string Remark { get; set; }

        public DateTime? Modified { get; set; }

        public string ModifiedStr { get; set; }

        public byte? iType { get; set; }
    }
}