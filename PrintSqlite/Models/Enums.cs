/// <summary>
/// 出品打印状态
/// </summary>
public enum ProdPrinterStatus
{
    等待打印 = 0,
    正在打印 = 1,
    打印成功 = 2,
    打印失败 = 51
}

/// <summary>
/// 出品打印机状态(1：正常；2：故障；51：禁用)
/// </summary>
public enum ProdPrinterIStatus
{
    正常 = 1,
    故障 = 2,
    禁用 = 51
}

/// <summary>
/// 打印标识
/// </summary>
public enum PrintSign
{
    待提交 = 0,
    已提交 = 1,
}