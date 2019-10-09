using System.ComponentModel;

namespace Gemstar.BSPMS.Common.Enumerator
{
    public enum InvoiceType
    {
        [Description("普通发票")]
        Nomal = 0,
        [Description("增值税专用发票")]
        Special = 1
    }
}
