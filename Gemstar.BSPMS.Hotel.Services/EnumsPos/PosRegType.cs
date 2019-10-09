using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 开台属性
    /// </summary>
    public enum PosRegType : byte
    {

        [Description("无台")]
        无台 = 0,
        [Description("物理台")]
        物理台 = 1,
        [Description("指定物理台")]
        指定物理台 = 2,
        [Description("虚似台")]
        虚似台 = 3
    }
}
