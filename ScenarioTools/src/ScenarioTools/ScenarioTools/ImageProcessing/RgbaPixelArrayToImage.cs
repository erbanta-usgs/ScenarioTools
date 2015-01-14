using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

using ScenarioTools.Graphics;

namespace ScenarioTools.ImageProcessing
{
    public class RgbaPixelArrayToImage
    {
        const int MIN_WIDTH = 2;
        const int MIN_HEIGHT = 2;
        const PixelFormat PIXEL_FORMAT = PixelFormat.Format32bppArgb;

        /// <summary>
        /// Returns an Image constructed from the RGBPixel array. </summary>
        public unsafe static Image Convert(RgbaPixel[,] array)
        {
            int i, j;                   // iterators
            int width, height;          // width and height of array
            RgbaPixel* pPixel;           // points to the memory location of the current pixel
            BitmapData bitmapData;      // data returned by image.LockBits()

            // Get the width and height of the pixel array.
            width = array.GetLength(0);
            height = array.GetLength(1);

            // If the width or height are too small, return a blank image.
            if (width < MIN_WIDTH || height < MIN_HEIGHT)
            {
                return (Image)new Bitmap(MIN_WIDTH, MIN_HEIGHT);
            }

            // Create a new bitmap.
            Bitmap image = new Bitmap(width, height, PIXEL_FORMAT);

            // Lock the bitmap in memory.
            bitmapData = image.LockBits(new Rectangle(0, 0, width - 1, height - 1), ImageLockMode.WriteOnly, PIXEL_FORMAT);

            // Get the starting memory location of the image.
            pPixel = (RgbaPixel*)bitmapData.Scan0;

            // iterate through image
            for (j = 0; j < height; j++)
            {
                // set pixels
                for (i = 0; i < width; i++)
                {
                    pPixel->B = array[i, j].B;
                    pPixel->G = array[i, j].G;
                    pPixel->R = array[i, j].R;
                    pPixel->A = array[i, j].A;
                    pPixel++;
                }
            }
            image.UnlockBits(bitmapData);

            return (Image)image;
        }
    }
}
