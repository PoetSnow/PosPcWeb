using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 要求属性(0：一般；1：追单；2：叫起；3：起菜)
    /// </summary>
    public enum PosRequestProperty : byte
    {
        //要求属性(0：一般；1：追单；2：叫起；3：起菜)

        [Description("一般")]
        一般 = 0,
        [Description("追单")]
        追单 = 1,
        [Description("叫起")]
        叫起 = 2,
        [Description("起菜")]
        起菜 = 3
    }
}
