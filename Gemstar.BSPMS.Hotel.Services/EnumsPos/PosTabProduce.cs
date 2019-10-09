using System.ComponentModel;

namespace Gemstar.BSPMS.Hotel.Services.EnumsPos
{
    /// <summary>
    /// 转台通知单是否出品
    /// </summary>
    public enum PosTabProduce:byte
    {
        //0：不出品；1：传菜部；2：全部；3：指定

        [Description("不出品")]
        不出品 =0,
        [Description("传菜部")]
        传菜部 =1,
        [Description("全部")]
        全部 =2,
        [Description("指定")]
        指定 =3
    }
}
