using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 显示临时台(0:不显示；1:显示)
    /// </summary>
    public enum PosShuffleHidetab : byte
    {
        //显示临时台(0:不显示；1:显示)

        [Description("不显示")]
        不显示 = 0,
        [Description("显示")]
        显示 = 1
    }
}




