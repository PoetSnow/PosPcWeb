using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace Gemstar.BSPMS.Common.Tools
{
    /// <summary>
    /// 验证码生成器
    /// </summary>
    public class CheckCodeBuilder
    {
        public const string CheckCodeKeyInSession = "CheckCodeKeyInSession";

        public CheckCodeBuilder():this(4,90,30,Level.None,Level.Medium,Level.Low,Level.Low)
        {

        }
        public CheckCodeBuilder(int length,int width,int height,Level fontSize,Level fontWarp,Level backgroundNoise,Level lineNoise)
        {
            _length = length;
            _width = width;
            _height = height;
            _fontSize = fontSize;
            _fontWarp = fontWarp;
            _backgroundNoise = backgroundNoise;
            _lineNoise = lineNoise;
            _rand = new Random();
        }
        private readonly Random _rand;
        private static readonly string[] RandomFontFamily = { "arial", "arial black", "comic sans ms", "courier new", "estrangelo edessa", "franklin gothic medium", "georgia", "lucida console", "lucida sans unicode", "mangal", "microsoft sans serif", "palatino linotype", "sylfaen", "tahoma", "times new roman", "trebuchet ms", "verdana" };
        private static readonly Color[] RandomColor = { Color.Red, Color.Green, Color.Blue, Color.Black, Color.Purple, Color.Orange };
        private readonly int _length, _width, _height;
        private readonly Level _fontSize, _fontWarp, _backgroundNoise, _lineNoise;

        /// <summary> 
        /// 生成随机的验证码中的字符串
        /// </summary> 
        public string GenerateRandomText()
        {
            const string txtChars = "ACDEFGHJKLMNPQRSTUVWXYZ2346789";
            var sb = new StringBuilder(_length);
            int maxLength = txtChars.Length;
            for (int n = 0,len=_length-1 ; n <= len; n++)
            {
                sb.Append(txtChars.Substring(_rand.Next(maxLength), 1));
            }

            return sb.ToString();
        }

        /// <summary> 
        /// 输出验证码图片 
        /// </summary> 
        public Bitmap RenderImage(string text)
        {
            var bmp = new Bitmap(_width, _height, PixelFormat.Format24bppRgb);

            using (var gr = Graphics.FromImage(bmp))
            {
                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.Clear(Color.White);

                int charOffset = 0;
                double charWidth = _width / text.Length;
                Rectangle rectChar;

                foreach (char c in text)
                {
                    // establish font and draw area 
                    using (Font fnt = GetFont())
                    {
                        using (Brush fontBrush = new SolidBrush(GetRandomColor()))
                        {
                            rectChar = new Rectangle(Convert.ToInt32(charOffset * charWidth), 0, Convert.ToInt32(charWidth), _height);

                            // warp the character 
                            GraphicsPath gp = TextPath(c.ToString(), fnt, rectChar);
                            WarpText(gp, rectChar);
                            // draw the character 
                            gr.FillPath(fontBrush, gp);
                            charOffset += 1;
                        }
                    }
                }

                var rect = new Rectangle(new Point(0, 0), bmp.Size);
                AddNoise(gr, rect);
                AddLine(gr, rect);
            }
            return bmp;
        }

        /// <summary> 
        /// Returns a random font family from the font whitelist 
        /// </summary> 
        private string GetRandomFontFamily()
        {
            return RandomFontFamily[_rand.Next(0, RandomFontFamily.Length)];
        }

        /// <summary> 
        /// Returns the CAPTCHA font in an appropriate size 
        /// </summary> 
        private Font GetFont()
        {
            float fsize;
            string fname = GetRandomFontFamily();

            switch (_fontSize)
            {
                case Level.Low:
                    fsize = Convert.ToInt32(_height * 0.8);
                    break;
                case Level.Medium:
                    fsize = Convert.ToInt32(_height * 0.85);
                    break;
                case Level.High:
                    fsize = Convert.ToInt32(_height * 0.9);
                    break;
                case Level.Extreme:
                    fsize = Convert.ToInt32(_height * 0.95);
                    break;
                default:
                    fsize = Convert.ToInt32(_height * 0.7);
                    break;
            }
            return new Font(fname, fsize, FontStyle.Bold);
        }

        /// <summary> 
        /// Get Random color. 
        /// </summary> 
        private Color GetRandomColor()
        {
            return RandomColor[_rand.Next(0, RandomColor.Length)];
        }
        private PointF RandomPoint(Rectangle rect)
        {
            return RandomPoint(rect.Left, rect.Width, rect.Top, rect.Bottom);
        }

        /// <summary> 
        /// Returns a random point within the specified rectangle 
        /// </summary>  
        private PointF RandomPoint(int xmin, int xmax, int ymin, int ymax)
        {
            return new PointF(_rand.Next(xmin, xmax), _rand.Next(ymin, ymax));
        }
        private static GraphicsPath TextPath(string s, Font f, Rectangle r)
        {
            var sf = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near };
            var gp = new GraphicsPath();
            gp.AddString(s, f.FontFamily, (int)f.Style, f.Size, r, sf);
            return gp;
        }

        /// <summary> 
        /// Warp the provided text GraphicsPath by a variable amount 
        /// </summary> 
        /// <param name="textPath">The text path.</param> 
        /// <param name="rect">The rect.</param> 
        private void WarpText(GraphicsPath textPath, Rectangle rect)
        {
            float warpDivisor;
            float rangeModifier;

            switch (_fontWarp)
            {
                case Level.Low:
                    warpDivisor = 6F;
                    rangeModifier = 1F;
                    break;
                case Level.Medium:
                    warpDivisor = 5F;
                    rangeModifier = 1.3F;
                    break;
                case Level.High:
                    warpDivisor = 4.5F;
                    rangeModifier = 1.4F;
                    break;
                case Level.Extreme:
                    warpDivisor = 4F;
                    rangeModifier = 1.5F;
                    break;
                default:
                    return;
            }

            var rectF = new RectangleF(Convert.ToSingle(rect.Left), 0, Convert.ToSingle(rect.Width), rect.Height);

            int hrange = Convert.ToInt32(rect.Height / warpDivisor);
            int wrange = Convert.ToInt32(rect.Width / warpDivisor);
            int left = rect.Left - Convert.ToInt32(wrange * rangeModifier);
            int top = rect.Top - Convert.ToInt32(hrange * rangeModifier);
            int width = rect.Left + rect.Width + Convert.ToInt32(wrange * rangeModifier);
            int height = rect.Top + rect.Height + Convert.ToInt32(hrange * rangeModifier);

            if (left < 0)
                left = 0;
            if (top < 0)
                top = 0;
            if (width > _width)
                width = _width;
            if (height > _height)
                height = _height;

            PointF leftTop = RandomPoint(left, left + wrange, top, top + hrange);
            PointF rightTop = RandomPoint(width - wrange, width, top, top + hrange);
            PointF leftBottom = RandomPoint(left, left + wrange, height - hrange, height);
            PointF rightBottom = RandomPoint(width - wrange, width, height - hrange, height);

            var points = new[] { leftTop, rightTop, leftBottom, rightBottom };
            var m = new Matrix();
            m.Translate(0, 0);
            textPath.Warp(points, rectF, m, WarpMode.Perspective, 0);
        }


        /// <summary> 
        /// Add a variable level of graphic noise to the image 
        /// </summary> 
        private void AddNoise(Graphics g, Rectangle rect)
        {
            int density;
            int size;

            switch (_backgroundNoise)
            {
                case Level.None:
                    goto default;
                case Level.Low:
                    density = 30;
                    size = 40;
                    break;
                case Level.Medium:
                    density = 18;
                    size = 40;
                    break;
                case Level.High:
                    density = 16;
                    size = 39;
                    break;
                case Level.Extreme:
                    density = 12;
                    size = 38;
                    break;
                default:
                    return;
            }
            var br = new SolidBrush(GetRandomColor());
            int max = Convert.ToInt32(Math.Max(rect.Width, rect.Height) / size);
            for (int i = 0; i <= Convert.ToInt32((rect.Width * rect.Height) / density); i++)
                g.FillEllipse(br, _rand.Next(rect.Width), _rand.Next(rect.Height), _rand.Next(max), _rand.Next(max));
            br.Dispose();
        }

        /// <summary> 
        /// Add variable level of curved lines to the image 
        /// </summary> 
        private void AddLine(Graphics g, Rectangle rect)
        {
            int length;
            float width;
            int linecount;

            switch (_lineNoise)
            {
                case Level.None:
                    goto default;
                case Level.Low:
                    length = 4;
                    width = Convert.ToSingle(_height / 31.25);
                    linecount = 1;
                    break;
                case Level.Medium:
                    length = 5;
                    width = Convert.ToSingle(_height / 27.7777);
                    linecount = 1;
                    break;
                case Level.High:
                    length = 3;
                    width = Convert.ToSingle(_height / 25);
                    linecount = 2;
                    break;
                case Level.Extreme:
                    length = 3;
                    width = Convert.ToSingle(_height / 22.7272);
                    linecount = 3;
                    break;
                default:
                    return;
            }

            var pf = new PointF[length + 1];
            using (var p = new Pen(GetRandomColor(), width))
            {
                for (int l = 1; l <= linecount; l++)
                {
                    for (int i = 0; i <= length; i++)
                        pf[i] = RandomPoint(rect);

                    g.DrawCurve(p, pf, 1.75F);
                }
            }
        }
    }
    public enum Level
    {
        None,
        Low,
        Medium,
        High,
        Extreme
    }
}
