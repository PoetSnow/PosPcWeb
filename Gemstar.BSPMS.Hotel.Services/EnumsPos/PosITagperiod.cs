using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 自动标志
    /// </summary>
    public enum PosITagperiod : byte
    {
        //0：随时；1：平时；2：周末；3：节假日
        [Description("随时")]
        随时 = 0,
        [Description("平时")]
        平时 = 1,
        [Description("周末")]
        周末 = 2,
        [Description("节假日")]
        节假日 = 3,
    }
}
