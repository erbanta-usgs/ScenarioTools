using System.Drawing;
using System;

namespace ScenarioTools.Graphics
{
    /// <summary>
    /// Represents a 32bpp RGB pixel</summary>
    public struct RgbaPixel
    {
        public byte B;
        public byte G;
        public byte R;
        public byte A;
        static int rainbowColorIndex = 0;

        public int ToInt()
        {
            return A << 24 | R << 16 | G << 8 | B;
        }

        public RgbaPixel(Color color)
        {
            B = color.B;
            G = color.G;
            R = color.R;
            A = color.A;
        }
        public RgbaPixel(byte B, byte G, byte R, byte A)
        {
            this.B = B;
            this.G = G;
            this.R = R;
            this.A = A;
        }
        public RgbaPixel(RgbaPixel pixel) : this(pixel.B, pixel.G, pixel.R, pixel.A) { }

        public override string ToString()
        {
            return "B:" + B + ", G:" + G + ", R:" + R + ", A:" + A;
        }

        public string ToStringConfig()
        {
            return (A / 16).ToString("X") + (A % 16).ToString("X") +
                   (R / 16).ToString("X") + (R % 16).ToString("X") +
                   (G / 16).ToString("X") + (G % 16).ToString("X") +
                   (B / 16).ToString("X") + (B % 16).ToString("X");
        }
        public static void SetSolidColor(RgbaPixel[,] pic, RgbaPixel color)
        {
            for (int j = 0; j < pic.GetLength(1); j++)
            {
                for (int i = 0; i < pic.GetLength(0); i++)
                    pic[i, j] = color;
            }
        }

        public static RgbaPixel GetRainbowColor()
        {
            return GetRainbowColor(rainbowColorIndex++);
        }
        public static RgbaPixel GetRainbowColor(int colorIndex)
        {
            RgbaPixel[] colors = { new RgbaPixel(0, 0, 0, 0) }; //Blue, Turquoise, Green, Yellow, Orange, Red };
            return colors[colorIndex % colors.Length];
        }

        #region predefined colors
        public static RgbaPixel Clear = new RgbaPixel(0, 0, 0, 0);
        /*
        public static RgbaPixel Black = RgbPixel.Black.
        public static RgbaPixel White = new RgbPixel(0xFF, 0xFF, 0xFF);

        public static RgbaPixel Brown = new RgbPixel(0x1E, 0x69, 0xD2);

        public static RgbaPixel LtRed = new RgbPixel(0x46, 0x46, 0xFF);
        public static RgbaPixel Red = new RgbPixel(0x0, 0x0, 0xFF);
        public static RgbaPixel DkRed = new RgbPixel(0x0, 0x0, 0x82);

        public static RgbaPixel LtOrange = new RgbPixel(0x30, 0x8F, 0xFF);
        public static RgbaPixel Orange = new RgbPixel(0x0, 0x7F, 0xFF);
        public static RgbaPixel dkOrange = new RgbPixel(0x0, 0x3C, 0xFF);

        public static RgbaPixel LtYellow = new RgbPixel(0x0, 0x64, 0x64);
        public static RgbaPixel Yellow = new RgbPixel(0x0, 0xFF, 0xFF);
        public static RgbaPixel DkYellow = new RgbPixel(0x0, 0xD9, 0xD9);

        public static RgbaPixel LtGreen = new RgbPixel(0x0, 0xFF, 0x0);
        public static RgbaPixel Green = new RgbPixel(0x0, 0x91, 0x0);
        public static RgbaPixel DkGreen = new RgbPixel(0x0, 0x5A, 0x0);

        public static RgbaPixel LtBlue = new RgbPixel(0xFF, 0xB0, 0xB0);
        public static RgbaPixel Blue = new RgbPixel(0xFF, 0x0, 0x0);
        public static RgbaPixel DkBlue = new RgbPixel(0x60, 0x0, 0x0);
        public static RgbaPixel VDkBlue = new RgbPixel(0x30, 0x0, 0x0);

        public static RgbaPixel LtPurple = new RgbPixel(0xFF, 0x6F, 0xDB);
        public static RgbaPixel Purple = new RgbPixel(0xFF, 0x0, 0xBF);
        public static RgbaPixel DkPurple = new RgbPixel(0x9B, 0x0, 0x75);

        public static RgbaPixel VLtGrey = new RgbPixel(0xF0, 0xF0, 0xF0);
        public static RgbaPixel LtGrey = new RgbPixel(0xC0, 0xC0, 0xC0);
        public static RgbaPixel Grey = new RgbPixel(0x80, 0x80, 0x80);
        public static RgbaPixel DkGrey = new RgbPixel(0x50, 0x50, 0x50);
        public static RgbaPixel VDkGrey = new RgbPixel(0x20, 0x20, 0x20);

        public static RgbaPixel VLtTurquoise = new RgbPixel(0xF4, 0xFF, 0xC4);
        public static RgbaPixel LtTurquoise = new RgbPixel(0xE9, 0xF0, 0xA4);
        public static RgbaPixel Turquoise = new RgbPixel(0xD0, 0xE0, 0x40);
        public static RgbaPixel DkTurquoise = new RgbPixel(0x74, 0x7E, 0x14);
        public static RgbaPixel VDkTurquoise = new RgbPixel(0x20, 0x23, 0x5);
         * */
        #endregion

        public Color ToColor()
        {
            return Color.FromArgb(A, R, G, B);
        }
        public RgbaPixel Brighten()
        {
            return new RgbaPixel(brighten(B), brighten(G), brighten(R), A);
        }
        byte brighten(byte channel)
        {
            int diff = 255 - channel;
            int newChannel = channel + diff / 2;
            return (byte)newChannel;
        }
        public RgbaPixel Darken()
        {
            return new RgbaPixel(darken(B), darken(G), darken(R), A);
        }
        byte darken(byte channel)
        {
            return (byte)(channel * 2 / 3);
        }
        public static int GetMinBrightness(RgbaPixel[,] image)
        {
            int minBrightness = 255;

            int width = image.GetLength(0);
            int height = image.GetLength(1);
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                {
                    int brightness = image[i, j].getBrightness();
                    minBrightness = (brightness < minBrightness ? brightness : minBrightness);
                }

            return minBrightness;
        }

        int getBrightness()
        {
            return ((int)B + (int)G + (int)R) / 3;
        }
        public static void Brighten(RgbaPixel[,] image)
        {
            int width = image.GetLength(0);
            int height = image.GetLength(1);

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    image[i, j] = image[i, j].Brighten();
        }

        public static void SetPixel(RgbaPixel[,] image, int x, int y, RgbaPixel color)
        {
            if (x >= 0 && x < image.GetLength(0) && y >= 0 && y < image.GetLength(1))
            {
                image[x, y] = color;
            }
        }
    }
}