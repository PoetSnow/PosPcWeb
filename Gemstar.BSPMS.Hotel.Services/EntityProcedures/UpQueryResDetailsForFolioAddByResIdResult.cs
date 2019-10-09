namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 客账入账时，用于选择的账单实例
    /// 存储过程up_queryResDetailsForFolioAddByResId执行后的结果
    /// </summary>
    public class UpQueryResDetailsForFolioAddByResIdResult
    {
        /// <summary>
        /// 账单id
        /// </summary>
        public string Regid { get; set; }
        /// <summary>
        /// 账单名称
        /// </summary>
        public string RegName { get; set; }
    }
}
