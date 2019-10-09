using System;

namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 可取消结账的批次号信息
    /// 存储过程up_queryResFolioCheckoutBatchNos的结果对象
    /// </summary>
    public class UpQueryResFolioCheckoutBatchNosResult
    {
        public string BillId { get; set; }
        public string SettleDate { get; set; }
        public string SettleUser { get; set; }
        public string Name { get; set; }
        public decimal? Amount { get; set; }
    }
}
