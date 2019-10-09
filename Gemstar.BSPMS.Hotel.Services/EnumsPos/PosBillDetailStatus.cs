using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 计费状态
    /// </summary>
    public enum PosBillDetailStatus : byte
    {
        //0：正常 ,1：例送；2：赠送；3：取酒；51：不加回库取消；52：加回库存取消；小于50的非正常，,不统计 取消，套餐组成 ,作废，赠送
        [Description("正常")]
        正常 = 0,
        [Description("例送")]
        例送 = 1,
        [Description("赠送")]
        赠送 = 2,
        [Description("取酒")]
        取酒 = 3,
        [Description("保存")]
        保存 = 4,
        [Description("找赎")]
        找赎 = 10,
        [Description("不加回库取消")]
        不加回库取消 = 51,
        [Description("加回库存取消")]
        加回库存取消 = 52,
        [Description("未落单取消")]
        未落单取消 = 54,
        [Description("反结取消")]
        反结取消 = 55,
        [Description("作废")]
        作废 = 56,
    }
}
