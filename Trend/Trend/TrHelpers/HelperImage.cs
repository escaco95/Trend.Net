using System;
using System.Drawing;
using System.IO;

namespace Trend
{
    internal static class HelperImage
    {
        public static Image ConvertFromBase64(string base64String)
        {
            byte[] data = Convert.FromBase64String(base64String);
            using (var stream = new MemoryStream(data, 0, data.Length))
            {
                return Image.FromStream(stream);
            }
        }
    }
}
