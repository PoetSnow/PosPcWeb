using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 要求操作
    /// </summary>
    public enum PosRequestOperator : byte
    {
        //要求操作（0：单道；1：全单[当前点的全部菜式]；2：本单[新点的菜式，也就是未落单的]）

        [Description("单道")]
        单道 = 0,
        [Description("全单")]
        全单 = 1,
        [Description("本单")]
        本单 = 2
    }
}
