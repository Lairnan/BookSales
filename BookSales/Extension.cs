using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BookSales
{
    public static class Extension
    {
        public static ImageSource ToImageSource(this byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            var ms = new MemoryStream(bytes);
            var bitImg = new BitmapImage();
            bitImg.BeginInit();
            bitImg.StreamSource = ms;
            bitImg.EndInit();
            return bitImg;
        }
    }
}
