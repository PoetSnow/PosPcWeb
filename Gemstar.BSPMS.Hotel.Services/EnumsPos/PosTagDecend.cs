using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 尾数处理方式
    /// </summary>
    public enum PosTagDecend : byte
    {
        //：0：四舍五入；1：逢一进五；2：逢一进八；3：逢四进一；8：进位；9：截位

        [Description("四舍五入")]
        四舍五入 = 0,
        [Description("逢一进五")]
        逢一进五 = 1,
        [Description("逢一进八")]
        逢一进八 = 2,
        [Description("逢四进一")]
        逢四进一 = 3,
        [Description("进位")]
        进位 = 8,
        [Description("截位")]
        截位 = 9
    }
}
