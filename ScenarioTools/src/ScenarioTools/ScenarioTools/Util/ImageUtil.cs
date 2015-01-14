using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ScenarioTools.Util
{
    public static class ImageUtil
    {

        static public Bitmap BitmapFromBitmapData(byte[] BitmapData)
        {

            MemoryStream ms = new MemoryStream(BitmapData);

            return (new Bitmap(ms));

        }

        static public byte[] BitmapDataFromBitmap(Bitmap objBitmap, ImageFormat imageFormat)
        {

            MemoryStream ms = new MemoryStream();

            objBitmap.Save(ms, imageFormat);

            return (ms.GetBuffer());

        }

    }
}
