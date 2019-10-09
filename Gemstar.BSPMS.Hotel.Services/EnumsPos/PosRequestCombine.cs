using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 联单打印(0：非联单打印；1：联单打印[关联下一个菜式打印在一起])
    /// </summary>
    public enum PosRequestCombine : byte
    {
        //联单打印(0：非联单打印；1：联单打印[关联下一个菜式打印在一起])

        [Description("非联单打印")]
        非联单打印 = 0,
        [Description("联单打印")]
        联单打印 = 1
    }
}




