using System.Drawing;
using System;
namespace ScenarioTools.Graphics
{
    /// <summary>
    /// Represents a 24bpp RGB pixel</summary>
    public struct RgbPixel
    {
        public byte B;
        public byte G;
        public byte R;
        static int rainbowColorIndex = 0;

        public int ToInt()
        {
            return ((byte)1) << 24 | R << 16 | G << 8 | B;
        }
        public RgbPixel(int colorValue)
        {
            B = (byte)colorValue;
            G = (byte)(colorValue >> 8);
            R = (byte)(colorValue >> 16);
        }
        public static bool IsGoodColor(int colorValue)
        {
            return colorValue != 0;
        }
        public RgbPixel(string hexString)
        {
            // if the string is too short, set to black
            if (hexString.Length < 6)
            {
                B = 0;
                G = 0;
                R = 0;
            }
            // otherwise, get the values from the string (in RRGGBB hex format)
            else
            {
                byte.TryParse(hexString.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out B);
                byte.TryParse(hexString.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out G);
                byte.TryParse(hexString.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out R);
            }
        }
        public static bool TryParse(string s, out RgbPixel value)
        {
            // if the string is too short, set to black and return false
            if (s.Length < 6)
            {
                value = RgbPixel.Black;
                return false;
            }
            // otherwise, get the values from the string (in RRGGBB hex format)
            else
            {
                Byte b = 0, g = 0, r = 0;
                bool success =
                    byte.TryParse(s.Substring(4, 2), System.Globalization.NumberStyles.HexNumber, null, out b) &&
                    byte.TryParse(s.Substring(2, 2), System.Globalization.NumberStyles.HexNumber, null, out g) &&
                    byte.TryParse(s.Substring(0, 2), System.Globalization.NumberStyles.HexNumber, null, out r);
                value = new RgbPixel(b, g, r);
                return success;
            }
        }
        public RgbPixel(Color color)
        {
            B = color.B;
            G = color.G;
            R = color.R;
            //Console.WriteLine("B: " + B + ", G: " + G + ", R: " + R);
        }
        public RgbPixel(byte B, byte G, byte R)
        {
            this.B = B;
            this.G = G;
            this.R = R;
        }
        public RgbPixel(RgbPixel pixel) : this(pixel.B, pixel.G, pixel.R) { }

        public override string ToString()
        {
            return "B:" + B + ", G:" + G + ", R:" + R;
        }

        public string ToStringConfig()
        {
            return (R / 16).ToString("X") + (R % 16).ToString("X") +
                   (G / 16).ToString("X") + (G % 16).ToString("X") +
                   (B / 16).ToString("X") + (B % 16).ToString("X");
        }
        public static void SetSolidColor(RgbPixel[,] pic, RgbPixel color)
        {
            for (int j = 0; j < pic.GetLength(1); j++)
            {
                for (int i = 0; i < pic.GetLength(0); i++)
                    pic[i, j] = color;
            }
        }

        public static RgbPixel GetRainbowColor()
        {
            return GetRainbowColor(rainbowColorIndex++);
        }
        public static RgbPixel GetRainbowColor(int colorIndex)
        {
            RgbPixel[] colors = { Blue, Turquoise, Green, Yellow, Orange, Red };
            return colors[colorIndex % colors.Length];
        }

        #region predefined colors
        public static RgbPixel Black = new RgbPixel(0x0, 0x0, 0x0);
        public static RgbPixel White = new RgbPixel(0xFF, 0xFF, 0xFF);

        public static RgbPixel Brown = new RgbPixel(0x1E, 0x69, 0xD2);

        public static RgbPixel LtRed = new RgbPixel(0x46, 0x46, 0xFF);
        public static RgbPixel Red = new RgbPixel(0x0, 0x0, 0xFF);
        public static RgbPixel DkRed = new RgbPixel(0x0, 0x0, 0x82);

        public static RgbPixel LtOrange = new RgbPixel(0x30, 0x8F, 0xFF);
        public static RgbPixel Orange = new RgbPixel(0x0, 0x7F, 0xFF);
        public static RgbPixel dkOrange = new RgbPixel(0x0, 0x3C, 0xFF);

        public static RgbPixel LtYellow = new RgbPixel(0x0, 0x64, 0x64);
        public static RgbPixel Yellow = new RgbPixel(0x0, 0xFF, 0xFF);
        public static RgbPixel DkYellow = new RgbPixel(0x0, 0xD9, 0xD9);

        public static RgbPixel LtGreen = new RgbPixel(0x0, 0xFF, 0x0);
        public static RgbPixel Green = new RgbPixel(0x0, 0x91, 0x0);
        public static RgbPixel DkGreen = new RgbPixel(0x0, 0x5A, 0x0);

        public static RgbPixel LtBlue = new RgbPixel(0xFF, 0xB0, 0xB0);
        public static RgbPixel Blue = new RgbPixel(0xFF, 0x0, 0x0);
        public static RgbPixel DkBlue = new RgbPixel(0x60, 0x0, 0x0);
        public static RgbPixel VDkBlue = new RgbPixel(0x30, 0x0, 0x0);

        public static RgbPixel LtPurple = new RgbPixel(0xFF, 0x6F, 0xDB);
        public static RgbPixel Purple = new RgbPixel(0xFF, 0x0, 0xBF);
        public static RgbPixel DkPurple = new RgbPixel(0x9B, 0x0, 0x75);

        public static RgbPixel VLtGrey = new RgbPixel(0xF0, 0xF0, 0xF0);
        public static RgbPixel LtGrey = new RgbPixel(0xC0, 0xC0, 0xC0);
        public static RgbPixel Grey = new RgbPixel(0x80, 0x80, 0x80);
        public static RgbPixel DkGrey = new RgbPixel(0x50, 0x50, 0x50);
        public static RgbPixel VDkGrey = new RgbPixel(0x20, 0x20, 0x20);

        public static RgbPixel VLtTurquoise = new RgbPixel(0xF4, 0xFF, 0xC4);
        public static RgbPixel LtTurquoise = new RgbPixel(0xE9, 0xF0, 0xA4);
        public static RgbPixel Turquoise = new RgbPixel(0xD0, 0xE0, 0x40);
        public static RgbPixel DkTurquoise = new RgbPixel(0x74, 0x7E, 0x14);
        public static RgbPixel VDkTurquoise = new RgbPixel(0x20, 0x23, 0x5);
        #endregion

        public Color ToColor()
        {
            return Color.FromArgb(R, G, B);
        }
        public RgbPixel Brighten()
        {
            return new RgbPixel(brighten(B), brighten(G), brighten(R));
        }
        byte brighten(byte channel)
        {
            int diff = 255 - channel;
            int newChannel = channel + diff / 2;
            return (byte)newChannel;
        }
        public RgbPixel Darken()
        {
            return new RgbPixel(darken(B), darken(G), darken(R));
        }
        byte darken(byte channel)
        {
            return (byte)(channel * 2 / 3);
        }
        public static int GetMinBrightness(RgbPixel[,] image)
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
        public static void Brighten(RgbPixel[,] image)
        {
            int width = image.GetLength(0);
            int height = image.GetLength(1);

            for (int j = 0; j < height; j++)
                for (int i = 0; i < width; i++)
                    image[i, j] = image[i, j].Brighten();
        }

        public static void SetPixel(RgbPixel[,] image, int x, int y, RgbPixel color)
        {
            if (x >= 0 && x < image.GetLength(0) && y >= 0 && y < image.GetLength(1))
                image[x, y] = color;
        }
    }
}