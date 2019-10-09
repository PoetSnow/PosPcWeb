using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 点同项目处理
    /// </summary>
    public enum PosOrderSameItem:byte
    {
        //0：累加操作；1：新行插入；2：已点的再点提示
        [Description("累加操作")]
        累加操作 =0,
        [Description("新行插入")]
        新行插入 =1,
        [Description("已点的再点提示")]
        已点的再点提示 =2
    }
}
