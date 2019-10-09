using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 结转设置
    /// </summary>
    public enum PosBusinessEnd:byte
    {
        //0：当日结转；1：次日结转
        [Description("当日结转")]
        当日结转 =0,
        [Description("次日结转")]
        次日结转 =1
    }
}
