using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Gma.QrCodeNet.Encoding;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Gemstar.BSPMS.Common.Tools
{
    public static class QrCodeHelper
    {
        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static byte[] GetQrCode(string url)
        {
            MemoryStream ms = null;
            try
            {
                QrEncoder encoder = new QrEncoder(ErrorCorrectionLevel.M);
                QrCode qrCode;
                encoder.TryEncode(url, out qrCode);

                GraphicsRenderer gRenderer = new GraphicsRenderer(
                    new FixedModuleSize(5, QuietZoneModules.Zero),
                    Brushes.Black, Brushes.White);

                ms = new MemoryStream();
                gRenderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);
                return ms.GetBuffer();
            }
            finally
            {
                if(ms != null)
                {
                    ms.Dispose();
                    ms.Close();
                }
            }
        }


    }
}
