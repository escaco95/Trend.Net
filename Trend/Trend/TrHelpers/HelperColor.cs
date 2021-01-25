using System.Drawing;
using System.Drawing.Imaging;

namespace Trend
{
    /// <summary>
    /// 색상에 대한 다양한 연산 메소드를 제공합니다.
    /// </summary>
    public static class HelperColor
    {
        /// <summary>
        /// 색상 위에 다른 색상을 오버레이한 결과를 가져옵니다.
        /// </summary>
        public static Color Overlay(int srcA, int srcR, int srcG, int srcB, int ovrA, int ovrR, int ovrG, int ovrB)
        {
            var a = ((ovrA * ovrA) >> 8) + ((srcA * (255 - ovrA)) >> 8);
            var r = ((ovrR * ovrA) >> 8) + ((srcR * (255 - ovrA)) >> 8);
            var g = ((ovrG * ovrA) >> 8) + ((srcG * (255 - ovrA)) >> 8);
            var b = ((ovrB * ovrA) >> 8) + ((srcB * (255 - ovrA)) >> 8);
            return Color.FromArgb(a, r, g, b);
        }
        /// <summary>
        /// 색상 위에 다른 색상을 오버레이한 결과를 가져옵니다.
        /// </summary>
        public static Color Overlay(Color source, Color overlay)
            => Overlay(source.A, source.R, source.G, source.B, overlay.A, overlay.R, overlay.G, overlay.B);
        /// <summary>
        /// 색상 매트릭스를 만듭니다.
        /// </summary>
        public static ColorMatrix Matrix(int r, int g, int b)
        {
            return new ColorMatrix(new float[][]
            {
              new float[] { r/ 255F, 0, 0, 0, 0},
              new float[] {0, g / 255F, 0, 0, 0},
              new float[] {0, 0, b / 255F, 0, 0},
              new float[] {0, 0, 0, 1, 0},
              new float[] {0, 0, 0, 0, 1 }
            });
        }
        /// <summary>
        /// 색상 매트릭스를 만듭니다.
        /// </summary>
        public static ColorMatrix Matrix(Color c)
            => Matrix(c.R, c.G, c.B);
    }
}
