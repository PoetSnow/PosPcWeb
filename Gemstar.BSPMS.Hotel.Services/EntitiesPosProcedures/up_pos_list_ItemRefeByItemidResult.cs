using System;

namespace Gemstar.BSPMS.Hotel.Services.EntitiesPosProcedures
{
    /// <summary>
    /// 存储过程 up_pos_list_ItemRefeByItemidResult 执行后的结果集对象
    /// </summary>
    public class up_pos_list_ItemRefeByItemidResult
    {
        public Guid Id { get; set; }

        public string itemName { get; set; }

        public string RefeName { get; set; }

        public string ProdPrinter { get; set; }

        public string shuffleName { get; set; }

        public string IsDepartPrintStr { get; set; }

        public string IsTabPrintStr { get; set; }

        public int? Seqid { get; set; }

        public string Remark { get; set; }

        public string ModifiedStr { get; set; }

        public string SentPrtNo { get; set; }
    }
}
